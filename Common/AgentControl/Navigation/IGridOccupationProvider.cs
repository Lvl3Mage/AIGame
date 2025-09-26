using Godot;

namespace Game.Common.AgentControl.Navigation;

public interface IGridOccupationProvider
{
	public bool IsCellOccupied(Vector2I cell);
}