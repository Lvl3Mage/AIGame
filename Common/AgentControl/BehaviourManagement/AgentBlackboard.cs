using System.Linq;
using Godot;
using Game.Common.Modules;
using Godot.Collections;

namespace Game.Common.AgentControl.BehaviourManagement;

//todo refactor this, change comments to english, refactor detection to use distance check + raycast
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

    public bool CanSeePlayer { get; private set; } = false;
    public Vector2 LastKnownPlayerPosition { get; private set; }
    public float TimeSinceLastSeenPlayer { get; private set; } = 0f;

    PlayerController player;

    public override void _Ready()
    {
        player = GameManager.Instance.Player;
    }

    public override void _Process(double delta)
    {

        UpdatePlayerDetection();
        if (CanSeePlayer){
            TimeSinceLastSeenPlayer = 0f;
        }
        else
        {
            TimeSinceLastSeenPlayer += (float)delta;
        }
    }

    /// <summary>
    /// Comprueba si el jugador está dentro del área y si hay línea de visión directa.
    /// </summary>
    void UpdatePlayerDetection()
    {
        Array<Node2D> bodiesInArea = DetectionArea.GetOverlappingBodies();
        bool playerInArea = bodiesInArea.Any(body => body == player);
        if (playerInArea)
        {
            LineOfSightRay.TargetPosition = AgentBody.ToLocal(player.GlobalPosition);
            LineOfSightRay.ForceRaycastUpdate();
            DebugDraw2D.SetText("Sight check");
            if (LineOfSightRay.GetCollider() == player)
            {
                CanSeePlayer = true;
                LastKnownPlayerPosition = player.GlobalPosition;
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