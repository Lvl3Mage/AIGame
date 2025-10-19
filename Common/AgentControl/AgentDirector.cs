using Godot;

namespace Game.Common.AgentControl;

public partial class AgentDirector : Node
{

	public class InformationSignal
	{
		public enum InformationType
		{
			FoundPlayer,
			LostPlayer,
			
		}
	}
}