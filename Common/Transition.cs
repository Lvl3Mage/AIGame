using Game.Common.Utility;
using Godot;
using System.Threading;
using System.Threading.Tasks;

namespace Game.Common;

public partial class Transition : Control
{
    [Export] ColorRect blackSquare;
    [Export] float TransitionTime;
    [Export] Curve TransitionCurve;

    CancellationTokenSource cts;

    public async Task FadeIn()
    {
        cts?.Cancel();
        cts = new CancellationTokenSource();
        await FadeScaleX(1f);
    }

    public async Task FadeOut()
    {
        cts?.Cancel();
        cts = new CancellationTokenSource();
        await FadeScaleX(0f);
    }

    float GetScaleX() => blackSquare.Scale.X;
    void SetScaleX(float value) => blackSquare.Scale = new(value, blackSquare.Scale.Y);
    async Task FadeScaleX(float targetScale) => await MathUtility.LerpAsync(GetScaleX, SetScaleX, targetScale, TransitionTime, cts.Token, TransitionCurve);
}
