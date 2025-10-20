using System;
using Game.Common.Utility;
using Godot;

namespace Game.Common.Modules;

[GlobalClass]
public partial class CharacterAnimationModule : Node2D
{
    [Export(PropertyHint.Range, "0.0, 50.0")]
    public float AmplitudeMultiplier { get; set; } = 16.0f;
    [Export(PropertyHint.Range, "0.1, 50.0")]
    public float Frequency { get; set; } = 13.0f;
    [Export] float maxVel = 100f;
    [Export] float minVel = 0f;
    float time = 0.0f;
    float velocityAttenuation = 0.0f;

    public override void _Process(double delta)
    {
        if (GetParent() is CharacterBody2D characterBody)
        {
            time += (float)delta;
            float currentVelocity = characterBody.Velocity.Length();
            float clampedVel = Mathf.Clamp(currentVelocity, minVel, maxVel);
            float currentAttenuation = Mathf.InverseLerp(minVel, maxVel, clampedVel);
            velocityAttenuation = Mathf.Lerp(velocityAttenuation, currentAttenuation, MathUtility.ComputeLerpWeight(10f, (float)delta));
            float offsetY = Mathf.Abs(Mathf.Sin(time * Frequency)) * AmplitudeMultiplier * velocityAttenuation;
            Position = new Vector2(0, -offsetY);
        }
    }
}