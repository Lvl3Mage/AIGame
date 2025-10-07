using Game.Common.AgentControl.BehaviourManagement;
using Game.Common.AgentControl.Navigation;
using Godot;

namespace Game.Common.AgentControl.BehaviourManagement;

public partial class InvestigateBehaviour : Node, IAgentBehaviour
{
    [Export]
    public IAgentBehaviour.Priority CurrentPriority { get; set; } = IAgentBehaviour.Priority.Low;

    // Radio de detección para iniciar el comportamiento.
    [Export(PropertyHint.Range, "1.0,100.0,0.5")]
    public float DetectionRadius { get; set; } = 10.0f;

    [Export]
    private Node2D agentNode;

    // --- Estado Interno ---
    private bool isActive = false;
    private Vector2 target; // Última posición conocida del jugador.
    private Vector2I[] path = [];
    private Vector2I investigationTarget; // Punto al que el agente se moverá para investigar.

    public override void _Ready()
    {
        if (agentNode == null)
        {
            GD.PrintErr("InvestigateBehaviour: El nodo del agente no está asignado.");
        }
    }

    // -----------------------------------------------------------------------------------------

    // Implementación IAgentBehaviour

    public IAgentBehaviour.Priority GetPriority()
    {
        return CurrentPriority;
    }

    public void StartBehavior()
    {
        if (!isActive)
        {
            GD.Print($"Agente: Jugador detectado. Iniciando comportamiento Investigate.");
            isActive = true;
        }
    }

    public void StopBehavior()
    {
        if (isActive)
        {
            GD.Print("Agente: Finalizando comportamiento Investigate.");
            isActive = false;
            path = GameManager.Instance.GridNav?.FindPath((Vector2I)Vector2.Zero, (Vector2I)Vector2.Zero); // Detiene el movimiento
        }
    }

    // -----------------------------------------------------------------------------------------


    public override void _Process(double delta)
    {
        return;
        if (agentNode.GlobalPosition.DistanceTo(GameManager.Instance.Player.Position) <= DetectionRadius)
        {
            StartBehavior();
        }
        else
        {
            StopBehavior();
        }

        if (!isActive) return;
        // if () //Si veo al jugador //Aun no se como va lo de la vision
            // investigationTarget = target;
        if (isActive)
        {
            // Configurar el objetivo de navegación para investigar el último punto conocido.
            path = GameManager.Instance.GridNav?.FindPath((Vector2I)agentNode.GlobalPosition, investigationTarget);
        }
    }

}