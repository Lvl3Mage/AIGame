using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Game.Common.AgentControl.BehaviourManagement;

public class PathFollower
{
	readonly List<Vector2> points =[];

	public Vector2[] GetPathPoints()
	{
		return points.ToArray();
	}

	public PathFollower(IEnumerable<Vector2> sourcePoints = null)
	{
		if (sourcePoints != null){
			points = sourcePoints.ToList();
		}
	}
	public bool TryAdvancePath(Vector2 position, float threshold)
	{
		Vector2? target = GetCurrentTarget();
		if (target == null){
			return false;
		}

		if (position.DistanceTo(target.Value) <= threshold){
			AdvancePath();
			return true;
		}
		return false;
	}
	public void StartPathAtPosition(Vector2 position, float threshold)
	{
		int closestPointIndex = -1;
		float closestDistance = float.MaxValue;
		for (int i = 0; i < points.Count; i++){
			float distance = position.DistanceTo(points[i]);
			if (!(distance < closestDistance)) continue;
			closestDistance = distance;
			closestPointIndex = i;

		}
		if (closestPointIndex != -1 && closestDistance <= threshold){
			points.RemoveRange(0, closestPointIndex+1);
		}

	}

	public Vector2? GetCurrentTarget()
	{
		if (points.Count == 0){
			return null;
		}
		return points[0];
	}

	public Vector2? GetTargetDirection(Vector2 position)
	{
		Vector2? target = GetCurrentTarget();
		if (target == null){
			return null;
		}

		return (target.Value - position ).Normalized();

	}

	public void SetPoints(IEnumerable<Vector2> newPoints)
	{
		points.Clear();
		points.AddRange(newPoints);
	}
	public bool PathComplete()
	{
		return points.Count == 0;
	}

	public void AdvancePath()
	{

		if (points.Count == 0){
			return;
		}
		points.RemoveAt(0);
	}
}