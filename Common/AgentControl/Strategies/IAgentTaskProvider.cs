using System;
using System.Collections.Generic;

namespace Game.Common.AgentControl.Strategies;

public interface IAgentTaskProvider
{
	public IEnumerable<AgentTask> GetTasks();
}