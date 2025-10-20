using Game.Common.Utility;
using Godot;

namespace Game.Global;

[GlobalClass]
public partial class SquishModule : Node2D
{
	[Export] public float SquashFactor { get; set; } = 0.3f; // Factor by which to squash/stretch
	[Export] public float MinValueThreshold { get; set; } = -180f; // Value at which max stretch occurs
	[Export] public float MaxValueThreshold { get; set; } = 300f;
	[Export] public float DeadZone { get; set; } = 5f; // Value below which no scaling occurs
	[Export] public float ImpactFactor { get; set; } = 0.6f; // Factor by which to scale on impact
	[Export] public float ImpactDecay { get; set; } = 6f; // Speed at which impact effect fades out

	public bool FlipH { get; set; } = false;
	public bool FlipV { get; set; } = false;

	float lastValue = 0f;
	Vector2 impactWeight = Vector2.Zero;

	public override void _Process(double delta)
	{
		UpdateMirroring();
	}

	public void UpdateDynamicScaling(float value)
	{
		float deltaVy = value - lastValue;
		float dt = (float)GetProcessDeltaTime();

		// Calculate proportional impact strength based on value change
		float normalizedImpact = Mathf.Abs(deltaVy) / MaxValueThreshold;

		if (normalizedImpact > 0.1f) // Ignore very small changes
			impactWeight.X = Mathf.Clamp(normalizedImpact, 0f, 1f);

		lastValue = value;
		float weight = MathUtility.ComputeLerpWeight(ImpactDecay, dt);

		impactWeight.X = Mathf.Lerp(impactWeight.X, 0f, weight);
		impactWeight.Y = Mathf.Lerp(impactWeight.Y, 0f, weight);

		if (Mathf.Abs(value) < DeadZone) ApplyScaling(0f);
		else ApplyScaling(CalculateScaleFactor(value));
	}

	/// <summary> Stretches the sprite based on an impact weight. </summary>
	public void ApplyImpact(float weight, bool vertical = false)
	{
		if (vertical)
			impactWeight.Y = Mathf.Clamp(weight, 0f, 1f); // Vertical: shrink X, grow Y
		else
			impactWeight.X = Mathf.Clamp(weight, 0f, 1f); // Classic: grow X, shrink Y
	}

	/// <summary> Calculates a scale factor based on a given value. </summary>
	/// <param name="value">The value which will affect scale.</param>
	/// <returns>A value between 0 and 1 representing the scale factor.</returns>
	float CalculateScaleFactor(float value)
	{
		if (value < 0) // Rising
		{
			float clamped = Mathf.Clamp(value, MinValueThreshold, -DeadZone);
			float t = (clamped - (-DeadZone)) / (MinValueThreshold - (-DeadZone));
			return Mathf.Clamp(t, 0f, 1f);
		}
		else // Falling
		{
			float clamped = Mathf.Clamp(value, DeadZone, MaxValueThreshold);
			float t = (clamped - DeadZone) / (MaxValueThreshold - DeadZone);
			return Mathf.Clamp(t, 0f, 1f);
		}
	}

	/// <summary> Stretches the sprite based on the given weight. </summary>
	/// <param name="weight">A value between 0 and 1 representing the scaling.</param>
	void ApplyScaling(float weight)
	{
		Vector2 maxStretchScale = new(1f - SquashFactor, 1f + SquashFactor);
		Vector2 baseScale = Vector2.One.Lerp(maxStretchScale, weight);

		Vector2 impactScaleHorizontal = new(
			1f + ImpactFactor * impactWeight.X,
			1f - ImpactFactor * impactWeight.X);

		Vector2 impactScaleVertical = new(
			1f - ImpactFactor * impactWeight.Y,
			1f + ImpactFactor * impactWeight.Y);

		Vector2 finalScale = baseScale * impactScaleHorizontal * impactScaleVertical;

		Scale = finalScale;
	}

	void UpdateMirroring()
	{
		Scale = new Vector2(
			Mathf.Abs(Scale.X) * (FlipH ? -1 : 1),
			Mathf.Abs(Scale.Y) * (FlipV ? -1 : 1)
		);
	}
}
