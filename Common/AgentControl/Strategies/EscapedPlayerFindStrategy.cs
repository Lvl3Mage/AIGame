using System.Collections.Generic;
using Godot;

namespace Game.Common.AgentControl.Strategies;

public partial class EscapedPlayerFindStrategy : Node, IAgentTaskProvider, IAgentEventListener<PlayerLostEvent>
{
	public IEnumerable<AgentTask> GetTasks()
	{
		throw new System.NotImplementedException();
	}

	public void OnEvent(PlayerLostEvent agentEvent)
	{
		throw new System.NotImplementedException();
	}
}