using Godot;

namespace Game.Common;

public partial class PlayerController : CharacterBody2D
{
    [Export] Modules.TopdownMovementModule movementModule;

    public override void _Process(double delta)
    {
        movementModule.InputRight = Input.IsActionPressed("moveRight");
        movementModule.InputLeft = Input.IsActionPressed("moveLeft");
        movementModule.InputUp = Input.IsActionPressed("moveUp");
        movementModule.InputDown = Input.IsActionPressed("moveDown");
    }
}