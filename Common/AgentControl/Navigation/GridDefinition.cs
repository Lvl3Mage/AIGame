using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Game.Common.AgentControl.Navigation;

[Tool][GlobalClass]
public partial class GridDefinition : Node2D
{
	[Export] int width = 32;
	[Export] int height = 32;
	[Export] float cellSize = 64f;
	public int Height => height;
	public int Width => width;
	public float CellSize => cellSize;

	public Transform2D GetGridTransform()
	{
		return GlobalTransform;
	}

	/// <summary> Checks if a grid cell is within the grid bounds. </summary>
	public bool IsInsideGrid(Vector2I cell)
	{
		return cell.X >= 0 && cell.Y >= 0 && cell.X < width && cell.Y < height;
	}

	/// <summary> Transforms a world position to a grid cell. </summary>
	public Vector2I WorldToGrid(Vector2 position)
	{
		Vector2 local = ToLocal(position);

		return new Vector2I(
			Mathf.FloorToInt(local.X / cellSize),
			Mathf.FloorToInt(local.Y / cellSize)
		);
	}

	/// <summary> Transforms a grid cell to a world position. </summary>
	public Vector2 GridToWorld(Vector2I cell)
	{
		Vector2 local = new(cell.X * cellSize, cell.Y * cellSize);
		return ToGlobal(local);
	}

	public IEnumerable<Vector2I> GridCells()
	{
		for (int x = 0; x < width; x++){
			for (int y = 0; y < height; y++){
				yield return new Vector2I(x, y);
			}
		}
	}
	/// <summary> Gets the adjacent cells of a given cell. </summary>
	public IList<Vector2I> GetAdjacentCells(Vector2I cell, bool includeDiagonals = false)
	{
		Vector2I[] directions = includeDiagonals
			? [
				new(1, 0), new(-1, 0), new(0, 1), new(0, -1),
				new(1, 1), new(-1, -1), new(1, -1), new(-1, 1)
			]
			: [new(1, 0), new(-1, 0), new(0, 1), new(0, -1)];

		return directions.Select(dir => cell + dir).Where(IsInsideGrid).ToList();
	}

	[Export] bool debugDraw = true;

	public override void _Draw()
	{
		if (!debugDraw) return;
		for (int y = 0; y <= height; y++){
			DrawLine(new Vector2(0, y * cellSize), new Vector2(width * cellSize, y * cellSize), Colors.Gray);
		}

		for (int x = 0; x <= width; x++){
			DrawLine(new Vector2(x * cellSize, 0), new Vector2(x * cellSize, width * cellSize), Colors.Gray);
		}
	}

	public override void _Process(double delta)
	{
		if (debugDraw){
			QueueRedraw();
		}
	}
}