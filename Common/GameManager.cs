using System;
using System.Linq;
using System.Threading.Tasks;
using Game.Common.AgentControl.Navigation;
using Game.Common.Utility;
using Godot;

namespace Game.Common;

public partial class GameManager : Node
{
	[Export] public Node2D GameLevel { get; private set; }
	[Export] float transitionDelay = 0.6f;
	[Export] PackedScene winScreenScene;

	public static GameManager Instance { get; private set; }
	public PlayerController Player { get; private set; }
	public GridNavigation GridNav { get; private set; }
	public GridDefinition GridDef { get; private set; }
	Transition screenTransition;
	public override void _EnterTree()
	{
		if (Instance != null)
		{
			GD.PrintErr("Multiple GameManager instances detected. There should only be one GameManager in the scene tree.");
			QueueFree();
			return;
		}
		Instance = this;

		Node root = GetTree().Root;
		Player = root.GetAllChildrenOfType<PlayerController>().First();
		GridNav = root.GetAllChildrenOfType<GridNavigation>().First();
		GridDef = root.GetAllChildrenOfType<GridDefinition>().First();
		screenTransition = root.GetAllChildrenOfType<Transition>().First();
	}

	public override void _Ready()
	{
		screenTransition.Scale = Vector2.One;
		screenTransition.Visible = true;
		DelayFadeOut();
	}

	public override void _ExitTree()
	{
		if (Instance == this) Instance = null;
	}

	public void RestartGame() => _ = InitialiseNewGame();

	public async Task WinGame()
    {
		Player.hasWon = true;
		Player.LockMovement = true;
		await Task.Delay(1500);
		await screenTransition.FadeIn();
		winScreenScene.InstantiateUnderAs<Control>(GetTree().CurrentScene);
		GameLevel.QueueFree();
    }

	async Task InitialiseNewGame()
	{
		await screenTransition.FadeIn();
		GetTree().ReloadCurrentScene();
	}

	async void DelayFadeOut()
	{
		await Task.Delay((int)(transitionDelay * 1000));
		await screenTransition.FadeOut();
	}
}