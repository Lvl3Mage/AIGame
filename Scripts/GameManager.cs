using System;
using System.Linq;
using System.Threading.Tasks;
using Game.Common.AgentControl.Navigation;
using Game.Common.Modules;
using Game.Common.Utility;
using Godot;

namespace Game.Common;

public partial class GameManager : Node
{
	[Export] public Node2D GameLevel { get; private set; }
	[Export] float transitionDelay = 0.6f;
	[Export] float bgMusicVolume = 0.5f;
	[Export] float winMusicVolume = 0.5f;
	[Export] PackedScene winScreenScene;

	public static GameManager Instance { get; private set; }
	public PlayerController Player { get; private set; }
	public DynamicCamera Camera { get; private set; }
	public TimeFreezerModule TimeFreezer { get; private set; }
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
		Camera = root.GetAllChildrenOfType<DynamicCamera>().First();
		TimeFreezer = root.GetAllChildrenOfType<TimeFreezerModule>().First();
		GridNav = root.GetAllChildrenOfType<GridNavigation>().First();
		GridDef = root.GetAllChildrenOfType<GridDefinition>().First();
		screenTransition = root.GetAllChildrenOfType<Transition>().First();
	}

	public override async void _Ready()
	{
		Callable.From(() => AudioManager.PlayAudio2D(SoundLibrary.Instance.BackgroundMusic, Player, bgMusicVolume)).CallDeferred();
		Player.LockMovement = true;
		screenTransition.Scale = Vector2.One;
		screenTransition.Visible = true;
		await DelayedFadeOut();
		Player.LockMovement = false;
	}

	public override void _ExitTree()
	{
		if (Instance == this) Instance = null;
	}

	public void RestartGame() => _ = InitialiseNewGame();

	public async Task WinGame()
    {
		Player.HasWon = true;
		Player.LockMovement = true;
		await screenTransition.FadeIn();
		winScreenScene.InstantiateUnderAs<CanvasLayer>(GetTree().CurrentScene);
		GameLevel.QueueFree();
		_ = DelayedFadeOut();
		await Task.Delay((int)(transitionDelay * 1000));
		await Task.Delay((int)screenTransition.TransitionTime / 2 * 1000); // Await until halfway through fade out
		AudioManager.PlayAudio(SoundLibrary.Instance.VictoryMusic, winMusicVolume);
    }

	async Task InitialiseNewGame()
	{
		await screenTransition.FadeIn();
		GetTree().ReloadCurrentScene();
	}

	async Task DelayedFadeOut()
	{
		await Task.Delay((int)(transitionDelay * 1000));
		await screenTransition.FadeOut();
	}
}