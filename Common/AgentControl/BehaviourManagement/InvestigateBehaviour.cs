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

    GameManager manager;
    GridNavigation mesh;
    Node2D root;
    PlayerController target;

    public override void _Ready()
    {
        root = GetOwner<Node2D>();
        manager = GetTree().CurrentScene as GameManager;
        mesh = manager.GridNavigation;
        target = manager.Player;
    }
    
    public IAgentBehaviour.Priority GetPriority()
    {
        return CurrentPriority;
    }

    public void StartBehavior()
    {
        throw new System.NotImplementedException();
    }

    public void StopBehavior()
    {
        throw new System.NotImplementedException();
    }

}