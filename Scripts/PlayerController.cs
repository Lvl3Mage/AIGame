using Godot;
using System;

public partial class PlayerController : Node2D
{
    GameManager manager;
    GridModule grid;
    public Vector2I PlayerCell { get; set; } = new(4, 4);

    [Export] public float LerpSpeed { get; set; } = 10f;

    public override void _Ready()
    {
        manager = GetTree().CurrentScene as GameManager;
        grid = manager.Grid;

        GlobalPosition = grid.GridToWorld(PlayerCell);
    }

    public override void _Process(double delta)
    {
        Vector2I move = Vector2I.Zero;

        if (Input.IsActionJustPressed("moveRight")) move = Vector2I.Right;
        else if (Input.IsActionJustPressed("moveLeft")) move = Vector2I.Left;
        else if (Input.IsActionJustPressed("moveUp")) move = Vector2I.Up;
        else if (Input.IsActionJustPressed("moveDown")) move = Vector2I.Down;

        if (move != Vector2I.Zero) PlayerCell += move;

        Vector2 targetPos = grid.GridToWorld(PlayerCell);
       GlobalPosition = GlobalPosition.Lerp(targetPos, (float)(LerpSpeed * delta));
    }
}
