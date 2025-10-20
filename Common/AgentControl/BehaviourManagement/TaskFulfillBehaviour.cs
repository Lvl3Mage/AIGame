using System;
using System.Linq;
using Game.Common.AgentControl.Strategies;
using Game.Common.Managers;
using Godot;

namespace Game.Common.AgentControl.BehaviourManagement;

[GlobalClass]
public partial class TaskFulfillBehaviour : Node, IPrioritizedBehaviour
{
	[Export] AgentModules modules;
	[Export] float moveSpeed = 100f;
	[Export] float navPointRadius = 10f;
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
			TryAdvancePath();

			foreach (Vector2 point in  pathFollower.GetPathPoints()){
				DebugDrawQueue.DebugDrawCircle(point,30,Colors.Gray);
			}
			return;
		}

		currentTask = null;
		modules.MovementModule.SetTargetVelocity(Vector2.Zero);
		currentTask = SelectBestTask();
		if(currentTask == null) return;
		currentTask.Reserve();
		pathFollower.SetPoints(GameManager.Instance.GridNav.GetPathBetween(modules.AgentBody.GlobalPosition, currentTask.TargetPosition));



	}

	void TryAdvancePath()
	{
		Vector2? target = pathFollower.GetCurrentTarget();
		if (target == null){
			return;
		}

		float distance = (target.Value - modules.AgentBody.GlobalPosition).LengthSquared();
		if (distance <= navPointRadius*navPointRadius){
			pathFollower.AdvancePath();
		}
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
			.ThenBy(t =>PathDistance(GameManager.Instance.GridNav.GetPathBetween(modules.AgentBody.GlobalPosition, t.TargetPosition)))
			.ToArray();
		return tasks.FirstOrDefault();

	}

	float PathDistance(Vector2[] path)
	{
		if (path.Length < 2) return 0f;
		float distance = 0f;
		for (int i = 1; i < path.Length; i++){
			distance += (path[i] - path[i-1]).Length();
		}
		return distance;

	}

	public void StopBehavior()
	{
		currentTask = null;
		pathFollower.SetPoints([]);
		modules.MovementModule.SetTargetVelocity(Vector2.Zero);
		active = false;
	}

	public IPrioritizedBehaviour.Priority GetPriority()
	{
		return IPrioritizedBehaviour.Priority.Background;
	}
}