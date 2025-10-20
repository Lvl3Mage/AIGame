using Game.Common.AgentControl.Strategies;
using Godot;

namespace Game.Common.AgentControl.BehaviourManagement;

[GlobalClass]
public partial class ChaseBehaviour : Node, IPrioritizedBehaviour
{
	[Export] AgentModules modules;
	[Export] PointLight2D light2D;
	[Export] Color lightColor = Colors.White;
	[Export] float chaseSpeed = 100f;
	[Export] float alertVolume = 1f;
	[Export] float growlsVolume = 1f;
	[Export] float shakeStrength = 10f;
	[Export] float screamDuration = 0.6f;

	bool isActive, isScreaming;
	PlayerController player;
	Timer growlTimer;

	public override void _Ready()
	{
		player = GameManager.Instance.Player;
	}

	public IPrioritizedBehaviour.Priority GetPriority()
	{
		return modules.PlayerVisible
			? IPrioritizedBehaviour.Priority.Important
			: IPrioritizedBehaviour.Priority.Disabled;
	}

	public async void StartBehavior()
	{
		isActive = true;
		isScreaming = true;
		light2D.Color = lightColor;
		AudioManager.PlayAudio2D(SoundLibrary.Instance.AlertadorAlert, modules.AgentBody, alertVolume).Finished += Growl;
		GetTree().CreateTimer(screamDuration).Timeout += () => isScreaming = false;
		_ = GameManager.Instance.Camera.ScreenShake(shakeStrength, screamDuration);
	}

	public void StopBehavior()
	{
		isActive = false;
		modules.MovementModule.SetTargetVelocity(Vector2.Zero);

		if (growlTimer != null && growlTimer.IsInsideTree())
		{
			growlTimer.Stop();
			growlTimer.QueueFree();
			growlTimer = null;
		}
	}

	public override void _Process(double delta)
	{
		DebugDraw2D.SetText("Chase", GetPriority().ToString());
		DebugDraw2D.SetText("Active", isActive.ToString());
		if (!isActive) return;
		Vector2 targetDirection = (player.GlobalPosition - modules.AgentBody.GlobalPosition).Normalized();
		float screamFactor = isScreaming ? 0f : 1f;

		modules.MovementModule.SetTargetVelocity(targetDirection * chaseSpeed * screamFactor);
		AgentDirector.Instance.AddPlayerVisibleEvent(new PlayerVisibleEvent
		{
			PlayerPosition = player.GlobalPosition,
			PlayerDirection = modules.PlayerDirection,
			Strength = 5f * (float)delta,
			Origin = modules.AgentBody.GlobalPosition,
		});

	}

	void Growl()
	{
		isScreaming = false;
		if (!isActive) return;

		AudioStreamPlayer2D player = AudioManager.PlayAudio2D(SoundLibrary.Instance.AlertadorIdle, modules.AgentBody, growlsVolume);
		player.Finished += Growl;
	}
}
