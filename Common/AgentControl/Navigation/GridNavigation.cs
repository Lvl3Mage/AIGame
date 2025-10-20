using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.Intrinsics.X86;
using System.Threading.Tasks;
using Godot;

namespace Game.Common.AgentControl.Navigation;

//Todo rework this to use the IGridOccupationProvider interface
[GlobalClass]
public partial class GridNavigation : Node2D
{
	[Export] GridDefinition grid;

	GridCell[,] gridCells;

	[Export] GridPhysicsOccupation gridPhysicsOccupation;

	public override void _Ready()
	{
		gridCells = new GridCell[grid.Width, grid.Height];

		for (int i = 0; i < grid.Width; i++)
		{
			for (int j = 0; j < grid.Height; j++)
			{
				bool occupied = gridPhysicsOccupation.IsCellOccupied(new Vector2I(i, j));
				gridCells[i, j] = new GridCell
				{
					Position = new Vector2I(i, j),
					GCost = int.MaxValue,
					HCost = 0
				};
			}
		}
	}
	public ReachableArea ComputeReachableArea(Vector2 origin, float maxTravelDistance)
	{
		Vector2I gridOrigin = grid.WorldToGrid(origin);
		HashSet<Vector2I> visited =[];
		Dictionary<Vector2I, float> distances = new();
		PriorityQueue<Vector2I, float> toVisit = new();
		toVisit.Enqueue(gridOrigin, 0);
		while (toVisit.TryDequeue(out Vector2I current, out float cellDistance))
		{
			if(!visited.Add(current)){
				continue;
			}
			visited.Add(gridOrigin);

			foreach (var neighbor in grid.GetAdjacentCells(current, true))
			{
				if (gridPhysicsOccupation.IsCellOccupied(gridCells[neighbor.X, neighbor.Y].Position))
					continue;

				float extraDistance = grid.GridToWorld(current, true).DistanceTo(grid.GridToWorld(neighbor, true));
				float totalDistance = cellDistance + extraDistance;
				if (totalDistance > maxTravelDistance)
					continue;
				if (!distances.TryAdd(neighbor, totalDistance)){
					if (totalDistance < distances[neighbor]){
						distances[neighbor] = totalDistance;
					}
				}
				if (visited.Contains(neighbor))
					continue;

				toVisit.Enqueue(neighbor, totalDistance);
			}
		}
		return new ReachableArea(visited, grid, distances);

	}

	public class ReachableArea(
		HashSet<Vector2I> reachableCells,
		GridDefinition grid,
		Dictionary<Vector2I, float> cellDistances)
	{
		public bool IsReachable(Vector2 worldPosition)
		{
			Vector2I gridPos = grid.WorldToGrid(worldPosition);
			return reachableCells.Contains(gridPos);
		}
		public Vector2[] GetAllReachablePositions()
		{
			return reachableCells.Select(cell => grid.GridToWorld(cell, true)).ToArray();
		}
		public float GetDistanceTo(Vector2 worldPosition)
		{
			Vector2I gridPos = grid.WorldToGrid(worldPosition);
			return cellDistances.GetValueOrDefault(gridPos, float.MaxValue);
		}

	}

	public Vector2[] GetPathBetween(Vector2 worldSpaceStart, Vector2 worldSpaceEnd)
	{
		Vector2I gridStart = grid.WorldToGrid(worldSpaceStart);
		Vector2I gridEnd = grid.WorldToGrid(worldSpaceEnd);
		Vector2I[] path = ComputePath(gridStart, gridEnd);
		if (path.Length == 0){
			return[];
		}
		Vector2[] worldPath = path.Select((pos) => grid.GridToWorld(pos,true)).ToArray();
		worldPath[^1] = worldSpaceEnd;
		return worldPath;
	}


	Vector2I[] ComputePath(Vector2I start, Vector2I target)
	{
		if (start == target)
			return [target];

		for (int i = 0; i < grid.Width; i++)
		{
			for (int j = 0; j < grid.Height; j++)
			{
				gridCells[i, j].GCost = int.MaxValue;
				gridCells[i, j].Parent = null;
				gridCells[i, j].HCost = 0;
			}
		}

		PriorityQueue<Vector2I, int> openSet = new();
		HashSet<Vector2I> closedSet = [];

		gridCells[start.X, start.Y].GCost = 0;
		openSet.Enqueue(start, 0);

		while (openSet.TryDequeue(out Vector2I current, out _))
		{
			if (current == target)
				return ReconstructPath(start, target);

			closedSet.Add(current);

			foreach (var neighbor in grid.GetAdjacentCells(current, true))
			{
				if (gridPhysicsOccupation.IsCellOccupied(gridCells[neighbor.X, neighbor.Y].Position))
					continue;
				if (closedSet.Contains(neighbor))
					continue;

				int tentativeG = gridCells[current.X, current.Y].GCost + gridCells[current.X, current.Y].GetDistance(neighbor);

				if (tentativeG < gridCells[neighbor.X, neighbor.Y].GCost)
				{
					gridCells[neighbor.X, neighbor.Y].GCost = tentativeG;
					gridCells[neighbor.X, neighbor.Y].Parent = current;

					int h = (int)current.DistanceTo(target);
					int f = tentativeG + h;

					openSet.Enqueue(neighbor, f);
				}
			}
		}

		return [];
	}


	private Vector2I[] ReconstructPath(Vector2I start, Vector2I end)
	{
		List<Vector2I> path =[];
		Vector2I current = end;

		while (current != start){
			path.Add(current);
			if (gridCells[current.X, current.Y].Parent == null)
				break;
			Vector2I parent = (Vector2I)gridCells[current.X, current.Y].Parent;
			current = parent;
		}

		path.Add(start);
		path.Reverse();

		return [.. path];
	}

	// [Export] Vector2 posA;
	// [Export] Vector2 posB;

    public override void _Process(double delta)
    {
		// var startCell = grid.WorldToGrid(posA);
		// var targetCell = grid.WorldToGrid(posB);
		// var path = ComputePath(startCell, targetCell);
		//
		// foreach (var pos in path)
		// {
		// 	grid.DrawTile(pos, Colors.Green);
		// }

    }
	public struct GridCell
	{
		public Vector2I Position;
		public int GCost;
		public int HCost;
		public readonly int FCost => GCost + HCost;
		public Vector2I? Parent;

		public readonly int GetDistance(Vector2I target)
		{
			int moveCost = (target.X != Position.X && target.Y != Position.Y) ? 14 : 10;
			return moveCost;
		}
	}
}

