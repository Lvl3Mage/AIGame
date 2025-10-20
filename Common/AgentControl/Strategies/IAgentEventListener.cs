namespace Game.Common.AgentControl.Strategies;

public interface IAgentEventListener<in T>
{
	public void OnEvent(T agentEvent);

}