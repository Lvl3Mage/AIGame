using Godot;

namespace Game.Common;

public partial class PlayerController : Node2D
{
    [Export] Modules.TopdownMovementModule movementModule;

    public override void _Process(double delta)
    {
        movementModule.InputRight = Input.IsActionJustPressed("moveRight");
        movementModule.InputLeft = Input.IsActionJustPressed("moveLeft");
        movementModule.InputUp = Input.IsActionJustPressed("moveUp");
        movementModule.InputDown = Input.IsActionJustPressed("moveDown");
    }
}