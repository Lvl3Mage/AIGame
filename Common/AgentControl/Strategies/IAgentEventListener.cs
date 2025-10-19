namespace Game.Common.AgentControl.Strategies;

public interface IAgentEventListener<in T> where T : IAgentEvent
{
	public void OnEvent(T agentEvent);

}