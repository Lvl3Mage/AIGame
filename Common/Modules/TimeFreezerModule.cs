using Godot;

namespace Game.Common.Modules;

//TODO: Joan pls make this a static class why does this need to be a node I'm gonna cry - Karl
/// <summary> Freezes or slows down time for a short duration. </summary>
[GlobalClass]
public partial class TimeFreezerModule : Node
{
    /// <summary> Freezes the game by setting the time scale to the given value for a set duration. </summary>
    /// <param name="timeScale">New time scale (0 = freeze).</param>
    /// <param name="duration">Duration in seconds (real time, not affected by time scale).</param>
    public async void FrameFreeze(float timeScale, float duration)
    {
        float previousScale = (float)Engine.TimeScale;

        Engine.TimeScale = timeScale;

        // Create a timer that ignores the global time scale
        var timer = GetTree().CreateTimer(duration, processAlways: true, processInPhysics: false, ignoreTimeScale: true);

        await ToSignal(timer, SceneTreeTimer.SignalName.Timeout);

        Engine.TimeScale = previousScale;
    }
}