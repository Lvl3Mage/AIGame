using System;
using System.Collections.Generic;
using System.Linq;
using Game.Common.AgentControl.Navigation;
using Game.Common.AgentControl.Strategies;
using Game.Common.Managers;
using Game.Common.Utility;
using Godot;

namespace Game.Common.AgentControl;

[GlobalClass]
public partial class AgentDirector : Node
{
	IAgentTaskProvider[] taskCreationStrategies = [];
	IAgentEventListener<PlayerVisibleEvent>[] playerVisibleEventListener;

	public static AgentDirector Instance { get; private set; }
	public override void _EnterTree()
	{
		if (Instance != null)
		{
			GD.PrintErr("Multiple AgentDirector instances detected. There should only be one AgentDirector in the scene tree.");
			QueueFree();
			return;
		}
		Instance = this;
		taskCreationStrategies = this.GetChildrenOfType<IAgentTaskProvider>().ToArray();
		// foreach (IAgentTaskProvider strategy in taskCreationStrategies){
		// 	strategy.OnNewTasksCreated += AddNewTasks;
		// }

		playerVisibleEventListener = this.GetChildrenOfType<IAgentEventListener<PlayerVisibleEvent>>().ToArray();
	}
	void AddNewTasks(IEnumerable<AgentTask> newTasks)
	{
		foreach (AgentTask task in newTasks){
			activeTasks[task.TaskPriority].Add(task);
		}
	}

	// public over
	[Export] float taskExpiryTime = 10f;
	readonly Dictionary<AgentTask.Priority, List<AgentTask>> activeTasks = new(){
		{AgentTask.Priority.Background, []},
		{AgentTask.Priority.Normal, []},
		{AgentTask.Priority.Urgent, []}
	};
	readonly Dictionary<AgentTask.Priority,Color> debugTaskColors = new()
	{
		{ AgentTask.Priority.Background, Colors.Gray },
		{ AgentTask.Priority.Normal, Colors.Yellow },
		{ AgentTask.Priority.Urgent, Colors.Orange },
	};
	public void AddPlayerVisibleEvent(PlayerVisibleEvent agentEvent)
	{
		foreach (var listener in playerVisibleEventListener){
			listener.OnEvent(agentEvent);
		}
	}


	public override void _Process(double delta)
	{
		DebugDraw2D.BeginTextGroup("AgentDirector");

		foreach (var (priority, tasks) in activeTasks){
			Color debugColor = debugTaskColors[priority];
			foreach (AgentTask activeTask in tasks){
				DebugDraw2D.SetText($"Task from {activeTask.TargetPosition}", $"Origin: {activeTask.TaskOrigin},Created: {activeTask.CreationTime:F}");
				DebugDrawQueue.DebugDrawCircle(activeTask.TargetPosition, activeTask.TaskRadius, debugColor, filled:false);
				DebugDrawQueue.DebugDrawCircle(activeTask.TargetPosition, 20, debugColor);
			}
		}
		DebugDraw2D.EndTextGroup();
		CleanupExpiredTasks();
	}
	void CleanupExpiredTasks()
	{
		float currentTime = Time.GetTicksMsec() / 1000f;
		foreach ((_, List<AgentTask> tasks) in activeTasks){
			tasks.RemoveAll(task => currentTime - task.CreationTime > taskExpiryTime);

		}
	}


}