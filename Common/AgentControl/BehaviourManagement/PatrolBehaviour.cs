using Godot;

namespace Game.Common.AgentControl.BehaviourManagement;

public partial class PatrolBehaviour : Node, IAgentBehaviour
{
    [Export] private AgentBlackboard _blackboard;
    [Export] private Vector2[] _patrolPoints = [];
    [Export(PropertyHint.Range, "0,1000,10")] private float _arrivalThreshold = 40f;
    [Export]
    private bool _continuousMovement = true;

    [Export(PropertyHint.Range, "0, 10, 0.1")]
    private float _waypointPauseTime = 2.0f;


    public IAgentBehaviour.Priority CurrentPriority { get; set; } = IAgentBehaviour.Priority.Low;

    private int _currentPatrolIndex = 0;
    private bool _isActive = false;
    private Timer _pauseTimer;
    private bool _isWaiting = false;

    public override void _Ready()
    {
        _pauseTimer = new Timer();
        _pauseTimer.OneShot = true;
        _pauseTimer.Timeout += OnPauseTimerTimeout;
        AddChild(_pauseTimer);
    }


    public IAgentBehaviour.Priority GetPriority()
    {
        return CurrentPriority;
    }

    public void StartBehavior()
    {
        GD.Print("Agente: Iniciando comportamiento de Patrulla.");
        _isActive = true;
        _isWaiting = false;

        // _blackboard.MovementModule.PathLooping = _continuousMovement;
        if (_continuousMovement)
        {
            Vector2I[] gridPath = new Vector2I[_patrolPoints.Length];
            for(int i = 0; i < _patrolPoints.Length; i++)
            {
                gridPath[i] = (Vector2I)_patrolPoints[i];
            }
            // _blackboard.MovementModule.SetPath(gridPath);
        }
        else // Si es con pausas, vamos al primer punto.
        {
            SetNavigationToPoint(_currentPatrolIndex);
        }
    }

    public void StopBehavior()
    {
        GD.Print("Agente: Finalizando comportamiento de Patrulla.");
        _isActive = false;
        _pauseTimer.Stop();
        // _blackboard.MovementModule.PathLooping = false;
        // _blackboard.MovementModule.Stop();
    }

    public override void _Process(double delta)
    {
        if (!_isActive || _continuousMovement || _isWaiting || _patrolPoints.Length == 0) return;

        Vector2 targetPosition = _patrolPoints[_currentPatrolIndex];
        // if (_blackboard.AgentBody.GlobalPosition.DistanceTo(targetPosition) < _blackboard.MovementModule.ArrivalThreshold)
        // {
        //     _isWaiting = true;
        //     // _blackboard.MovementModule.Stop();
        //     _pauseTimer.WaitTime = _waypointPauseTime;
        //     _pauseTimer.Start();
        //     GD.Print($"Agente: Pausando en el punto {_currentPatrolIndex} por {_waypointPauseTime}s.");
        // }
    }

    private void OnPauseTimerTimeout()
    {
        _isWaiting = false;
        _currentPatrolIndex = (_currentPatrolIndex + 1) % _patrolPoints.Length;
        SetNavigationToPoint(_currentPatrolIndex);
    }


    private void SetNavigationToPoint(int pointIndex)
    {
        if (pointIndex < 0 || pointIndex >= _patrolPoints.Length) return;

        GD.Print($"Agente: Moviéndose al punto de patrulla {pointIndex}.");
        Vector2I start = (Vector2I)_blackboard.AgentBody.GlobalPosition;
        Vector2I end = (Vector2I)_patrolPoints[pointIndex];

        // var path = GameManager.Instance.GridNav?.FindPath(start, end);
        // _blackboard.MovementModule.SetPath(path);
    }
}