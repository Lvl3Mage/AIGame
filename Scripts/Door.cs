using Godot;
using System;

namespace Game;
public partial class Door : Node2D
{
    [Export] public Key NeededKey { get; private set; }
    [Export] float fadeSpeed = 1.5f;
    [Export] public Sprite2D sprite;
    [Export] public CollisionShape2D collider;

    bool opened = false;
    bool fading = false;

    public void Open()
    {
        if (opened) return;
        opened = true;
        fading = true;
        collider.Disabled = false;
    }

    public void Close()
    {
        sprite.Modulate = new Color(1, 1, 1, 1);
        collider.Disabled = false;
        opened = false;
        fading = false;
    }

    public override void _Process(double delta)
    {
        if (fading && sprite != null)
        {
            float newAlpha = sprite.Modulate.A - (float)(delta / fadeSpeed);
            if (newAlpha <= 0)
            {
                newAlpha = 0;
                fading = false;
                collider.Disabled = true;
            }

            sprite.Modulate = new Color(1, 1, 1, newAlpha);
        }
    }
}
