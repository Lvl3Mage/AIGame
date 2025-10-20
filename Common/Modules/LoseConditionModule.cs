using Godot;

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
        if (body == GameManager.Instance.Player)
            GameManager.Instance.Player.Die();
    }
}