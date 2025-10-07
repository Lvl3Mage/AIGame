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
	[Export] CharacterBody2D enemy;
	[Export] CharacterBody2D player;
	[Export] GridDefinition grid;

	GridCell[,] gridCells;

	List<Vector2I> currentPath;

	public override async void _Ready()
	{
		gridCells = new GridCell[grid.Width, grid.Height];
		GD.Print(grid.Width + " " + grid.Height);

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

		await ToSignal(GetTree(), "process_frame");
		var startCell = grid.WorldToGrid(enemy.GlobalPosition);
		var targetCell = grid.WorldToGrid(player.GlobalPosition);
		FindPath(startCell, targetCell);

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

		gridCells[cell.X, cell.Y].Blocked = false;
	}

	//todo Alonso, can't look into it right now but this looks pretty wrong.
	// You only need to calculate the heuristic once a cell is reached to evaluate it, otherwise you'll be calculating a lot of unnecessary heuristics
	// The heuristic can just be euclidean distance (Aka straight line distance)
	// Also, you need to look at the nodes in order of lowest F cost, (sum of G and H cost) so use a priority queue

	// I recommend you try implementing this as Dijkstra first and then add the heuristic on top of it since the 2 algorithms are basically the same
	public Vector2I[] FindPath(Vector2I start, Vector2I target)
	{
		if (start == target)
			return [start];

		for (int i = 0; i < grid.Width; i++)
		{
			for (int j = 0; j < grid.Height; j++)
			{
				gridCells[i, j].GCost = int.MaxValue;
				gridCells[i, j].Parent = null;
			}
		}

		PriorityQueue<Vector2I, int> openSet = new();
		HashSet<Vector2I> closedSet = [];

		GD.Print(start);
		gridCells[start.X, start.Y].GCost = 0;
		openSet.Enqueue(start, 0);

		while (openSet.TryDequeue(out Vector2I current, out _))
		{
			if (current == target)
				return ReconstructPath(start, target);

			closedSet.Add(current);

			foreach (var neighbor in grid.GetAdjacentCells(current, true))
			{
				if (gridCells[neighbor.X, neighbor.Y].Blocked)
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
		currentPath = path;
		QueueRedraw();

		return [.. path];
	}

	public override void _Draw()
	{
		GD.Print("He pasado por aqui");
		if (currentPath == null)
			return;

		foreach (var pos in currentPath){
			Vector2 worldPos = grid.ToGlobal(new Vector2(pos.X * CellSize, pos.Y * CellSize));
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

	public readonly int GetDistance(Vector2I target)
	{
		int moveCost = (target.X != Position.X && target.Y != Position.Y) ? 14 : 10;
		return moveCost;
	}
}