using Godot;

namespace Game.Common.Modules;

/// <summary>
/// FloatModule gives a floating, oscillating motion to its parent (Node2D or Control).
/// </summary>
[GlobalClass]
public partial class FloatModule : Node
{
	[Export] public float MaxOffset { get; set; } = 50f; 
	[Export] public float SpeedX { get; set; } = 1f;      
	[Export] public float SpeedY { get; set; } = 1f;    
	[Export] public float AmplitudeVariance { get; set; } = 0.5f;

	Node parentNode;
	Vector2 initialPosition;
	float time = 0f;
	bool isControl = false;

	public override void _Ready()
	{
		parentNode = GetParent();

		if (parentNode is Node2D node2D)
		{
			initialPosition = node2D.Position;
			isControl = false;
		}
		else if (parentNode is Control control)
		{
			initialPosition = control.Position;
			isControl = true;
		}
		else
		{
			GD.PushError("FloatModule: Parent must be Node2D or Control.");
			QueueFree();
		}
	}

	public override void _Process(double delta)
	{
		if (parentNode == null) return;

		time += (float)delta;

		float offsetX = Mathf.Sin(time * SpeedX) * MaxOffset * (1f + AmplitudeVariance * Mathf.Sin(time * 1.3f));
		float offsetY = Mathf.Sin(time * SpeedY + 1.5f) * MaxOffset * (1f + AmplitudeVariance * Mathf.Cos(time * 1.1f));

		Vector2 newPos = initialPosition + new Vector2(offsetX, offsetY);

		if (isControl && parentNode is Control control)
			control.Position = newPos;
		else if (!isControl && parentNode is Node2D node2D)
			node2D.Position = newPos;
	}
}