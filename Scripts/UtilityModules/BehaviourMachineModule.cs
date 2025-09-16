using Godot;
using System.Collections.Generic;
using System.Linq;

[GlobalClass]
public partial class BehaviourMachineModule : Node
{
    private List<IBehaviour> behaviours = [];
    public IBehaviour CurrentBehaviour { get; private set; }

    public override void _Ready()
    {
        // Collect all children that implement IBehaviour
        foreach (Node child in GetChildren())
        {
            if (child is IBehaviour behaviour)
                behaviours.Add(behaviour);
        }

        SelectNewState();
    }

    public override void _Process(double delta)
    {
        SelectNewState();
    }

    private void SelectNewState()
    {
        if (behaviours.Count == 0) return;

        // Pick highest priority (Disabled excluded).
        var next = behaviours
            .Where(s => s.Priority != BehaviourPriority.Disabled)
            .OrderByDescending(s => s.Priority)
            .FirstOrDefault();

        if (next != CurrentBehaviour) CurrentBehaviour = next;
    }
}
