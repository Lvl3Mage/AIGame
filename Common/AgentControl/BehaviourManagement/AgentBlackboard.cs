using Godot;
using Game.Common.Modules;

namespace Game.Common.AgentControl.BehaviourManagement;

/// <summary>
/// Contiene el estado compartido para todos los comportamientos de un agente.
/// Esto evita que los comportamientos necesiten referencias directas entre ellos.
/// </summary>
[GlobalClass]
public partial class AgentBlackboard : Node
{
    [Export] public Node2D AgentBody { get; private set; }
    [Export] public Area2D DetectionArea { get; private set; }
    [Export] public RayCast2D LineOfSightRay { get; private set; }
    [Export] public MovementModule MovementModule { get; private set; }

    public bool CanSeePlayer { get; set; } = false;
    public Vector2 LastKnownPlayerPosition { get; set; }

    private PlayerController _player;

    public override void _Ready()
    {
        _player = GameManager.Instance.Player;
    }

    public override void _Process(double delta)
    {
        UpdatePlayerDetection();
    }

    /// <summary>
    /// Comprueba si el jugador está dentro del área y si hay línea de visión directa.
    /// </summary>
    private void UpdatePlayerDetection()
    {
        var bodiesInArea = DetectionArea.GetOverlappingBodies();
        bool playerInArea = false;
        foreach (var body in bodiesInArea)
        {
            if (body == _player)
            {
                playerInArea = true;
                break;
            }
        }

        if (playerInArea)
        {
            LineOfSightRay.TargetPosition = AgentBody.ToLocal(_player.GlobalPosition);
            LineOfSightRay.ForceRaycastUpdate();

            if (LineOfSightRay.GetCollider() == _player)
            {
                CanSeePlayer = true;
                LastKnownPlayerPosition = _player.GlobalPosition;
            }
            else
            {
                CanSeePlayer = false;
            }
        }
        else
        {
            CanSeePlayer = false;
        }
    }
}