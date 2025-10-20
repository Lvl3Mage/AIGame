using System;
using System.Linq;
using Game.Common.AgentControl.Strategies;
using Godot;

namespace Game.Common.AgentControl.BehaviourManagement;

[GlobalClass]
public partial class TaskFulfillBehaviour : Node, IPrioritizedBehaviour
{
	[Export] AgentModules modules;
	[Export] float moveSpeed = 100f;
	AgentTask currentTask = null;
	PathFollower pathFollower = new();
	bool active = false;
	public void StartBehavior()
	{
		active = true;
	}

	public override void _Process(double delta)
	{
		if (!active) return;
		if (!pathFollower.PathComplete()){
			MoveAlongPath();
			return;
		}
		currentTask = null;
		currentTask = SelectBestTask();
		if(currentTask == null) return;
		pathFollower.SetPoints(GameManager.Instance.GridNav.GetPathBetween(modules.AgentBody.GlobalPosition, currentTask.TargetPosition));



	}

	void MoveAlongPath()
	{
		Vector2? dir = pathFollower?.GetTargetDirection(modules.AgentBody.GlobalPosition);
		if (dir == null)
		{
			return;
		}

		modules.MovementModule.SetTargetVelocity(dir.Value * moveSpeed);
	}
	AgentTask SelectBestTask()
	{
		AgentTask[] tasks = AgentDirector.Instance.GetTasksInRange(modules.AgentBody.GlobalPosition);
		//first sort by priority then by distance to agent
		tasks = tasks.OrderByDescending(t => t.TaskPriority)
			.ThenBy(t => t.TargetPosition.DistanceTo(modules.AgentBody.GlobalPosition))
			.ToArray();
		return tasks.FirstOrDefault();

	}

	public void StopBehavior()
	{
		active = false;
	}

	public IPrioritizedBehaviour.Priority GetPriority()
	{
		return IPrioritizedBehaviour.Priority.Background;
	}
}