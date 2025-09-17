public interface IBehaviour
{
    BehaviourPriority Priority { get; set; }
}

public enum BehaviourPriority
{
    Disabled,
    Low,
    Medium,
    High
}