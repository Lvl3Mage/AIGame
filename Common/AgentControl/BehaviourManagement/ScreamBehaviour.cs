using Godot;

namespace Game.Common.AgentControl.BehaviourManagement;

public partial class ScreamBehaviour : Node, IPrioritizedBehaviour
{
	// [Export] private Color lightColor = Color.Red;
	// [Export] private float alertVolume = 0.8f;
	// [Export] private float screamDuration = 2.0f;
	[Export] AgentModules modules;
	//
	//
	// public void StartBehavior()
	// {
	// 	modules.MovementModule.SetTargetVelocity(Vector2.Zero);
	//
	// 	light2D.Color = lightColor;
	// 	AudioManager.PlayAudio2D(SoundLibrary.Instance.AlertadorAlert, modules.AgentBody, alertVolume).Finished += Growl;
	// 	GetTree().CreateTimer(screamDuration).Timeout += () => isScreaming = false;
	// 	_ = GameManager.Instance.Camera.ScreenShake(shakeStrength, screamDuration);
	// 	await Task.Delay(100); // Small delay to ensure it flips sprite into the player
	// }

	public void StartBehavior()
	{
		throw new System.NotImplementedException();
	}

	public void StopBehavior()
	{
		modules.MovementModule.SetTargetVelocity(Vector2.Zero);
	}

	public IPrioritizedBehaviour.Priority GetPriority()
	{
		return modules.PlayerVisible ? IPrioritizedBehaviour.Priority.Critical : IPrioritizedBehaviour.Priority.Disabled;
	}
}