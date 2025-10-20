using Game.Common;
using Game.Common.Utility;
using Godot;

namespace Game;
public partial class Door : Node2D
{
    [Export] float fadeSpeed = 1f;
    [Export] public Node2D NeededKey { get; private set; }
    [Export] Node2D[] occupiedPositions;
    [Export] public Sprite2D sprite;
    [Export] public CollisionShape2D collider;
    [Export] Texture2D openDoorSprite;
    [Export] PointLight2D light2D;
    [Export] float openSoundVolume = 1f;
    [Export] float shakeStrength = 15f;

    bool opened = false;
    Texture2D closedDoorSprite;

    public override void _Ready()
    {
        closedDoorSprite = sprite.Texture;
        ToggleOccupancy(true);
    }
    void ToggleOccupancy(bool occupied)
    {
        foreach (var position in occupiedPositions)
        {
            Vector2I coords = GameManager.Instance.GridDef.WorldToGrid(position.GlobalPosition);
            GameManager.Instance.GridOccupation.SetOccupancyOverride(coords, occupied);
            GD.Print("Occupied: " + position.GlobalPosition);
        }
    }

    public async void Open()
    {
        if (opened) return;
        ToggleOccupancy(false);
        sprite.Texture = openDoorSprite;

        _ = GameManager.Instance.Camera.ScreenShake(shakeStrength, fadeSpeed);
        await MathUtility.LerpAsync(() => light2D.Color.G, v => light2D.Color = new Color(light2D.Color.R, v, light2D.Color.B, light2D.Color.A), 2f, fadeSpeed);
        _ = MathUtility.LerpAsync(() => light2D.Energy, v => light2D.Energy = v, 0f, fadeSpeed);
        AudioManager.PlayAudio2D(SoundLibrary.Instance.DoorOpen, this, openSoundVolume);
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
