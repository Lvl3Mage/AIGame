using Godot;

namespace Game.Common.Modules;

/// <summary>
/// A full-screen flash effect with fade in, hold, and fade out.
/// Attach this node as a ColorRect covering the screen.
/// </summary>
[GlobalClass]
public partial class ScreenFlashModule : ColorRect
{
    Tween _tween;

    public override void _Ready()
    {
        Visible = false;
        Modulate = new Color(1f, 1f, 1f, 0f); // default white, transparent
    }

    /// <summary>
    /// Plays a flash effect on the screen.
    /// </summary>
    /// <param name="fadeIn">Seconds to fade in.</param>
    /// <param name="duration">Seconds to stay visible at full alpha.</param>
    /// <param name="fadeOut">Seconds to fade out.</param>
    /// <param name="alpha">Target alpha (intensity), default 1.0.</param>
    /// <param name="color">Flash color, default is white.</param>
    public async void ScreenFlash(float fadeIn, float duration, float fadeOut, float alpha = 0.5f, Color? color = null)
    {
        // Cancel any existing tween
        _tween?.Kill();

        Visible = true;
        Color targetColor = color ?? Colors.White;

        // Set starting color with 0 alpha
        Modulate = new Color(targetColor.R, targetColor.G, targetColor.B, 0f);

        _tween = CreateTween();

        // Fade In
        _tween.TweenProperty(this, "modulate:a", alpha, fadeIn);
        await ToSignal(_tween, Tween.SignalName.Finished);

        // Hold
        await ToSignal(GetTree().CreateTimer(duration, processAlways: true, processInPhysics: false, ignoreTimeScale: true), SceneTreeTimer.SignalName.Timeout);

        // Fade Out
        _tween = CreateTween();
        _tween.TweenProperty(this, "modulate:a", 0f, fadeOut);
        await ToSignal(_tween, Tween.SignalName.Finished);

        Visible = false;
    }
}