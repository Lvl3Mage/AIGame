using Game.Common.Utility;
using Godot;
using System.Threading;
using System.Threading.Tasks;

namespace Game.Common;

public partial class Transition : Control
{
    [Export] ColorRect blackSquare;
    [Export] public float TransitionTime { get; set; } = 0.8f;
    [Export] float soundVolume = 1f;
    [Export] public Curve TransitionCurve { get; set; }

    CancellationTokenSource cts;

    public async Task FadeIn() => await FadeScaleX(1f);

    public async Task FadeOut() => await FadeScaleX(0f);

    float GetScaleX() => blackSquare.Scale.X;

    void SetScaleX(float value) => blackSquare.Scale = new(value, blackSquare.Scale.Y);

    async Task FadeScaleX(float targetScale)
    {
        cts?.Cancel();
        cts = new CancellationTokenSource();
        AudioManager.PlayAudio(SoundLibrary.Instance.Transition, soundVolume);
        await MathUtility.LerpAsync(GetScaleX, SetScaleX, targetScale, TransitionTime, cts.Token, TransitionCurve);
    }
}
