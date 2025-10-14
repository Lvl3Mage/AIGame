using System.Linq;
using Game.Common.Utility;
using Godot;

namespace Game.Common.AgentControl.BehaviourManagement;

[GlobalClass]
public partial class BehaviourTreeController : Node
{
	IAgentBehaviour[] behaviours =[];
	IAgentBehaviour selectedBehaviour;

	public override void _Ready()
	{
		behaviours = this.GetAllChildrenOfType<IAgentBehaviour>().ToArray();

		UpdateBehaviourSelection();
	}

	public override void _Process(double delta)
	{
		UpdateBehaviourSelection();
		DebugDraw2D.SetText("Current behaviour", selectedBehaviour?.GetType().ToString());
		foreach (IAgentBehaviour agentBehaviour in behaviours){
			DebugDraw2D.SetText(agentBehaviour.GetType() + " :", agentBehaviour.GetPriority().ToString() );
		}
	}

	void UpdateBehaviourSelection()
	{
		IAgentBehaviour topBehaviour = GetTopPriorityBehaviour();
		if (selectedBehaviour != topBehaviour){
			SwitchSelectedBehaviour(topBehaviour);
		}
	}

	void SwitchSelectedBehaviour(IAgentBehaviour newBehaviour)
	{
		selectedBehaviour?.StopBehavior();
		selectedBehaviour = newBehaviour;
		selectedBehaviour?.StartBehavior();
	}

	IAgentBehaviour GetTopPriorityBehaviour()
	{
		if (behaviours.Length == 0) return null;
		IAgentBehaviour topBehaviour = null;
		IAgentBehaviour.Priority topPriority = IAgentBehaviour.Priority.Disabled;
		foreach (IAgentBehaviour agentBehaviour in behaviours){
			IAgentBehaviour.Priority priority = agentBehaviour.GetPriority();
			if (topPriority >= priority) continue;
			topPriority = priority;
			topBehaviour = agentBehaviour;
		}

		return topBehaviour;
	}
}