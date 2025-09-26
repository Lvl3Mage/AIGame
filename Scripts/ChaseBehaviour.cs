using Godot;
using System;
using System.ComponentModel;

public partial class ChaseBehaviour : Node//, IBehaviour
{
 //    [Export] TopdownMovementModule movementModule;
 //
 //    public BehaviourPriority Priority { get; set; } = BehaviourPriority.High;
	// GameManager manager;
	// NavMesh mesh;
 //    Node2D target, root;
 //    Vector2I[] path = [];
 //    Vector2I nextPos;
 //    int chaseIndex = 1;
 //    float updateRate = 0.4f;
 //
 //    public override void _Ready()
 //    {
 //        root = GetOwner<Node2D>();
 //        manager = GetTree().CurrentScene as GameManager;
 //        mesh = manager.Mesh;
 //        target = manager.Player;
 //        UpdateChasePath();
 //        CreateTimer();
 //    }
 //
 //    public override void _Process(double delta)
 //    {
 //        // if (nextPos
 //    }
 //
 //    void UpdateChasePath()
 //    {
 //        if (path.Length > chaseIndex)
 //        {
 //            // Advance one cell in the path
 //            CellPosition = path[chaseIndex];
 //        }
 //
 //        float width = mesh.CellSize / 2;
 //        Vector2I currentCell = CellPosition;
 //        Vector2I targetCell = mesh.WorldToGrid(target.Position + new Vector2(width, width));
 //
 //        manager.ClearPath(path);
 //        path = mesh.GetShortestPathBFS(currentCell, targetCell, manager.Walls, true);
 //        manager.PaintPath(path);
 //    }
 //
 //    void CreateTimer()
 //    {
 //        Timer chaseTimer = new();
 //        AddChild(chaseTimer);
 //        chaseTimer.WaitTime = updateRate;
 //        chaseTimer.Autostart = true;
 //        chaseTimer.OneShot = false;
 //        chaseTimer.Timeout += UpdateChasePath;
 //        chaseTimer.Start();
 //    }
}
