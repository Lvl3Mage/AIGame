using Godot;
using System.Linq;

namespace Game.Common.Modules;

[GlobalClass]
public partial class MovementModule : Node
{
    [Export] CharacterBody2D agentBody;
    Vector2 targetVelocity = Vector2.Zero;
    [Export] float acceleration = 500f;
    public void SetTargetVelocity(Vector2 velocity)
    {
        targetVelocity = velocity;
    }
    public override void _PhysicsProcess(double delta)
    {
        Vector2 velocity = agentBody.Velocity;
        Vector2 newVelocity = velocity.MoveToward(targetVelocity, acceleration * (float)delta);
        DebugDraw2D.SetText("Velocity", newVelocity.ToString());
        agentBody.Velocity = newVelocity;
        agentBody.MoveAndSlide();
    }
}