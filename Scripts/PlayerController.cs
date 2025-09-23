using Godot;
using System;

public partial class PlayerController : Node2D
{
    [Export] TopdownMovementModule movementModule;
    GameManager manager;
    

    public override void _Ready()
    {
        manager = GetTree().CurrentScene as GameManager;
    }

    public override void _Process(double delta)
    {
        movementModule.InputRight = Input.IsActionJustPressed("moveRight");
        movementModule.InputLeft = Input.IsActionJustPressed("moveLeft");
        movementModule.InputUp = Input.IsActionJustPressed("moveUp");
        movementModule.InputDown = Input.IsActionJustPressed("moveDown");
    }
}
