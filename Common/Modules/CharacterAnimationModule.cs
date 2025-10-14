using System;
using Game.Common.Utility;
using Godot;

namespace Game.Common.Modules;

[GlobalClass]
public partial class CharacterAnimationModule : Node2D
{

    //sety = abs(sin(tiempo * frecuencia)) * amplitud

    [Export(PropertyHint.Range, "0.0, 50.0")]
    public float AmplitudeMultiplier { get; set; } = 30.0f;
    [Export(PropertyHint.Range, "0.1, 50.0")]
    public float Frequency { get; set; } = 8.0f;
    [Export] float maxVel = 100f;
    [Export] float minVel = 20f;
    float time = 0.0f;
    float velocityAttenuation = 0.0f;


    public override void _Process(double delta)
    {
        //float amplitude = characterBody.Velocity * AmplitudeMultiplier;
        if (GetParent() is CharacterBody2D characterBody)
        {
            time += (float)delta;
            float currentVelocity = characterBody.Velocity.Length();
            float clampedVel = Mathf.Clamp(currentVelocity, minVel, maxVel);
            float currentAttenuation = Mathf.InverseLerp(minVel, maxVel, clampedVel);
            velocityAttenuation = Mathf.Lerp(velocityAttenuation, currentAttenuation, MathUtility.ComputeLerpWeight(10f, (float)delta));
            float offsetY = Mathf.Abs(Mathf.Sin(time * Frequency)) * AmplitudeMultiplier * velocityAttenuation;
            DebugDraw2D.SetText("offsetY player", offsetY.ToString());
            Position = new Vector2(0, -offsetY);
        }
    }
}