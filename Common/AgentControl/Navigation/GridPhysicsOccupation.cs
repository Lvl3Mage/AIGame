using Godot;

namespace Game.Common.AgentControl.Navigation;

public partial class GridPhysicsOccupation : Node2D, IGridOccupationProvider
{
	[Export] GridDefinition grid;
	//Todo scan the world for colliders and mark the cells as occupied

	public bool IsCellOccupied(Vector2I cell)
	{
		throw new System.NotImplementedException();
	}
}