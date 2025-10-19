using System.Collections.Generic;
using Game.Common.AgentControl.Navigation;
using Godot;

namespace Game.Common.AgentControl;

public partial class AgentDirector : Node
{

	public over
	[Export] float taskExpiryTime = 10f;
	readonly List<AgentTask> activeTasks = [];
	float accumulatedSightingStrength = 0f;
	[Export] float sightingDecayRate = 1f;
	[Export] float sightingThreshold = 5f;
	[Export] float maxExtrapolationDistance = 2000f;
	public void AddSightingEvent(PlayerSightingEvent sightingEvent)
	{
		accumulatedSightingStrength += sightingEvent.Strength;
		if (!(accumulatedSightingStrength > sightingThreshold)) return;
		FormSightingTasks(sightingEvent);
		accumulatedSightingStrength = 0f;
	}

	void FormSightingTasks(PlayerSightingEvent sightingEvent)
	{
		GridNavigation.ReachableMap reachMap = GameManager.Instance.GridNav.ComputeReachableLookupMap(
			sightingEvent.PlayerPosition,
			maxExtrapolationDistance
		);
		Vector2[] possibleTargats = new Vector2[10];


	}

	public override void _Process(double delta)
	{
		accumulatedSightingStrength -= sightingDecayRate * (float)delta;
		accumulatedSightingStrength = Mathf.Max(accumulatedSightingStrength, 0f);
		CleanupExpiredTasks();
	}
	void CleanupExpiredTasks()
	{
		float currentTime = Time.GetTicksMsec() / 1000f;
		activeTasks.RemoveAll(task => currentTime - task.CreationTime > taskExpiryTime);
	}


	public class AgentTask
	{
		public Vector2 TargetPosition { get; init; }
		public Vector2 TaskOrigin { get; init; }
		public float TaskRadius { get; init; }
		public float CreationTime { get; init; }
	}
	public struct PlayerSightingEvent
	{
		public Vector2 PlayerPosition { get; init; }
		public Vector2 PlayerDirection { get; init; }
		public EventInfo Info { get; init; }
		public float Strength { get; init; }

	}
	public struct PlayerLostEvent
	{
		public Vector2 LastKnownPosition { get; init; }
		public Vector2 LastKnownDirection { get; init; }
		public EventInfo Info { get; init; }
	}
	public struct EventInfo
	{
		public Vector2 Origin { get; init; }
		public float Radius { get; init; }
	}

}