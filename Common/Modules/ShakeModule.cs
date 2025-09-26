using System;
using Godot;

namespace Game.Common.Modules;

/// <summary> ScreenShakeModule provides a screen shake effect for a parent Node2D. </summary>
[GlobalClass]
public partial class ShakeModule : Node2D
{
    [Export] public float ShakeDuration { get; set; } = 0.5f; // Seconds
    [Export] public float ShakeMagnitude { get; set; } = 10f; // Maximum offset in pixels
    [Export] public float Smoothness { get; set; } = 25f; // Higher = smoother movement

    Node2D ParentNode => GetParent<Node2D>();
    Vector2 originalPosition;
    float shakeTime = 0f;
    readonly Random rnd = new();

    public override void _Process(double delta)
    {
        if (ParentNode == null) return;

        if (shakeTime > 0f)
        {
            shakeTime -= (float)delta;

            // Generate a random offset in both axes
            Vector2 offset = new(
                (float)(rnd.NextDouble() * 2 - 1) * ShakeMagnitude,
                (float)(rnd.NextDouble() * 2 - 1) * ShakeMagnitude
            );

            // Smoothly interpolate to the new offset
            ParentNode.Position = ParentNode.Position.Lerp(originalPosition + offset, Smoothness * (float)delta);

            // Reset position when finished
            if (shakeTime <= 0f)
                ParentNode.Position = originalPosition;
        }
    }

    /// <summary> Starts a shake effect on the assigned TargetNode. </summary>
    public void Shake()
    {
        if (ParentNode == null) return;

        originalPosition = ParentNode.Position;
        shakeTime = ShakeDuration;
    }
}