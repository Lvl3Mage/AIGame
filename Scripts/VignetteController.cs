using Godot;

namespace Game;

[GlobalClass]
public partial class VignetteController : Node
{
	[Export] ColorRect colorRect;
	[Export] ShaderMaterial shaderMaterial;
	[Export] float minRadius = 0;
	[Export] float maxRadius = 1;
	[Export] float lerpSpeed = 5;
	[Export] Color standardColor1 = new Color(0, 0, 0, 0.5f);
	[Export] Color standardColor2 = new Color(0, 0, 0, 0.7f);
	[Export] Color alertColor1 = new Color(0.5f, 0, 0, 0.5f);
	[Export] Color alertColor2 = new Color(0.5f, 0, 0, 0.7f);

	float currentAttenuation = 1;
	float targetAttenuation = 1;
	public override void _Ready()
	{

		colorRect.Material = shaderMaterial;

	}

	public void SetTargetAttenuation(float attenuation)
	{
		targetAttenuation = attenuation;
	}
	public void SetAttenuation(float attenuation)
	{
		currentAttenuation = attenuation;
		targetAttenuation = attenuation;
		shaderMaterial.SetShaderParameter("outerRadius", Mathf.Lerp(maxRadius, minRadius, currentAttenuation));
	}

	public override void _Process(double delta)
	{
		currentAttenuation = Mathf.Lerp(currentAttenuation, targetAttenuation, (float)delta * lerpSpeed);
		float smoothedAttenuation = Mathf.SmoothStep(0, 1, currentAttenuation);
		shaderMaterial.SetShaderParameter("outerRadius", Mathf.Lerp(maxRadius, minRadius, smoothedAttenuation));
		shaderMaterial.SetShaderParameter("color1", standardColor1.Lerp(alertColor1,smoothedAttenuation));
		shaderMaterial.SetShaderParameter("color2", standardColor2.Lerp(alertColor2,smoothedAttenuation));

	}
}