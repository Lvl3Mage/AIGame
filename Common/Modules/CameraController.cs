using Godot;

namespace Game.Common.Modules;

public partial class CameraController : Camera2D
{
	[Export] Node2D target;
	[Export] float smoothingSpeed = 5f;

	public override void _Process(double delta)
	{
		if (target == null) return;

		float weight = 1f - Mathf.Exp(-smoothingSpeed * (float)delta);
		GlobalPosition = GlobalPosition.Lerp(target.GlobalPosition, weight);
	}

}