using Godot;
using System;
using System.Threading;
using System.Threading.Tasks;

[GlobalClass]
public partial class DynamicCamera : Camera2D
{
    [Export] public float MouseInfluence = 0.1f;
    [Export] public float SmoothSpeed = 3f;
    [Export] public Node2D followTarget;
    private Vector2 targetOffset = Vector2.Zero;

    private Vector2 shakeOffset = Vector2.Zero;
    private CancellationTokenSource shakeTokenSource;

    public override void _Ready()
    {
        GlobalPosition = followTarget.GlobalPosition;
    }

    public override void _Process(double delta)
    {
        Vector2 mousePos = GetViewport().GetMousePosition();
        Vector2 screenCenter = GetViewport().GetVisibleRect().Size / 2;

        Vector2 mouseOffset = (mousePos - screenCenter) * MouseInfluence;

        targetOffset = targetOffset.Lerp(mouseOffset + followTarget.GlobalPosition, (float)(SmoothSpeed * delta));

        GlobalPosition = targetOffset + shakeOffset;
    }

    public async Task ScreenShake(float strength, float duration)
    {
        shakeTokenSource?.Cancel();
        shakeTokenSource = new CancellationTokenSource();
        CancellationToken token = shakeTokenSource.Token;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            if (token.IsCancellationRequested)
            {
                shakeOffset = Vector2.Zero;
                return;
            }

            float offsetX = (float)(GD.Randf() * 2 - 1) * strength;
            float offsetY = (float)(GD.Randf() * 2 - 1) * strength;
            shakeOffset = new Vector2(offsetX, offsetY);

            elapsed += (float)GetProcessDeltaTime();
            await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
        }

        shakeOffset = Vector2.Zero;
        shakeTokenSource = null;
    }
}
