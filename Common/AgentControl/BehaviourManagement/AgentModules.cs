using System;
using System.Linq;
using Godot;
using Game.Common.Modules;
using Godot.Collections;
using Game.Global;

namespace Game.Common.AgentControl.BehaviourManagement;

[GlobalClass]
public partial class AgentModules : Node
{
    [Export] public CharacterBody2D AgentBody { get; private set; }
    [Export] public Area2D DetectionArea { get; private set; }
    [Export] public Area2D FarsightArea { get; private set; }
    [Export] public RayCast2D LineOfSightRay { get; private set; }
    [Export] public MovementModule MovementModule { get; private set; }
    [Export] public SquishModule SquishModule { get; private set; }
    [Export] public CharacterAnimationModule CharacterAnimationModule { get; private set; }
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

        // Animate sprite
        if (AgentBody.Velocity.X > 0) Sprite.FlipH = false;
        else if (AgentBody.Velocity.X < 0) Sprite.FlipH = true;

        SquishModule.UpdateDynamicScaling(CharacterAnimationModule.OffsetY);
    }
    void UpdatePlayerDetection()
    {
        Array<Node2D> bodiesInDetectionArea = DetectionArea.GetOverlappingBodies();
        Array<Node2D> bodiesInFarsightArea = FarsightArea.GetOverlappingBodies();

        LineOfSightRay.TargetPosition = AgentBody.ToLocal(player.GlobalPosition);
        LineOfSightRay.ForceRaycastUpdate();

        bool hasLineOfSight = LineOfSightRay.GetCollider() == player;
        bool playerInDetectionArea = bodiesInDetectionArea.Any(body => body == player);
        bool playerInFarsightArea = bodiesInFarsightArea.Any(body => body == player);

        if (hasLineOfSight && playerInDetectionArea)
        {
            LastVisiblePlayerPosition = player.GlobalPosition;
            SetPlayerVisibility(true);
        }
        if (!playerInFarsightArea || !hasLineOfSight) SetPlayerVisibility(false);
       
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