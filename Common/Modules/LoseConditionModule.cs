using Godot;
using System;
using System.Linq;

namespace Game.Common.Modules;

[GlobalClass]
public partial class LoseConditionModule : Node
{
    [Export] Area2D damageArea;
    bool isPlayer = false;

    public override void _Ready()
    {
        damageArea.BodyEntered += OnBodyEntered;
    }

    void OnBodyEntered(Node2D body)
    {
        GD.Print("CollisioN!");
        if (body is PlayerController)
            GetTree().ReloadCurrentScene();
    }
}