using Godot;

namespace Game.Common.AgentControl.BehaviourManagement;

public partial class InvestigateBehaviour : Node, IAgentBehaviour
{
    [Export] private AgentBlackboard _blackboard;
    [Export(PropertyHint.Range, "0,30,0.1")] private float _investigationTime = 5.0f;
    [Export(PropertyHint.Range, "0,100,1")] private float _arrivalThreshold = 20f;

    public IAgentBehaviour.Priority CurrentPriority { get; private set; } = IAgentBehaviour.Priority.Disabled;

    private bool _isActive = false;
    private Timer _timer;
    private Vector2 _investigationTarget;
    private bool _isInvestigatingPoint = false;

    public override void _Ready()
    {
        _timer = new Timer();
        AddChild(_timer);
        _timer.WaitTime = _investigationTime;
        _timer.OneShot = true;
        _timer.Timeout += OnTimerTimeout;
    }

    public IAgentBehaviour.Priority GetPriority()
    {
        // Solo podemos investigar si no vemos al jugador y hay un punto de interés
        if (!_blackboard.CanSeePlayer && _blackboard.LastKnownPlayerPosition != Vector2.Zero)
        {
            CurrentPriority = IAgentBehaviour.Priority.Medium;
        }
        else
        {
            CurrentPriority = IAgentBehaviour.Priority.Disabled;
        }
        return CurrentPriority;
    }

    public void StartBehavior()
    {
        GD.Print($"Agente: Investigando última posición conocida.");
        _isActive = true;
        _isInvestigatingPoint = false;
        _investigationTarget = _blackboard.LastKnownPlayerPosition;

        Vector2I start = (Vector2I)_blackboard.AgentBody.GlobalPosition;
        Vector2I end = (Vector2I)_investigationTarget;

        var path = GameManager.Instance.GridNav?.FindPath(start, end);
        _blackboard.MovementModule.SetPath(path);
    }

    public void StopBehavior()
    {
        GD.Print("Agente: Finalizando investigación.");
        _isActive = false;
        _timer.Stop();
        _blackboard.MovementModule.Stop();
        // Limpiamos el punto de interés para no volver a investigar lo mismo.
        _blackboard.LastKnownPlayerPosition = Vector2.Zero;
    }

    public override void _Process(double delta)
    {
        if (!_isActive || _isInvestigatingPoint) return;

        if (_blackboard.AgentBody.GlobalPosition.DistanceTo(_investigationTarget) < _arrivalThreshold)
        {
            GD.Print("Agente: Llegué al punto de interés. Esperando...");
            _isInvestigatingPoint = true;
            _blackboard.MovementModule.Stop();
            _timer.Start();
        }
    }

    private void OnTimerTimeout()
    {
        GD.Print("Agente: No encontré nada. Volviendo a la patrulla.");
        // Al terminar el tiempo, forzamos el fin del comportamiento
        // limpiando el punto de interés y dejando que el controlador re-evalúe.
        _blackboard.LastKnownPlayerPosition = Vector2.Zero;
    }
}