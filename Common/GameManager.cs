using Game.Common.AgentControl.Navigation;
using Godot;

namespace Game.Common;

public partial class GameManager : Node
{
	public static GameManager Instance { get; private set; } = null;
	[Export] public PlayerController Player { get; private set; }
	[Export] public GridNavigation GridNav { get; private set; }
	[Export] public GridNavigation GridDef { get; private set; }

	public override void _Ready()
	{
		if (Instance != null)
		{
			GD.PrintErr("Multiple GameManager instances detected. There should only be one GameManager in the scene tree.");
			QueueFree();
			return;
		}
		Instance = this;
	}
}