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
	IAgentTaskProvider[] taskProviders = [];
	IAgentEventListener<PlayerVisibleEvent>[] playerVisibleEventListeners;
	IAgentEventListener<ScreamEvent>[] screamEventListeners;

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
		taskProviders = this.GetChildrenOfType<IAgentTaskProvider>().ToArray();

		playerVisibleEventListeners = this.GetChildrenOfType<IAgentEventListener<PlayerVisibleEvent>>().ToArray();
		screamEventListeners = this.GetChildrenOfType<IAgentEventListener<ScreamEvent>>().ToArray();
	}

	public override void _ExitTree()
	{
		if (Instance == this) Instance = null;
	}
	List<AgentTask> activeTasks =[];
	void UpdateActiveTasks()
	{
		activeTasks.Clear();
		foreach (IAgentTaskProvider taskProvider in taskProviders){
			activeTasks.AddRange(taskProvider.GetTasks());
		}

	}

	public void AddPlayerVisibleEvent(PlayerVisibleEvent agentEvent)
	{
		foreach (var listener in playerVisibleEventListeners){
			listener.OnEvent(agentEvent);
		}
	}
	public void AddScreamEvent(ScreamEvent agentEvent)
	{
		foreach (var listener in screamEventListeners){
			listener.OnEvent(agentEvent);
		}
	}

	public AgentTask[] GetTasksInRange(Vector2 position)
	{
		return activeTasks.Where(task => {
			Vector2 toTask = task.TargetPosition - position;
			return toTask.LengthSquared() <= task.TaskRadius*task.TaskRadius;
		}).ToArray();
	}


	public override void _Process(double delta)
	{
		UpdateActiveTasks();//todo probably shouldn't do this every frame, but for now it's fine
		// DebugDraw2D.BeginTextGroup("AgentDirector");
		//
		// Color debugColor = Colors.Gray;
		// foreach (AgentTask activeTask in activeTasks){
		// 	DebugDraw2D.SetText($"Task from {activeTask.TargetPosition}", $"Origin: {activeTask.TaskOrigin},Created: {activeTask.CreationTime:F}");
		// 	DebugDrawQueue.DebugDrawCircle(activeTask.TargetPosition, activeTask.TaskRadius, debugColor, filled:false);
		// 	DebugDrawQueue.DebugDrawCircle(activeTask.TargetPosition, 20, debugColor);
		// }
		// DebugDraw2D.EndTextGroup();
	}


}