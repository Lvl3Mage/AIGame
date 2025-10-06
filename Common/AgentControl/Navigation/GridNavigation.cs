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

	List<Vector2I> currentPath;
	public override void _Ready()
	{
		gridCells = new GridCell[grid.Width, grid.Height];

		for (int i = 0; i < grid.Width; i++)
		{
			for (int j = 0; j < grid.Height; j++)
			{
				gridCells[i, j] = new GridCell
				{
					Position = new Vector2I(i, j),
					Blocked = false,
					GCost = int.MaxValue,
					HCost = 0
				};
			}
		}

	}

	public float CellSize { get; private set; } = 64f;

	public void BlockCell(Vector2I cell)
	{
		if (!grid.IsInsideGrid(cell))
			return;

		gridCells[cell.X, cell.Y].Blocked = true;
	}

	public void UnblockCell(Vector2I cell)
	{
		if (!grid.IsInsideGrid(cell))
			return;

		gridCells[cell.X, cell.Y].Blocked = true;
	}

	public Vector2I[] FindPath(Vector2I start, Vector2I target)
	{
		if (start == target) return [start];
		CalculateHeuristic(target);
		var toSearch = new List<Vector2I>() { start };
		var processed = new List<Vector2I>();

		while (toSearch.Count != 0)
		{
			Vector2I current = toSearch[0];
			foreach (var t in toSearch)
				if (gridCells[t.X, t.Y].FCost < gridCells[current.X, current.Y].FCost
					|| gridCells[t.X, t.Y].FCost == gridCells[current.X, current.Y].FCost
					&& gridCells[t.X, t.Y].HCost < gridCells[current.X, current.Y].HCost)
					current = t;

			if (current == target)
				return ReconstructPath(start, target);


			processed.Add(current);
			toSearch.Remove(current);

			foreach (var neighbor in grid.GetAdjacentCells(current, true))
			{
				if (gridCells[neighbor.X, neighbor.Y].Blocked) continue;
				if (processed.Contains(neighbor)) continue;
				var inSearch = toSearch.Contains(neighbor);

				var costToNeighbor = gridCells[current.X, current.Y].GCost + gridCells[current.X, current.Y].GetDistance(neighbor);

				if (!inSearch || costToNeighbor < gridCells[neighbor.X, neighbor.Y].GCost)
				{
					gridCells[neighbor.X, neighbor.Y].GCost = costToNeighbor;
					gridCells[neighbor.X, neighbor.Y].Parent = current;
					if (!inSearch)
					{
						toSearch.Add(neighbor);
					}
				}
			}

		}
		return [];
	}

	private Vector2I[] ReconstructPath(Vector2I start, Vector2I end)
	{
		List<Vector2I> path = [];
		Vector2I current = end;

		while (current != start)
		{
			path.Add(current);
			if (gridCells[current.X, current.Y].Parent == null)
				break;
			Vector2I parent = (Vector2I)gridCells[current.X, current.Y].Parent;
			current = parent;
		}

		path.Add(start);
		path.Reverse();
		currentPath = path;
		QueueRedraw();
		return [.. path];
	}




	public void CalculateHeuristic(Vector2I target)
	{
		for (int i = 0; i < grid.Width; i++)
		{
			for (int j = 0; j < grid.Height; j++)
			{
				var cell = gridCells[i, j];
				int dx = Mathf.Abs(cell.Position.X - target.X);
				int dy = Mathf.Abs(cell.Position.Y - target.Y);
				cell.HCost = (int)(((Mathf.Sqrt(2) - 1) * Mathf.Min(dx, dy) + Mathf.Max(dx, dy)) * 10);

				gridCells[i, j] = cell;
			}
		}
	}
	
	public override void _Draw()
	{
		if (currentPath == null)
			return;

		foreach (var pos in currentPath)
		{
			Vector2 worldPos = new(pos.X * CellSize, pos.Y * CellSize);
			DrawRect(
				new Rect2(worldPos, new Vector2(CellSize, CellSize)),
				new Color(0, 1, 0, 0.4f), 
				filled: true
			);
			DrawRect(
				new Rect2(worldPos, new Vector2(CellSize, CellSize)),
				new Color(0, 1, 0),
				filled: false
			);
		}
	}


}

public struct GridCell
{
	public Vector2I Position;
	public int GCost;
	public int HCost;
	public readonly int FCost => GCost + HCost;
	public bool Blocked;
	public Vector2I? Parent;

	public int GetDistance(Vector2I target)
	{
		int moveCost = (target.X != Position.X && target.Y != Position.Y) ? 14 : 10;
		return moveCost;
	}

}