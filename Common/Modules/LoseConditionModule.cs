using Godot;
using System.Linq;

namespace Game.Common.Modules;

[GlobalClass]
public partial class LoseConditionModule : Node
{
    [Export] CharacterBody2D agentBody;
    bool isPlayer = false;

    public override void _PhysicsProcess(double delta)
    {
        DebugDraw2D.SetText("Enemigo ha colisionado con el jugador?", isPlayer);

        // for (KinematicCollision2D collision = agentBody.GetSlideCollision(0); collision != null; collision = collision.Next)
        for (int i = 0; i < agentBody.GetSlideCollisionCount(); i++)
        {
            KinematicCollision2D collision = agentBody.GetSlideCollision(i);
            if (collision.GetCollider() is Node2D collider)
            {
                if (collider.IsInGroup("player"))
                {
                    isPlayer = true;
                    GD.Print("¡Colisión con el jugador! Reiniciando la escena...");
                    GetTree().ReloadCurrentScene();
                    break;
                }
                else
                {
                    isPlayer = false;
                }
            }
        }
    }
}