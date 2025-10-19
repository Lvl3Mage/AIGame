using Godot;

namespace Game.Common;

public partial class PlayerController : CharacterBody2D
{
    [Export] Modules.TopdownMovementModule movementModule;
    [Export] Sprite2D sprite;

    bool pressRight, pressLeft, pressUp, pressDown;

    public override void _Process(double delta)
    {
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
}