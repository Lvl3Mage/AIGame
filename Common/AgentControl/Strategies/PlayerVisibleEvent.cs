using Godot;

namespace Game.Common.AgentControl.Strategies;

public class PlayerVisibleEvent : IAgentEvent
{
	public Vector2 PlayerPosition { get; init; }
	public Vector2 PlayerDirection { get; init; }
	public EventInfo Info { get; init; }
	public float Strength { get; init; }

	public Vector2 Origin { get; init; }
}