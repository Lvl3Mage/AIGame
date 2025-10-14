using Game.Common;
using Godot;
using System;

namespace Game;
public partial class Key : Node2D
{
	[Export] public Area2D InteractionArea { get; set; }
	[Export] float lerpFactor = 0.2f;
	[Export] float yOffset = -20f;
	public bool Collected { get; set; } = false;

	public override void _Ready()
	{
		InteractionArea.BodyEntered += OnBodyEntered;
		InteractionArea.AreaEntered += OnAreaEntered;
	}

	private void OnAreaEntered(Area2D area)
	{
		if (area.GetParent() is Door door)
		{
			if (door.NeededKey == this && Collected)
			{
				door.Open();
				QueueFree();
			}
		}
	}

    private void OnBodyEntered(Node2D body)
	{
		if (body == GameManager.Instance.Player) Collected = true;
	}

	public override void _Process(double delta)
	{
		if (Collected)
			Position = Position.Lerp(GameManager.Instance.Player.Position + yOffset * Vector2.Down, lerpFactor);
	}
}
