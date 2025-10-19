using System;
using System.Threading.Tasks;
using Game.Common.Utility;
using Godot;

namespace Game.Common;

public partial class PlayerController : CharacterBody2D
{
    [Export] Modules.TopdownMovementModule movementModule;
    [Export] Sprite2D sprite;
    [Export] float dieAnimationTime = 0.5f;
    [Export] float shrinkSpeed = 200f;

    public bool LockMovement { get; set; }
    public bool hasWon { get; set; }

    bool pressRight, pressLeft, pressUp, pressDown;

    public override void _Ready()
    {
        LockMovement = false;
        hasWon = false;
    }

    public override void _Process(double delta)
    {
        float dt = (float)delta;

        if (LockMovement)
            pressRight = pressLeft = pressUp = pressDown = false;
        else
            MapActions();

        movementModule.InputRight = pressRight;
        movementModule.InputLeft = pressLeft;
        movementModule.InputUp = pressUp;
        movementModule.InputDown = pressDown;

        if (pressRight) sprite.FlipH = false;
        else if (pressLeft) sprite.FlipH = true;

        if (hasWon) // Funny
        {
            sprite.RotationDegrees += 2000f * dt;
            sprite.Scale.Lerp(Vector2.Zero, MathUtility.ComputeLerpWeight(shrinkSpeed, dt));
        }
    }

    void MapActions()
    {
        pressRight = Input.IsActionPressed("moveRight");
        pressLeft = Input.IsActionPressed("moveLeft");
        pressUp = Input.IsActionPressed("moveUp");
        pressDown = Input.IsActionPressed("moveDown");
    }

    public void Die() => _ = TriggerDieAnimation();

    async Task TriggerDieAnimation()
    {
        float getRotation() => sprite.RotationDegrees;
        void setRotation(float v) => sprite.RotationDegrees = v;

        float getRedColor() => sprite.SelfModulate.R;
        void setRedColor(float v) => sprite.SelfModulate = new Color(v, sprite.SelfModulate.G, sprite.SelfModulate.B, sprite.SelfModulate.A);

        LockMovement = true;
        _ = MathUtility.LerpAsync(getRedColor, setRedColor, 255f, dieAnimationTime);
        await MathUtility.LerpAsync(getRotation, setRotation, 90f, dieAnimationTime);

        GameManager.Instance.RestartGame();
    }
}