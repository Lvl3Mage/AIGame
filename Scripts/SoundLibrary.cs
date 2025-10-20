using Godot;

namespace Game.Common;

public partial class SoundLibrary : Node
{
	[Export] public AudioStream BackgroundMusic { get; private set; }
	[Export] public AudioStream VictoryMusic { get; private set; }
	[Export] public AudioStream[] AlertadorIdle { get; private set; }
	[Export] public AudioStream AlertadorAlert { get; private set; }
	[Export] public AudioStream[] BossIdle { get; private set; }
	[Export] public AudioStream BossRoar { get; private set; }
	[Export] public AudioStream KeyCollect { get; private set; }
	[Export] public AudioStream KeyInsert { get; private set; }
	[Export] public AudioStream DoorOpen { get; private set; }
	[Export] public AudioStream[] PlayerSteps { get; private set; }
	[Export] public AudioStream PlayerDeath { get; private set; }
	[Export] public AudioStream Transition { get; private set; }

	public static SoundLibrary Instance { get; private set; }

	public override void _EnterTree()
	{
		if (Instance != null)
		{
			GD.PrintErr("Multiple Sounds instances detected. There should only be one GameManager in the scene tree.");
			QueueFree();
			return;
		}
		Instance = this;
	}

	public override void _ExitTree()
	{
		if (Instance == this) Instance = null;
	}
}