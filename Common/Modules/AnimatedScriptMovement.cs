using System;
using Game.Common.Utility;
using Godot;

namespace Game.Common.Modules;

[GlobalClass]
public partial class AnimatedScriptMovement : Node2D
{

    [Export] CharacterBody2D charBody;
    private AnimatedSprite2D animatedSprite;
    public override void _Ready()
    {
        animatedSprite = GetParent() as AnimatedSprite2D;

        if (animatedSprite == null)
            GD.PushError("AnimatedScriptMovement's parent must be an AnimatedSprite2D.");
    }
    public override void _Process(double delta)
    {
        if (charBody.Velocity.Length() < 0.1f)
        {
            animatedSprite.Play("Idle");
        }
        else
        {
            animatedSprite.Play("Walk");
        }
    }
}