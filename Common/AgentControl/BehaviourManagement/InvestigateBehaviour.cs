using Game.Common.Managers;
using Godot;

namespace Game.Common.AgentControl.BehaviourManagement;

[GlobalClass]
public partial class InvestigateBehaviour : Node, IAgentBehaviour
{
	[Export] AgentBlackboard blackboard;
	bool isActive;
	readonly PathFollower investigationPath = new();

	public override void _Ready()
	{
		blackboard.PlayerVisibilityChanged += OnPlayerVisibilityChanged;
	}

	void OnPlayerVisibilityChanged()
	{
		investigationPath.SetPoints(GameManager.Instance.GridNav.GetPathBetween(
				blackboard.AgentBody.GlobalPosition,
				blackboard.LastVisiblePlayerPosition
			)
		);
	}

	public IAgentBehaviour.Priority GetPriority()
	{
		if (investigationPath.PathComplete()) return IAgentBehaviour.Priority.Disabled;
		return IAgentBehaviour.Priority.Low;
	}

	public void StartBehavior()
	{
		isActive = true;
	}

	public void StopBehavior()
	{
		isActive = false;
		blackboard.MovementModule.SetTargetVelocity(Vector2.Zero);
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

		float distance = (target.Value - blackboard.AgentBody.GlobalPosition).LengthSquared();
		if (distance <= investigateRadius*investigateRadius){
			investigationPath.AdvancePath();
		}
	}

	void MoveAlongPath()
	{
		Vector2? dir = investigationPath?.GetTargetDirection(blackboard.AgentBody.GlobalPosition);
		if (dir == null){
			return;
		}

		blackboard.MovementModule.SetTargetVelocity(dir.Value * moveSpeed);
	}
}