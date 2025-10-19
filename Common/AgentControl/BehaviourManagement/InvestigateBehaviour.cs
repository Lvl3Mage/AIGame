using Game.Common.Managers;
using Godot;

namespace Game.Common.AgentControl.BehaviourManagement;

[GlobalClass]
public partial class InvestigateBehaviour : Node, IPrioritizedBehaviour
{
	[Export] AgentModules modules;
	bool isActive;
	readonly PathFollower investigationPath = new();

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
	}

	public IPrioritizedBehaviour.Priority GetPriority()
	{
		if (investigationPath.PathComplete()) return IPrioritizedBehaviour.Priority.Disabled;
		return IPrioritizedBehaviour.Priority.Low;
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

	[Export] float moveSpeed = 100;
	[Export] float investigateRadius = 40;

	public override void _Process(double delta)
	{
		if (investigationPath.PathComplete()) return;

		if (!isActive) return;
		foreach (Vector2 point in  investigationPath.GetPathPoints()){
			DebugDrawQueue.DebugDrawCircle(point,30,Colors.Aqua);
		}
		MoveAlongPath();
		TryAdvancePath();
	}

	void TryAdvancePath()
	{
		Vector2? target = investigationPath.GetCurrentTarget();
		if (target == null){
			return;
		}

		float distance = (target.Value - modules.AgentBody.GlobalPosition).LengthSquared();
		if (distance <= investigateRadius*investigateRadius){
			investigationPath.AdvancePath();
		}
	}

	void MoveAlongPath()
	{
		Vector2? dir = investigationPath?.GetTargetDirection(modules.AgentBody.GlobalPosition);
		if (dir == null){
			return;
		}

		modules.MovementModule.SetTargetVelocity(dir.Value * moveSpeed);
	}
}