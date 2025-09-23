using Godot;
using System;

[GlobalClass]
public partial class TopdownMovementModule : Node2D
{
    [Export] public float MaxMoveSpeed = 500f;  // The maximum speed at which the object can move.
    [Export] public float AccelerationTime = 0.2f; // Time to reach max speed.
    [Export] public float DecelerationTime = 0.2f; // Time to decelerate to zero.

    public bool InputDown = false;
    public bool InputUp = false;
    public bool InputRight = false;
    public bool InputLeft = false;

    private CharacterBody2D charBody;

    public override void _Ready()
    {
        charBody = GetParent() as CharacterBody2D;

        if (charBody == null)
            GD.PushError("TopdownMovementModule's parent must be a CharacterBody2D.");
        else if (charBody.MotionMode == CharacterBody2D.MotionModeEnum.Grounded)
            GD.PushWarning("CharacterBody2D.MotionMode should be set to MOTION_MODE_FLOATING.");
    }

    public override void _Process(double delta)
    {
        float acceleration = MaxMoveSpeed / AccelerationTime * (float)delta;
        float deceleration = MaxMoveSpeed / DecelerationTime * (float)delta;

        Vector2 direction = new Vector2(
            GetAxis(InputRight, InputLeft),
            GetAxis(InputDown, InputUp)
        ).Normalized();

        Vector2 maxVelocity = MaxMoveSpeed * direction.Abs();

        charBody.Velocity = new Vector2(
            AxisMotion(charBody.Velocity.X, acceleration, deceleration, maxVelocity.X, InputRight, InputLeft),
            AxisMotion(charBody.Velocity.Y, acceleration, deceleration, maxVelocity.Y, InputDown, InputUp)
        );

        charBody.MoveAndSlide();
    }

    /// <summary> Returns 1 if positive input is pressed, -1 if negative input is pressed, or 0 if neither.</summary>
    private static int GetAxis(bool positiveInput = false, bool negativeInput = false)
    {
        return (positiveInput ? 1 : 0) - (negativeInput ? 1 : 0);
    }

    /// <summary> Moves currentVelocity toward targetSpeed using acceleration or deceleration,
    /// depending on the inputs and velocity direction.</summary>
    private float AxisMotion(float currentVelocity, float acc, float dec, float targetSpeed, bool inputPos, bool inputNeg)
    {
        float vel = currentVelocity;
        int direction = GetAxis(inputPos, inputNeg);

        bool goingBackwards = direction != Mathf.Sign(vel) && vel != 0f;
        bool overspeed = Mathf.Abs(vel) > MaxMoveSpeed;
        bool canAccelerate = direction != 0;
        bool canDecelerate = direction == 0 || overspeed || goingBackwards;

        if (canAccelerate)
            vel = Mathf.MoveToward(vel, targetSpeed * direction, acc);

        if (canDecelerate)
            vel = Mathf.MoveToward(vel, 0f, dec);

        return vel;
    }
}
