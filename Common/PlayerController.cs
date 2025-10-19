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

    bool pressRight, pressLeft, pressUp, pressDown, lockMovement;

    public override void _Ready()
    {
        lockMovement = false;
    }

    public override void _Process(double delta)
    {
        if (lockMovement)
            pressRight = pressLeft = pressUp = pressDown = false;
        else
            MapActions();

        movementModule.InputRight = pressRight;
        movementModule.InputLeft = pressLeft;
        movementModule.InputUp = pressUp;
        movementModule.InputDown = pressDown;

        if (pressRight) sprite.FlipH = false;
        else if (pressLeft) sprite.FlipH = true;
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

        lockMovement = true;
        _ = MathUtility.LerpAsync(getRedColor, setRedColor, 255f, dieAnimationTime);
        await MathUtility.LerpAsync(getRotation, setRotation, 90f, dieAnimationTime);

        GameManager.Instance.RestartGame();
    }
}