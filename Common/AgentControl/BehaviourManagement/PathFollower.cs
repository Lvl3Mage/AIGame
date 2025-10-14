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