using System.Threading.Tasks;
using Game.Common.AgentControl.Strategies;
using Godot;

namespace Game.Common.AgentControl.BehaviourManagement;
//pretty ugly code duplication here but i guess it works

[GlobalClass]
public partial class ScreamBehaviour : Node, IPrioritizedBehaviour
{
	[Export] AgentModules modules;
	[Export] Color lightColor = Colors.White;
	[Export] float alertVolume = 1f;
	[Export] float growlsVolume = 1f;
	[Export] float shakeStrength = 10f;
	[Export] float screamDuration = 0.6f;
	[Export] float screamInterval = 1.2f; // Delay between screams

	bool isActive;
	PlayerController player;
	Timer screamTimer;

	public override void _Ready()
	{
		player = GameManager.Instance.Player;
	}

	public IPrioritizedBehaviour.Priority GetPriority()
	{
		return modules.PlayerVisible
			? IPrioritizedBehaviour.Priority.Critical
			: IPrioritizedBehaviour.Priority.Disabled;
	}

	public void StartBehavior()
	{
		isActive = true;
		modules.animatedSprite.Play("Alert");
		modules.light.Color = lightColor;

		Scream(); // Initial scream

		// Setup repeating screams
		screamTimer = new Timer();
		screamTimer.WaitTime = screamInterval;
		screamTimer.OneShot = false;
		screamTimer.Timeout += Scream;
		AddChild(screamTimer);
		screamTimer.Start();
	}

	public void StopBehavior()
	{
		isActive = false;
		modules.MovementModule.SetTargetVelocity(Vector2.Zero);

		if (screamTimer != null && screamTimer.IsInsideTree())
		{
			screamTimer.Stop();
			screamTimer.QueueFree();
			screamTimer = null;
		}
	}

	void Scream()
	{
		if (!isActive) return;

		AudioManager.PlayAudio2D(SoundLibrary.Instance.AlertadorIdle, modules.AgentBody, alertVolume);
		_ = GameManager.Instance.Camera.ScreenShake(shakeStrength, screamDuration);
	}

	public override void _Process(double delta)
	{
		// DebugDraw2D.SetText("Scream Active", isActive.ToString());
		if (!isActive) return;

		// No movement
		modules.MovementModule.SetTargetVelocity(Vector2.Zero);

		// Still report player visible
		AgentDirector.Instance.AddScreamEvent(new ScreamEvent
		{
			PlayerPosition = player.GlobalPosition,
			PlayerDirection = modules.PlayerDirection,
			Strength = 5f * (float)delta,
		});
		AgentDirector.Instance.AddPlayerVisibleEvent(new PlayerVisibleEvent()
		{
			PlayerPosition = player.GlobalPosition,
			PlayerDirection = modules.PlayerDirection,
			Strength = 5f * (float)delta,
		});
	}
}
