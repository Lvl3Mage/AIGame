using System;
using System.Threading;
using System.Threading.Tasks;
using Godot;

namespace Game.Common.Utility;

public static class MathUtility
{
	public static float ComputeLerpWeight(float smoothingSpeed, float delta)
	{
		float weight = 1f - Mathf.Exp(-smoothingSpeed * delta);
		return weight;
	}

	/// <summary>
    /// Smoothly interpolates a <see cref="float"/> value over a specified duration,
    /// using an optional easing <see cref="Curve"/> and supporting optional cancellation.
    /// </summary>
    /// <param name="getter">
    /// A delegate that retrieves the current value to interpolate from.
    /// Typically a property getter, e.g. <c>() => ZoomLevel</c>.
    /// </param>
    /// <param name="setter">
    /// A delegate that assigns the interpolated value.
    /// Typically a property setter, e.g. <c>v => ZoomLevel = v</c>.
    /// </param>
    /// <param name="targetValue">
    /// The destination value to interpolate toward.
    /// </param>
    /// <param name="time">
    /// Total duration of the interpolation, in seconds.
    /// If set to 0 or less, the value is applied instantly.
    /// </param>
    /// <param name="curve">
    /// Optional easing curve defining how the interpolation progresses over time.
    /// If <c>null</c>, a linear curve is used by default.
    /// </param>
    /// <param name="token">
    /// Optional <see cref="CancellationToken"/> that allows the interpolation
    /// to be canceled externally at any point.
    /// </param>
    /// <returns>
    /// A task that completes once the interpolation has finished or has been canceled.
    /// </returns>
    public static async Task LerpAsync(Func<float> getter, Action<float> setter, float targetValue, float time, CancellationToken token = default, Curve curve = default)
    {
        if (time <= 0f)
        {
            setter(targetValue);
            return;
        }

        if (curve == null)
        {
            curve = new Curve();
            curve.AddPoint(new Vector2(0, 0));
            curve.AddPoint(new Vector2(1, 1));
        }

        float startValue = getter();
        float elapsed = 0f;
        var tree = Engine.GetMainLoop() as SceneTree;

        while (elapsed < time)
        {
            if (token.IsCancellationRequested)
                return;

            elapsed += (float)tree.Root.GetProcessDeltaTime();
            float t = Mathf.Clamp(elapsed / time, 0f, 1f);
            float eased = curve.Sample(t);
            setter(Mathf.Lerp(startValue, targetValue, eased));

            await tree.ToSignal(tree, SceneTree.SignalName.ProcessFrame);
        }

        setter(targetValue);
    }
}