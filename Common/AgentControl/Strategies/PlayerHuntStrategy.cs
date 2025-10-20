using System;
using System.Collections.Generic;
using System.Linq;
using Game.Common.AgentControl.Navigation;
using Godot;

namespace Game.Common.AgentControl.Strategies;

[GlobalClass]
public partial class PlayerHuntStrategy : Node, IAgentTaskProvider, IAgentEventListener<PlayerVisibleEvent>
{
	[Export] float sightingDecayRate = 1f;
	[Export] float sightingThreshold = 5f;
	[Export] float minExtrapolationDistance = 1000f;
	[Export] float maxExtrapolationDistance = 2000f;
	[Export] int maxTargetSamples = 10;
	[Export] int maxTasksPerSighting = 3;
	[Export] int directionAveragingSamples = 5;
	[Export] float huntRecruitRadius = 500f;
	[Export] float taskExpiryTime = 15f;

	float accumulatedStrength = 0f;
	Queue<PlayerVisibleEvent> playerEventHistory =[];

	void AddEvent(PlayerVisibleEvent agentEvent)
	{
		accumulatedStrength += agentEvent.Strength;
		playerEventHistory.Enqueue(agentEvent);
		if (playerEventHistory.Count > directionAveragingSamples){
			playerEventHistory.Dequeue();
		}
	}

	public override void _Process(double delta)
	{
		accumulatedStrength -= sightingDecayRate * (float)delta;
		// DebugDraw2D.SetText("Hunt Sighting Strength", accumulatedStrength.ToString("F2"));
		accumulatedStrength = Mathf.Max(accumulatedStrength, 0f);
	}

	public void OnEvent(PlayerVisibleEvent agentEvent)
	{
		AddEvent(agentEvent);
		if (!(accumulatedStrength > sightingThreshold)) return;
		accumulatedStrength = 0f;
		BuildTasks();
	}

	Vector2 GetAveragedPlayerDirection()
	{
		Vector2 accumulatedDirection = Vector2.Zero;
		foreach (var e in playerEventHistory){
			accumulatedDirection += e.PlayerDirection.Normalized();
		}

		return (accumulatedDirection / playerEventHistory.Count).Normalized();
	}

	void BuildTasks()
	{
		PlayerVisibleEvent playerEvent = playerEventHistory.Last();
		Vector2 averageDirection = GetAveragedPlayerDirection();
		GridNavigation.ReachableArea reachArea = GameManager.Instance.GridNav.ComputeReachableArea(
			playerEvent.PlayerPosition,
			maxExtrapolationDistance
		);
		Vector2[] reachablePositions = reachArea.GetAllReachablePositions();
		Random.Shared.Shuffle(reachablePositions);

		Vector2[] possibleTargets = SelectMultipleConstrained(reachablePositions, point => {
			float distanceToSighting = reachArea.GetDistanceTo(point);
			return distanceToSighting >= minExtrapolationDistance && distanceToSighting <= maxExtrapolationDistance;
		}, maxTargetSamples);

		if (possibleTargets.Length == 0){
			// DebugDraw2D.SetText("No targets found with min distance", "Relaxing min distance constraint", duration: 2f);
			possibleTargets = SelectMultipleConstrained(reachablePositions, point => {
				float distanceToSighting = reachArea.GetDistanceTo(point);
				return distanceToSighting <= maxExtrapolationDistance;
			}, maxTargetSamples);
		}

		Vector2[] targets = possibleTargets.OrderBy(point => {
			Vector2 directionToPoint = (point - playerEvent.PlayerPosition).Normalized();
			float dot = directionToPoint.Dot(averageDirection);
			return -dot;
		}).Take(maxTasksPerSighting).ToArray();

		// DebugDraw2D.SetText("Sighting Targets", $"Found {targets.Length} targets for sighting event", duration: 2f);

		var tasks = targets.Select(target => new AgentTask{
			TargetPosition = target,
			TaskRadius = huntRecruitRadius,
			CreationTime = Time.GetTicksMsec() / 1000f,
			TaskPriority = AgentTask.Priority.Normal
		});
		pendingTasks.Clear();
		pendingTasks.AddRange(tasks);
		foreach (AgentTask pendingTask in pendingTasks){
			pendingTask.AddOnReservedCallback(RemoveTask);
		}
	}
	void RemoveTask(AgentTask task)
	{
		pendingTasks.Remove(task);
		task.RemoveOnReservedCallback(RemoveTask);
	}
	List<AgentTask> pendingTasks =[];

	static T[] SelectMultipleConstrained<T>(IList<T> candidates, Func<T, bool> constraint, int maxItemsToSelect)
	{
		if (maxItemsToSelect > candidates.Count){
			maxItemsToSelect = candidates.Count;
		}
		List<T> results =[];
		for (int attempt = 0; attempt < maxItemsToSelect; attempt++){
			T candidate = candidates[(int)(GD.Randi() % candidates.Count)];
			if (constraint(candidate)){
				results.Add(candidate);
			}
		}
		return results.ToArray();
	}

	public IEnumerable<AgentTask> GetTasks()
	{
		return pendingTasks;
	}

	void CleanupExpiredTasks()
	{
		float currentTime = Time.GetTicksMsec() / 1000f;
		pendingTasks.RemoveAll(task => (currentTime - task.CreationTime) > taskExpiryTime);
	}
}