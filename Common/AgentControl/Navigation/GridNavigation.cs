using Godot;

namespace Game.Common.AgentControl.Navigation;

//Todo rework this to use the IGridOccupationProvider interface
[GlobalClass]
public partial class GridNavigation : Node2D
{
	bool[,] blockedCells;
	[Export] GridDefinition grid;
	public override void _Ready()
	{
		blockedCells = new bool[grid.Width, grid.Height];
	}

	public float CellSize { get; private set; } = 64f;

	public void BlockCell(Vector2I cell)
	{
		if (grid.IsInsideGrid(cell))
		{
			return;
		}

		blockedCells[cell.X, cell.Y] = true;
	}

	public void UnblockCell(Vector2I cell)
	{
		if (grid.IsInsideGrid(cell))
		{
			return;
		}

		blockedCells[cell.X, cell.Y] = false;
	}



	public Vector2I[] FindPath(Vector2I start, Vector2I target)
	{
		//Todo Alonso do a* here for now (maybe we change a little later idk)
		return null;
	}
}