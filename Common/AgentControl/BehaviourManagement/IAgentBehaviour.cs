namespace Game.Common.AgentControl.BehaviourManagement;

public interface IAgentBehaviour
{
	public enum Priority
	{
		Disabled,
		Low,
		Medium,
		High
	}

	Priority GetPriority();
	void StartBehavior();
	void StopBehavior();
}