using Godot;

namespace Game.Common.AgentControl.BehaviourManagement;

public partial class ChaseBehaviour : Node, IPrioritizedBehaviour
{
	[Export] AgentModules modules;
	[Export] float chaseSpeed = 100f;

	bool isActive;
	PlayerController player;

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
	}

	public void StopBehavior()
	{
		isActive = false;
		modules.MovementModule.SetTargetVelocity(Vector2.Zero);
	}

	public override void _Process(double delta)
	{
		DebugDraw2D.SetText("Chase", GetPriority().ToString());
		DebugDraw2D.SetText("Active", isActive.ToString());
		if (!isActive) return;
		Vector2 targetDirection = (player.GlobalPosition - modules.AgentBody.GlobalPosition).Normalized();
		modules.MovementModule.SetTargetVelocity(targetDirection * chaseSpeed);
	}
}
