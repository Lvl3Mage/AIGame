using Godot;

namespace Game.Common.AgentControl.BehaviourManagement;

public partial class ChaseBehaviour : Node, IPrioritizedBehaviour
{
	[Export] AgentModules modules;
	[Export] CharacterBody2D enemyBody;
	[Export] float chaseSpeed = 100f;
	[Export] float alertVolume = 1f;
	[Export] float growlsVolume = 1f;
	
	bool isActive;
	PlayerController player;
	Timer growlTimer;

	public override void _Ready()
	{
		player = GameManager.Instance.Player;
	}

	public IPrioritizedBehaviour.Priority GetPriority()
	{
		return modules.PlayerVisible ? IPrioritizedBehaviour.Priority.Important : IPrioritizedBehaviour.Priority.Disabled;
	}

	public void StartBehavior()
	{
		isActive = true;
		AudioManager.PlayAudio2D(SoundLibrary.Instance.AlertadorAlert, enemyBody, alertVolume).Finished += Growl;
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
		modules.MovementModule.SetTargetVelocity(targetDirection * chaseSpeed);
	}

	void Growl()
	{
		if (!isActive) return;

		AudioStreamPlayer2D player = AudioManager.PlayAudio2D(SoundLibrary.Instance.AlertadorIdle, enemyBody, growlsVolume);
		player.Finished += Growl;
	}
}
