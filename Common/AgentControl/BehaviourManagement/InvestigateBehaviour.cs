using Game.Common.AgentControl.Navigation;
using Godot;

namespace Game.Common.AgentControl.BehaviourManagement;

public partial class ChaseBehaviour : Node, IAgentBehaviour
{
    [Export] 
    public IAgentBehaviour.Priority CurrentPriority { get; set; } = IAgentBehaviour.Priority.Low;
    
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