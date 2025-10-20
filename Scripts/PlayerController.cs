using System;
using System.Threading.Tasks;
using Game.Common.Modules;
using Game.Common.Utility;
using Game.Global;
using Godot;

namespace Game.Common;

public partial class PlayerController : CharacterBody2D
{
    [Export] TopdownMovementModule movementModule;
    //[Export] Sprite2D sprite;
    [Export] AnimatedSprite2D animatedSprite;
    [Export] CharacterAnimationModule animationModule;
    [Export] SquishModule squishModule;
    [Export] CollisionShape2D collider;
    [Export] float dieAnimationTime = 0.5f;
    [Export] float stepSoundInterval = 0.4f;
    [Export] float stepSoundPitch = 1f;
    [Export] float stepSoundVolume = 1f;
    [Export] float deathSoundVolume = 1f;
    [Export] float deathScreenshakeStrength = 20f;
    [Export] float deathScreenshakeDuration = 0.3f;

    public bool LockMovement { get; set; }

    bool pressRight, pressLeft, pressUp, pressDown;
    Timer stepSoundTimer;

    public override void _Ready()
    {
        LockMovement = false;

        stepSoundTimer = new()
        {
            WaitTime = stepSoundInterval,
            OneShot = false,
            Autostart = true
        };
        AddChild(stepSoundTimer);
        stepSoundTimer.Timeout += CheckPlayStepSound;
    }

    public override void _Process(double delta)
    {
        float dt = (float)delta;

        if (LockMovement)
            pressRight = pressLeft = pressUp = pressDown = false;
        else
            MapActions();

        movementModule.InputRight = pressRight;
        movementModule.InputLeft = pressLeft;
        movementModule.InputUp = pressUp;
        movementModule.InputDown = pressDown;

        // Animate sprite
        if (pressRight) animatedSprite.FlipH = false;
        else if (pressLeft) animatedSprite.FlipH = true;

        squishModule.UpdateDynamicScaling(animationModule.OffsetY);
    }

    void MapActions()
    {
        pressRight = Input.IsActionPressed("moveRight");
        pressLeft = Input.IsActionPressed("moveLeft");
        pressUp = Input.IsActionPressed("moveUp");
        pressDown = Input.IsActionPressed("moveDown");
    }

    public void Die() => _ = TriggerDieAnimation();

    async Task TriggerDieAnimation()
    {
        float getRotation() => animatedSprite.RotationDegrees;
        void setRotation(float v) => animatedSprite.RotationDegrees = v;

        float getRedColor() => animatedSprite.SelfModulate.R;
        void setRedColor(float v) => animatedSprite.SelfModulate = new Color(v, animatedSprite.SelfModulate.G, animatedSprite.SelfModulate.B, animatedSprite.SelfModulate.A);

        LockMovement = true;
        Velocity = Vector2.Zero;
        Callable.From(() => collider.Disabled = true).CallDeferred();
        GameManager.Instance.TimeFreezer.FrameFreeze(0f, 0.2f);
        AudioManager.PlayAudio2D(SoundLibrary.Instance.PlayerDeath, this, deathSoundVolume);
        _ = GameManager.Instance.Camera.ScreenShake(deathScreenshakeStrength, deathScreenshakeDuration);

        _ = MathUtility.LerpAsync(getRedColor, setRedColor, 255f, dieAnimationTime);
        await MathUtility.LerpAsync(getRotation, setRotation, 90f, dieAnimationTime);

        GameManager.Instance.RestartGame();
    }

    void CheckPlayStepSound()
    {
        if (Velocity.Length() > movementModule.MaxMoveSpeed * 0.1f)
            AudioManager.PlayAudio2D(SoundLibrary.Instance.PlayerSteps, this, stepSoundVolume, stepSoundPitch);
    }
}