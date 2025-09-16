using Godot;
using System;

[GlobalClass]
/// <summary> Virtual base class for all states. DO NOT USE BY ITSELF, instead, extend this class on another script. </summary>
public abstract partial class State : Node
{
    /// <summary> The state machine node will set it. STATE MACHINE SCENE HAS TO BE PARENT OF ALL STATES </summary>
    public Node StateMachine { get; set; } = null;

    public override void _Ready()
    {
        // Finite State Machine parent
        if (GetParent() is not StateMachineModule)
            GD.PushError("State's parent must be a StateMachineComponent.");
    }

    /// <summary> Virtual function. Receives events from the "_unhandled_input()" callback. </summary>
    public virtual void HandleInput(InputEvent @event) { }

    /// <summary> Virtual function. Corresponds to the "_process()" callback. </summary>
    public virtual void Update(float delta) { }

    /// <summary> Virtual function. Corresponds to the "_physics_process()" callback. </summary>
    public virtual void PhysicsUpdate(float delta) { }

    /// <summary> Virtual function. Called by the state machine upon changing the active state. </summary>
    public virtual void Enter() { }

    /// <summary> Virtual function. Called by the state machine before changing the active state. Use this function to clean up the state. </summary>
    public virtual void Exit() { }
}
