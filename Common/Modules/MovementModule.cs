using Godot;
using System.Linq;

namespace Game.Common.Modules;

/// <summary>
/// Gestiona el movimiento físico de un agente top-down.
/// Recibe un camino y se encarga de seguirlo de forma suave (lerp).
/// </summary>
[GlobalClass]
public partial class MovementModule : Node
{
    [Export]
    public CharacterBody2D AgentBody { get; private set; }

    [Export(PropertyHint.Range, "50, 1000, 10")]
    public float MaxSpeed { get; set; } = 250.0f;

    [Export(PropertyHint.Range, "1, 20, 0.1")]
    public float Acceleration { get; set; } = 8.0f;

    [Export(PropertyHint.Range, "1, 50, 1")]
    public float ArrivalThreshold { get; set; } = 5.0f;

    public bool PathLooping { get; set; } = true;


    private Vector2[] _worldPath = [];
    private int _currentPathIndex = 0;
    private Vector2 _targetVelocity = Vector2.Zero;

    private bool isDebug = true;

    public override void _PhysicsProcess(double delta)
    {
        if (AgentBody == null)
        {
            GD.PrintErr("MovementModule: AgentBody no está asignado.");
            return;
        }

        UpdateTargetVelocity();
        AgentBody.Velocity = AgentBody.Velocity.Lerp(_targetVelocity, (float)delta * Acceleration);
        AgentBody.MoveAndSlide();
    }

    /// <summary>
    /// Establece un nuevo camino para que el agente lo siga.
    /// Convierte las coordenadas del grid a coordenadas del mundo.
    /// </summary>
    public void SetPath(Vector2I[] gridPath)
    {
        if (gridPath == null || gridPath.Length == 0)
        {
            Stop();
            return;
        }

        float cellSize = GameManager.Instance.GridNav.CellSize;
        _worldPath = gridPath.Select(p => new Vector2(p.X * cellSize + cellSize / 2, p.Y * cellSize + cellSize / 2)).ToArray();
        _currentPathIndex = 0;
    }

    /// <summary>
    /// Detiene todo movimiento, limpiando el camino y la velocidad objetivo.
    /// </summary>
    public void Stop()
    {
        _worldPath = [];
        _currentPathIndex = 0;
        _targetVelocity = Vector2.Zero;
    }

    /// <summary>
    /// Lógica interna para seguir el camino punto por punto.
    /// Actualiza la _targetVelocity basándose en el siguiente punto del camino.
    /// </summary>
    private void UpdateTargetVelocity()
    {
        if (_worldPath.Length == 0 || _currentPathIndex >= _worldPath.Length)
        {
            _targetVelocity = Vector2.Zero;
            return;
        }

        if (_currentPathIndex >= _worldPath.Length)
        {
            if (PathLooping)
                _currentPathIndex = 0;
            else
            {
                Stop();
                return;
            }
        }

        Vector2 targetPoint = _worldPath[_currentPathIndex];

        if (AgentBody.GlobalPosition.DistanceTo(targetPoint) < ArrivalThreshold)
            _currentPathIndex++;

        if (_currentPathIndex >= _worldPath.Length && !PathLooping)
        {
            Stop();
            return;
        }

        targetPoint = _worldPath[_currentPathIndex % _worldPath.Length];
        Vector2 direction = (targetPoint - AgentBody.GlobalPosition).Normalized();
        _targetVelocity = direction * MaxSpeed;

        DebugDraw2D.SetText("_targetVelocity", _targetVelocity);
        DebugDraw2D.SetText("targetPoint", targetPoint);
        DebugDraw2D.SetText("AgentBody.GlobalPosition", AgentBody.GlobalPosition);
    }
}