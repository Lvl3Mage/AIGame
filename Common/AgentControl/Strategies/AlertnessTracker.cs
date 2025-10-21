using Godot;

namespace Game.Common.AgentControl.Strategies;

[GlobalClass]
public partial class AlertnessTracker : Node, IAgentEventListener<PlayerVisibleEvent>
{
	[Export] float decayRate = 0.1f;
	[Export] float increaseRate = 0.5f;
	[Export] float maxAlertness = 10.0f;

	public override void _Ready()
	{
		GameManager.Instance.Vignette.SetAttenuation(0);
	}

	public override void _Process(double delta)
	{
		alertnessLevel -= decayRate * (float)delta;
		alertnessLevel = Mathf.Clamp(alertnessLevel, 0.0f, maxAlertness);
		// DebugDraw2D.SetText("Alertness Level", alertnessLevel.ToString("F2"));
		GameManager.Instance.Vignette.SetTargetAttenuation(Mathf.InverseLerp(0.0f, maxAlertness, alertnessLevel));
	}
	float alertnessLevel = 0.0f;
	public void OnEvent(PlayerVisibleEvent agentEvent)
	{
		alertnessLevel += increaseRate * agentEvent.Strength;
		alertnessLevel = Mathf.Clamp(alertnessLevel, 0.0f, maxAlertness);
	}
}