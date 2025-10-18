using System.Linq;
using Game.Common.Utility;
using Godot;

namespace Game.Common.AgentControl.BehaviourManagement;

[GlobalClass]
public partial class PriorityBehaviourSelector : Node, IAgentBehaviour
{
	IPrioritizedBehaviour[] behaviours =[];
	IPrioritizedBehaviour selectedBehaviour;
	bool active;

	public override void _Ready()
	{
		behaviours = this.GetAllChildrenOfType<IPrioritizedBehaviour>().ToArray();
	}

	public override void _Process(double delta)
	{
		if (!active) return;
		UpdateBehaviourSelection();
		DebugDraw2D.SetText("Current behaviour", selectedBehaviour?.GetType().ToString());
		foreach (IPrioritizedBehaviour agentBehaviour in behaviours){
			DebugDraw2D.SetText(agentBehaviour.GetType() + " :", agentBehaviour.GetPriority().ToString() );
		}
	}

	void UpdateBehaviourSelection()
	{
		IPrioritizedBehaviour topBehaviour = GetTopPriorityBehaviour();
		if (selectedBehaviour != topBehaviour){
			SwitchSelectedBehaviour(topBehaviour);
		}
	}

	void SwitchSelectedBehaviour(IPrioritizedBehaviour newBehaviour)
	{
		selectedBehaviour?.StopBehavior();
		selectedBehaviour = newBehaviour;
		selectedBehaviour?.StartBehavior();
	}

	IPrioritizedBehaviour GetTopPriorityBehaviour()
	{
		if (behaviours.Length == 0) return null;
		IPrioritizedBehaviour topBehaviour = null;
		IPrioritizedBehaviour.Priority topPriority = IPrioritizedBehaviour.Priority.Disabled;
		foreach (IPrioritizedBehaviour agentBehaviour in behaviours){
			IPrioritizedBehaviour.Priority priority = agentBehaviour.GetPriority();
			if (topPriority >= priority) continue;
			topPriority = priority;
			topBehaviour = agentBehaviour;
		}

		return topBehaviour;
	}
	public void StartBehavior()
	{
		UpdateBehaviourSelection();
		active = true;
	}

	public void StopBehavior()
	{
		SwitchSelectedBehaviour(null);
		active = false;
	}
}