namespace Game.Common.AgentControl.BehaviourManagement;

public interface IPrioritizedBehaviour : IAgentBehaviour
{
	public enum Priority
	{
		Disabled,
		Background,
		Low,
		Normal,
		Important,
		Critical
	}

/*
 * public enum Priority
{
    Obliterated,
    Trivial,
    BarelyNoticeable,
    MildlyRelevant,
    SomewhatUrgent,
    ModeratelyDire,
    QuiteSerious,
    CriticallyImportant,
    AlarminglyPressing,
    CatastrophicallyHigh,
    ApocalypticallySevere,
    UniverseEnding,
    GodTier
}
 */

	Priority GetPriority();
}