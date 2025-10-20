using System.Collections.Generic;
using Game.Common.AgentControl.Navigation;
using Godot;

namespace Game.Common.AgentControl.Strategies;

[GlobalClass]
public partial class WanderStrategy : Node, IAgentTaskProvider
{
	[Export] int concurrentWanderTaskAmount = 15;
	[Export] float taskExpiryTime = 10f;
	List<AgentTask> tasks = new();
	void RemoveTask(AgentTask task)
	{
		tasks.Remove(task);
		task.RemoveOnReservedCallback(RemoveTask);
	}
	bool TryAddTask()
	{
		GridDefinition grid = GameManager.Instance.GridDef;
		Vector2I dimensions = new Vector2I(grid.Width, grid.Height);
		Vector2I target = new Vector2I(
			(int)(GD.Randi() % dimensions.X),
			(int)(GD.Randi() % dimensions.Y)
		);
		GD.Print(target);
		if (GameManager.Instance.GridOccupation.IsCellOccupied(target)){
			return false;
		}
		var task = new AgentTask(){
			TargetPosition = grid.GridToWorld(target),
			TaskRadius = 99999,
			TaskOrigin = grid.GridToWorld(target),
			CreationTime = Time.GetTicksMsec()/1000f + GD.Randf()*3f,
			TaskPriority = AgentTask.Priority.Background,
		};
		task.AddOnReservedCallback(RemoveTask);
		tasks.Add(task);
		return true;
	}

	public override void _Process(double delta)
	{
		CleanupExpiredTasks();
		int attempts = 0;
		while (tasks.Count < concurrentWanderTaskAmount && attempts < 1){
			if (!TryAddTask()){
				attempts++;
			}
		}
	}

	void CleanupExpiredTasks()
	{
		float currentTime = Time.GetTicksMsec() / 1000f;
		tasks.RemoveAll(task => (currentTime - task.CreationTime) > taskExpiryTime);
	}

	public IEnumerable<AgentTask> GetTasks()
	{
		return tasks;
	}
}