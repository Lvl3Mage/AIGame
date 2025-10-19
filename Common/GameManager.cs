using System;
using System.Threading.Tasks;
using Game.Common.AgentControl.Navigation;
using Godot;

namespace Game.Common;

public partial class GameManager : Node
{
	public static GameManager Instance { get; private set; }
	[Export] public PlayerController Player { get; private set; }
	[Export] public GridNavigation GridNav { get; private set; }
	[Export] public GridDefinition GridDef { get; private set; }
	[Export] Transition screenTransition;

	public override void _Ready()
	{
		if (Instance != null)
		{
			GD.PrintErr("Multiple GameManager instances detected. There should only be one GameManager in the scene tree.");
			QueueFree();
			return;
		}
		Instance = this;

		screenTransition.Scale = Vector2.One;
		screenTransition.Visible = true;
		_ = screenTransition.FadeOut();
	}

	public override void _ExitTree()
	{
		if (Instance == this) Instance = null;
	}

	public void RestartGame() => _ = InitialiseNewGame();

    async Task InitialiseNewGame()
    {
		await screenTransition.FadeIn();
		GetTree().ReloadCurrentScene();
    }
}