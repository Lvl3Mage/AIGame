using Game.Common.Utility;
using Godot;
using System;
using System.Threading.Tasks;

namespace Game;
public partial class Door : Node2D
{
    [Export] float fadeSpeed = 1f;
    [Export] public Node2D NeededKey { get; private set; }
    [Export] public Sprite2D sprite;
    [Export] public CollisionShape2D collider;
    [Export] Texture2D openDoorSprite;
    [Export] PointLight2D light2D;

    bool opened = false;
    Texture2D closedDoorSprite;

    public override void _Ready()
    {
        closedDoorSprite = sprite.Texture;
    }

    public async void Open()
    {
        if (opened) return;
        sprite.Texture = openDoorSprite;
        await MathUtility.LerpAsync(() => light2D.Color.G, v => light2D.Color = new Color(light2D.Color.R, v, light2D.Color.B, light2D.Color.A), 2f, fadeSpeed);
        _ = MathUtility.LerpAsync(() => light2D.Energy, v => light2D.Energy = v, 0f, fadeSpeed);
        await MathUtility.LerpAsync(() => sprite.Modulate.A, v => sprite.Modulate = new Color(sprite.Modulate.R, sprite.Modulate.G, sprite.Modulate.B, v), 0f, fadeSpeed);
        Callable.From(() => collider.Disabled = true).CallDeferred();
        opened = true;
    }

    public void Close()
    {
        sprite.Modulate = new Color(1, 1, 1, 1);
        Callable.From(() => collider.Disabled = true).CallDeferred();
        opened = false;
        sprite.Texture = closedDoorSprite;
    }
}
