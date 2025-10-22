using Game.Common.Managers;
using Godot;

namespace Game.Common.AgentControl.BehaviourManagement;

[GlobalClass]
public partial class InvestigateBehaviour : Node, IPrioritizedBehaviour
{
	[Export] AgentModules modules;
	[Export] PointLight2D light2D;
	[Export] Color lightColor = Colors.White;
	[Export] float growlsVolume = 0.5f;
	[Export] float growlFrequency = 2;
	[Export] float growlRandomization = 0.4f;
	
	float RandomGrowlFrequency => (float)(growlFrequency + GD.RandRange(-growlRandomization * growlFrequency, growlRandomization * growlFrequency));
	bool isActive;
	readonly PathFollower investigationPath = new();
	Timer growlTimer;

	public override void _Ready()
	{
		modules.PlayerVisibilityChanged += OnPlayerVisibilityChanged;
	}

	void OnPlayerVisibilityChanged()
	{
		investigationPath.SetPoints(GameManager.Instance.GridNav.GetPathBetween(
				modules.AgentBody.GlobalPosition,
				modules.LastVisiblePlayerPosition
			)
		);
		investigationPath.RecalculateAtPosition(modules.AgentBody.GlobalPosition,investigateRadius);
	}

	public IPrioritizedBehaviour.Priority GetPriority()
	{
		if (investigationPath.PathComplete()) return IPrioritizedBehaviour.Priority.Disabled;
		return IPrioritizedBehaviour.Priority.Important;
	}

	public void StartBehavior()
	{
		isActive = true;
		light2D.Color = lightColor;
		CreateGrowlTimer();
	}

	public void StopBehavior()
	{
		isActive = false;
		modules.MovementModule.SetTargetVelocity(Vector2.Zero);
		investigationPath.SetPoints([]);

		if (growlTimer != null && growlTimer.IsInsideTree())
		{
			growlTimer.Stop();
			growlTimer.QueueFree();
			growlTimer = null;
		}
	}

	[Export] float moveSpeed = 100;
	[Export] float investigateRadius = 40;

	public override void _Process(double delta)
	{
		if (investigationPath.PathComplete()) return;

		if (!isActive) return;
		// foreach (Vector2 point in  investigationPath.GetPathPoints()){
		// 	DebugDrawQueue.DebugDrawCircle(point,30,Colors.Aqua);
		// }
		MoveAlongPath();
		TryAdvancePath();
	}

	void TryAdvancePath()
	{
		investigationPath.TryAdvancePath(modules.AgentBody.GlobalPosition, investigateRadius);
	}

	void MoveAlongPath()
	{
		Vector2? dir = investigationPath?.GetTargetDirection(modules.AgentBody.GlobalPosition);
		if (dir == null)
		{
			return;
		}

		modules.MovementModule.SetTargetVelocity(dir.Value * moveSpeed);
	}
	
	void CreateGrowlTimer()
	{
		growlTimer = new()
		{
			WaitTime = RandomGrowlFrequency,
			OneShot = false
		};
		modules.AgentBody.AddChild(growlTimer);
		growlTimer.Timeout += Growl;
		growlTimer.Start();
	}

	void Growl()
	{
		AudioManager.PlayAudio2D(SoundLibrary.Instance.AlertadorIdle, modules.AgentBody, growlsVolume);
		growlTimer.WaitTime = RandomGrowlFrequency;
	}
}