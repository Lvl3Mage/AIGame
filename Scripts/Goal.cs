using Game.Common;
using Godot;
using System;

namespace Game;

public partial class Goal : Area2D
{
    public override void _Ready()
    {
        BodyEntered += OnBodyEntered;
    }

    void OnBodyEntered(Node2D body)
    {
        if (body == GameManager.Instance.Player)
            _ = GameManager.Instance.WinGame();
    }
}
