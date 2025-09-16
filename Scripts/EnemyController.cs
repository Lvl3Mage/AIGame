using Godot;
using System;

public partial class EnemyController : Node2D
{
    [Export] public float LerpSpeed = 10f;
    public Vector2I CellPosition { get; set; }
}
