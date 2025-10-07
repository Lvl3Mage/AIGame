using Godot;

namespace Game.Common.Modules;

[GlobalClass]
public partial class CharacterAnimationModule : Node2D
{

    //sety = abs(sin(tiempo * frecuencia)) * amplitud

    [Export(PropertyHint.Range, "0.0, 10.0")]
    public float AmplitudeMultiplier { get; set; } = 1.0f;
    [Export(PropertyHint.Range, "0.1, 50.0")]
    public float Frequency { get; set; } = 10.0f;

    Vector2 velocity = Vector2.Zero;

    public void SetVelocity(Vector2 newVelocity)
    {
        velocity = newVelocity;
    }


    public override void _Process(double delta)
    {
        float time = Time.GetTicksUsec() / 1_000_000.0f;
        //float amplitude = characterBody.Velocity * AmplitudeMultiplier;
        float offsetY = Mathf.Abs(Mathf.Sin(time * Frequency)) * AmplitudeMultiplier;
        this.Position = new Vector2(this.Position.X, this.Position.Y - offsetY);
    }
}