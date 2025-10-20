using System;
using Godot;

namespace Game.Common.AgentControl.Strategies;

public class AgentTask
{
	public enum Priority
	{
		Background,
		Normal,
		Urgent
	}
	public Priority TaskPriority { get; init; }
	public Vector2 TargetPosition { get; init; }
	public Vector2 TaskOrigin { get; init; }
	public float TaskRadius { get; init; }
	public float CreationTime { get; init; }

	public void AddOnReservedCallback(Action<AgentTask> onReserved)
	{
		OnReserved += onReserved;
	}
	public void RemoveOnReservedCallback(Action<AgentTask> onReserved)
	{
		OnReserved -= onReserved;
	}
	event Action<AgentTask> OnReserved;
	public void Reserve()
	{
		OnReserved?.Invoke(this);
	}

}