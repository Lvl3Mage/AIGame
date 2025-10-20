using Godot;

namespace Game.Common.AgentControl.Strategies;

public class PlayerLostEvent
{
	public Vector2 PlayerPosition { get; init; }
	public Vector2 PlayerDirection { get; init; }
	
}