using Godot;
using System;

[GlobalClass]
public partial class DynamicCamera : Camera2D
{
	[Export] public float MouseInfluence = 0.1f;
	[Export] public float SmoothSpeed = 3f;
	[Export] Node2D followTarget;
	private Vector2 targetOffset = Vector2.Zero;

	public override void _Ready()
	{
		GlobalPosition = followTarget.GlobalPosition;
	}
	
	public override void _Process(double delta)
	{
		Vector2 mousePos = GetViewport().GetMousePosition();
		Vector2 screenCenter = GetViewport().GetVisibleRect().Size / 2;

		Vector2 mouseOffset = (mousePos - screenCenter) * MouseInfluence;

		targetOffset = targetOffset.Lerp(mouseOffset + followTarget.GlobalPosition, (float)(SmoothSpeed * delta));



		GlobalPosition = targetOffset;
	}
}
