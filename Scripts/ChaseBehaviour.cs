using Godot;
using System;
using System.ComponentModel;

public partial class ChaseBehaviour : Node, IBehaviour
{
    [Export] public float LerpSpeed = 10f;

    public BehaviourPriority Priority { get; set; } = BehaviourPriority.High;
    public Vector2I CellPosition { get; set; }
	GameManager manager;
	GridModule grid;
    Node2D target, root;
    Vector2I[] path = [];
    int chaseIndex = 1;
    float updateRate = 0.4f;

    public override void _Ready()
    {
        root = GetOwner<Node2D>();
        manager = GetTree().CurrentScene as GameManager;
        grid = manager.Grid;
        target = manager.Player;
        CellPosition = grid.WorldToGrid(root.Position);

        UpdateChasePath();
        CreateTimer();
    }

    public override void _Process(double delta)
    {
        Vector2 targetPos = grid.GridToWorld(CellPosition);
        root.GlobalPosition = root.GlobalPosition.Lerp(targetPos, (float)(LerpSpeed * delta));
    }

    void UpdateChasePath()
    {
        if (path.Length > chaseIndex)
        {
            // Advance one cell in the path
            CellPosition = path[chaseIndex];
        }

        float width = grid.CellSize / 2;
        Vector2I currentCell = CellPosition;
        Vector2I targetCell = grid.WorldToGrid(target.Position + new Vector2(width, width));

        manager.ClearPath(path);
        path = grid.GetShortestPathBFS(currentCell, targetCell, manager.Walls, true);
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
