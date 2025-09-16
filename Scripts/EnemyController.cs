using Godot;
using System;

public partial class EnemyController : Node2D
{
    [Export] public float LerpSpeed = 10f;
    public Vector2I EnemyCell { get; set; }

    GameManager manager;
    GridModule grid;
    Node2D target;
    Vector2I[] path = [];
    int chaseIndex = 1;
    float updateRate = 0.4f;

    public override void _Ready()
    {
        manager = GetTree().CurrentScene as GameManager;
        grid = manager.Grid;
        target = manager.Player;
        EnemyCell = grid.WorldToGrid(Position);

        UpdateChasePath();
        CreateTimer();
    }

    public override void _Process(double delta)
    {
        Vector2 targetPos = grid.GridToWorld(EnemyCell);
        GlobalPosition = GlobalPosition.Lerp(targetPos, (float)(LerpSpeed * delta));
    }

    void UpdateChasePath()
    {
        if (path.Length > chaseIndex)
        {
            // Advance one cell in the path
            EnemyCell = path[chaseIndex];
        }

        float width = grid.CellSize / 2;
        Vector2I currentCell = EnemyCell;
        Vector2I targetCell = grid.WorldToGrid(target.Position + new Vector2(width, width));

        manager.ClearPath(path);
        path = grid.GetShortestPathBFS(currentCell, targetCell, manager.Walls, true);
        if (path.Length == 0) GD.Print("Path is null");
        manager.PaintPath(path);
    }

    void CreateTimer()
    {
        Timer chaseTimer = new();
        AddChild(chaseTimer);
        chaseTimer.WaitTime = updateRate;
        chaseTimer.Autostart = true;
        chaseTimer.OneShot = false;
        chaseTimer.Timeout += UpdateChasePath;
        chaseTimer.Start();
    }
}
