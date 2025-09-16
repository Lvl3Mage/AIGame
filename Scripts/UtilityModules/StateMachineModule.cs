using Godot;
using System;

[GlobalClass]
/// <summary> Generic finite state machine. Handles State type childs so that only one of them is running at a time. </summary>
public partial class StateMachineModule : Node
{
    /// <summary> Emitted when transitioning to a new state. </summary>
    [Signal]
    public delegate void TransitionedEventHandler(string lastStateName, string stateName);

    /// <summary> State in which the State Machine begins. If no _initialState passed, default will be first _state. </summary>
    [Export]
    public State initialState;

    private State state;

    public string CurrentState { get; private set; }

    public override async void _Ready()
    {
        await ToSignal(Owner, "ready");

        // Script will only run if there is at least one State 
        foreach (Node child in GetChildren())
        {
            if (child is State stateChild)
            {
                stateChild.StateMachine = this;
                initialState ??= stateChild;
            }
        }

        // There is at least one State child
        if (initialState is not null)
        {
            state = initialState;
            CurrentState = initialState.Name;
            state.Enter();
        }
    }

    /// <summary> Transitions to a new state. </summary>
    public void Transition(string targetStateName)
    {
        if (!HasNode(targetStateName))
        {
            GD.PushWarning("Target _state name does not exist.");
            return;
        }

        State previousState = state;

        state.Exit();
        state = GetNode<State>(targetStateName);
        CurrentState = state.Name;
        state.Enter();

        EmitSignal(SignalName.Transitioned, previousState.Name, state.Name);
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        state?.HandleInput(@event);
    }

    public override void _Process(double delta)
    {
        state?.Update((float)delta);
    }

    public override void _PhysicsProcess(double delta)
    {
        state?.PhysicsUpdate((float)delta);
    }
}
