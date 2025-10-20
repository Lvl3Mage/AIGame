using System;
using System.Linq;
using Godot;
using Game.Common.Modules;
using Godot.Collections;

namespace Game.Common.AgentControl.BehaviourManagement;

[GlobalClass]
public partial class AgentModules : Node
{
    [Export] public CharacterBody2D AgentBody { get; private set; }
    [Export] public Area2D DetectionArea { get; private set; }
    [Export] public RayCast2D LineOfSightRay { get; private set; }
    [Export] public MovementModule MovementModule { get; private set; }
    [Export] public Sprite2D Sprite { get; private set; }

    public bool PlayerVisible { get; private set; } = false;
    public event Action PlayerVisibilityChanged;
    public Vector2 LastVisiblePlayerPosition { get; private set; }
    public float TimeSincePlayerVisible { get; private set; } = 0f;
    public Vector2 PlayerDirection { get; private set; }

    PlayerController player;
    Vector2 pastPlayerPosition;

    public override void _Ready()
    {
        player = GameManager.Instance.Player;
        pastPlayerPosition = player.GlobalPosition;
    }

    public override void _Process(double delta)
    {

        UpdatePlayerDetection();
        if (PlayerVisible){
            TimeSincePlayerVisible = 0f;
        }
        else
        {
            TimeSincePlayerVisible += (float)delta;
        }
        PlayerDirection = (player.GlobalPosition - pastPlayerPosition).Normalized();

        pastPlayerPosition = player.GlobalPosition;

        // Flip animation
        if (AgentBody.Velocity.X > 0) Sprite.FlipH = false;
        else if (AgentBody.Velocity.X < 0) Sprite.FlipH = true;
    }
    void UpdatePlayerDetection()
    {
        Array<Node2D> bodiesInArea = DetectionArea.GetOverlappingBodies();
        bool playerInArea = bodiesInArea.Any(body => body == player);
        if (playerInArea)
        {
            LineOfSightRay.TargetPosition = AgentBody.ToLocal(player.GlobalPosition);
            LineOfSightRay.ForceRaycastUpdate();
            if (LineOfSightRay.GetCollider() == player)
            {
                LastVisiblePlayerPosition = player.GlobalPosition;
                SetPlayerVisibility(true);
            }
            else
            {
                SetPlayerVisibility(false);
            }
        }
        else
        {
            SetPlayerVisibility(false);
        }
    }

    void SetPlayerVisibility(bool visible)
    {
        bool vis = PlayerVisible;
        PlayerVisible = visible;
        if (vis != PlayerVisible){
            PlayerVisibilityChanged?.Invoke();
        }
    }

}