using Game.Common;
using Game.Common.Utility;
using Godot;
using System;
using System.Diagnostics;

namespace Game;
public partial class DiamondKey : Node2D
{
	[Export] public Area2D CollectionArea { get; set; }
	[Export] public Area2D InsertKeyArea { get; set; }
	[Export] float lerpFactor = 0.2f;
	[Export] float yOffset = -20f;
	public bool Collected { get; set; } = false;

	bool collided = false;

	public override void _Ready()
	{
		CollectionArea.BodyEntered += CheckPlayerCollection;
		InsertKeyArea.BodyEntered += CheckKeyInsertion;
	}

    private void CheckKeyInsertion(Node2D body) // Manage key insertion
	{
		if (body.GetParent() is Door door)
		{
			if (((door.NeededKey == this && door.NeededKey != null) || door.NeededKey == null) && Collected)
			{
				door.Open();
				QueueFree();
			}
		}
    }

    private void CheckPlayerCollection(Node2D body) // Manage player collection
	{
		if (body == GameManager.Instance.Player) Collected = true;
	}

	public override void _Process(double delta)
	{
		float dt = (float)delta;
		Vector2 targetPos = GameManager.Instance.Player.Position + yOffset * Vector2.Down;

		if (Collected)
			GlobalPosition = GlobalPosition.Lerp(targetPos, MathUtility.ComputeLerpWeight(lerpFactor, dt));
	}
}
