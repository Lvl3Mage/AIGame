using Godot;

namespace Game.Common.AgentControl.Navigation;

public partial class GridPhysicsOccupation : Node2D, IGridOccupationProvider
{
	[Export] GridDefinition grid;

	bool[,] collisionMap;

	public override void _Ready()
	{
		collisionMap = new bool[grid.Width, grid.Height];
	}
	void UpdateCollisionMap()
	{
		collisionMap = new bool[grid.Width, grid.Height];
		foreach (Vector2I gridCell in grid.GridPositions()){
			
		}
	}
	//Todo scan the world for colliders and mark the cells as occupied

	public bool IsCellOccupied(Vector2I cell)
	{
		throw new System.NotImplementedException();
	}
}