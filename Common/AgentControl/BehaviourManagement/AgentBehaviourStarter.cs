using Godot;
using Game.Common.Utility;
namespace Game.Common.AgentControl.BehaviourManagement;

[GlobalClass]
public partial class AgentBehaviourStarter : Node
{
	public enum StartMode
	{
		FirstChild,
		AllChildren
	}
	[Export] StartMode mode= StartMode.FirstChild;
	public override void _Ready()
	{
		foreach (IAgentBehaviour behaviour in this.GetChildrenOfType<IAgentBehaviour>())
		{
			behaviour.StartBehavior();
			if(mode == StartMode.FirstChild)
			{
				break;
			}
		}
	}
}