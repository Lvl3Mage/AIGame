using Godot;

namespace Game.Common.AgentControl.BehaviourManagement;

public partial class InvestigateBehaviour : Node, IAgentBehaviour
{
    [Export] AgentBlackboard blackboard;
    [Export(PropertyHint.Range, "0,30,0.1")]
    float investigationTime = 5.0f;
    [Export(PropertyHint.Range, "0,100,1")]
    float arrivalThreshold = 20f;

    public IAgentBehaviour.Priority CurrentPriority { get; private set; } = IAgentBehaviour.Priority.Disabled;

    bool 8isActive = false;
    Timer timer;
    Vector2 investigationTarget;
    bool isInvestigatingPoint = false;

    public override void _Ready()
    {
        timer = new Timer();
        AddChild(timer);
        timer.WaitTime = investigationTime;
        timer.OneShot = true;
        timer.Timeout += OnTimerTimeout;
    }

    public IAgentBehaviour.Priority GetPriority()
    {
        // Solo podemos investigar si no vemos al jugador y hay un punto de interés
        if (!blackboard.CanSeePlayer && blackboard.LastKnownPlayerPosition != Vector2.Zero)
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
        isActive = true;
        isInvestigatingPoint = false;
        investigationTarget = blackboard.LastKnownPlayerPosition;

        Vector2I start = (Vector2I)blackboard.AgentBody.GlobalPosition;
        Vector2I end = (Vector2I)investigationTarget;

        var path = GameManager.Instance.GridNav?.FindPath(start, end);
        // _blackboard.MovementModule.SetPath(path);
    }

    public void StopBehavior()
    {
        GD.Print("Agente: Finalizando investigación.");
        isActive = false;
        timer.Stop();
        // _blackboard.MovementModule.Stop();
        // Limpiamos el punto de interés para no volver a investigar lo mismo.
        blackboard.LastKnownPlayerPosition = Vector2.Zero;
    }

    public override void _Process(double delta)
    {
        if (!isActive || isInvestigatingPoint) return;

        if (blackboard.AgentBody.GlobalPosition.DistanceTo(investigationTarget) < arrivalThreshold)
        {
            GD.Print("Agente: Llegué al punto de interés. Esperando...");
            isInvestigatingPoint = true;
            // _blackboard.MovementModule.Stop();
            timer.Start();
        }
    }

    void OnTimerTimeout()
    {
        GD.Print("Agente: No encontré nada. Volviendo a la patrulla.");
        // Al terminar el tiempo, forzamos el fin del comportamiento
        // limpiando el punto de interés y dejando que el controlador re-evalúe.
        blackboard.LastKnownPlayerPosition = Vector2.Zero;
    }
}