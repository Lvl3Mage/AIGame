using Godot;

namespace Game.Common.Modules;

/// <summary> FlashModule is a reusable component that provides a flashing effect for a CanvasItem, better suited for Sprite nodes. </summary>
[GlobalClass]
public partial class FlashModule : Node2D
{
    [Export] public Color Color = Colors.White;

    [Export(PropertyHint.Range, "0,1,0.001")]
    public float Intensity { get; set; } = 1f; // Default intensity of the flash, clamped between 0 and 1.

    [Export(PropertyHint.Range, "0,0.001,or_greater,hide_slider,suffix:seconds")]
    public float Duration { get; set; } = 0.1f; // Default duration of the flash on maximum intensity.

    [Export(PropertyHint.Range, "0,0.001,or_greater,hide_slider,suffix:seconds")]
    public float FadeInTime { get; set; } = 0f; // Default fade-in duration.

    [Export(PropertyHint.Range, "0,0.001,or_greater,hide_slider,suffix:seconds")]
    public float FadeOutTime { get; set; } = 0.1f; // Default fade-out duration.
    [Export] public bool FlashResetsFlash { get; set; } = true; // If true, a new flash will reset the timer if it is already flashing.

    float TotalDuration => currentFadeInDuration + currentDuration + currentFadeOutDuration;
    CanvasItem targetNode;
    ShaderMaterial material;
    Color currentFlashColor;
    float currentIntensity;
    float currentDuration;
    float currentFadeInDuration;
    float currentFadeOutDuration;
    float timer = 0f;
    bool flashing = false;

    const string ShaderParamName = "flash_value";
    const string ShaderColorName = "flash_color";

    [Signal] public delegate void TriggerFlashEventHandler(float intensity, float duration, float fadeInDuration = 0f, float fadeOutDuration = 0.1f, Color flashColor = default);

    public override void _Ready()
    {
        targetNode = GetParent() as CanvasItem;

        if (targetNode == null)
        {
            SetProcess(false);
            return;
        }

        material = GetFlashShader(targetNode);
        SetFlashParameters(); // Set default flash parameters
        TriggerFlash += Flash;
    }

    public override void _Process(double delta)
    {
        if (!flashing || material == null) return;
        if (timer >= TotalDuration) flashing = false;

        material.SetShaderParameter(ShaderParamName, GetFlashValue(timer));
        timer += (float)delta;
    }

    /// <summary> Initiates a flash with the specified parameters or uses default flash parameters if not provided. </summary>
    /// <param name="intensity">Intensity of the flash, clamped between 0 and 1.</param>
    /// <param name="duration">Duration of the flash in seconds.</param>   
    /// <param name="fadeInDuration">Duration of the fade-in effect in seconds.</param>
    /// <param name="fadeOutDuration">Duration of the fade-out effect in seconds.</param>
    public void Flash(float intensity = -1, float duration = -1, float fadeInDuration = -1, float fadeOutDuration = -1, Color flashColor = default)
    {
        SetFlashParameters(intensity, duration, fadeInDuration, fadeOutDuration, flashColor);

        bool isFadingIn = flashing && timer < currentFadeInDuration;
        bool isFadingOut = flashing && timer >= (currentFadeInDuration + currentDuration);

        if (flashing && !FlashResetsFlash)
        {
            if (isFadingOut)
                timer = Mathf.Lerp(0f, fadeInDuration, GetFlashValue(timer) / currentIntensity);
            else if (!isFadingIn)
                timer = fadeInDuration;
        }
        else // Start a new flash
        {
            timer = 0f;
            flashing = true;
        }
    }

    /// <summary> Sets the flash parameters for the next flash effect. </summary>
    void SetFlashParameters(float intensity = -1, float duration = -1, float fadeInTime = -1, float fadeOutTime = -1, Color flashColor = default)
    {
        currentIntensity = intensity == -1 ? Intensity : Mathf.Clamp(intensity, 0f, 1f);
        currentDuration = duration == -1 ? Duration : Mathf.Max(0f, duration);
        currentFadeInDuration = fadeInTime == -1 ? FadeInTime : Mathf.Max(0f, fadeInTime);
        currentFadeOutDuration = fadeOutTime == -1 ? FadeOutTime : Mathf.Max(0f, fadeOutTime);
        currentFlashColor = flashColor == default ? Color : flashColor;

        material?.SetShaderParameter(ShaderColorName, currentFlashColor);
    }

    /// <summary> Gets the current flash value based on the timer and durations. </summary>
    float GetFlashValue(float currentTime)
    {
        if (!flashing || TotalDuration == 0f)
            return 0f;
        if (timer < currentFadeInDuration)
            return Mathf.Lerp(0f, currentIntensity, timer / currentFadeInDuration);
        if (timer < currentFadeInDuration + currentDuration)
            return currentIntensity;
        if (timer < TotalDuration)
            return Mathf.Lerp(currentIntensity, 0f, (timer - currentFadeInDuration - currentDuration) / currentFadeOutDuration);

        return 0f;
    }

    /// <summary> Gets the ShaderMaterial from the target node, or creates one if it doesn't exist. </summary>
    ShaderMaterial GetFlashShader(CanvasItem node)
    {
        if (node.Material is ShaderMaterial smat && smat.Shader != null) return smat;

        var shaderCode = @"
        shader_type canvas_item;

        uniform vec4 flash_color : source_color;
        uniform float flash_value : hint_range(0.0, 1.0, 0.1);

        void fragment() {
            vec4 texture_color = texture(TEXTURE, UV);
            COLOR.rgb = mix(texture_color.rgb, flash_color.rgb, flash_value);
            COLOR.a = texture_color.a;
        }";

        Shader shader = new() { Code = shaderCode };
        ShaderMaterial newMat = new() { Shader = shader };
        node.Material = newMat;
        return newMat;
    }
}