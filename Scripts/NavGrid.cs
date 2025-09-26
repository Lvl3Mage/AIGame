using System.Collections.Generic;
using Godot;

namespace Game.Scripts;

public partial class NavGrid : Node2D
{
	bool[,] blockedCells;
	[Export] int Rows = 10;
	[Export] int Columns = 10;
	public override void _Ready()
	{
		blockedCells = new bool[Columns, Rows];
	}

	public float CellSize { get; private set; } = 64f;

	public void BlockCell(Vector2I cell)
	{
		if (IsInsideGrid(cell)){
			return;
		}
		blockedCells[cell.X, cell.Y] = true;
	}

	public void UnblockCell(Vector2I cell)
	{
		if(IsInsideGrid(cell)){
			return;
		}
		blockedCells[cell.X, cell.Y] = false;
	}


	/// <summary> Checks if a grid cell is within the grid bounds. </summary>
	public bool IsInsideGrid(Vector2I cell)
	{
		return cell.X >= 0 && cell.Y >= 0 && cell.X < Columns && cell.Y < Rows;
	}

	/// <summary> Transforms a world position to a grid cell. </summary>
	public Vector2I WorldToGrid(Vector2 position)
	{
		Vector2 local = ToLocal(position);

		return new Vector2I(
			Mathf.FloorToInt(local.X / CellSize),
			Mathf.FloorToInt(local.Y / CellSize)
		);
	}

	/// <summary> Transforms a grid cell to a world position. </summary>
	public Vector2 GridToWorld(Vector2I cell)
	{
		Vector2 local = new(cell.X * CellSize, cell.Y * CellSize);
		return ToGlobal(local);
	}
	public Vector2I[] FindPath(Vector2I start, Vector2I target)
	{
		//Todo Alonso do a* here for now (maybe we change a little later idk)
		return null;
	}


}