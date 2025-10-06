using Game.Common.AgentControl.Navigation;
using Game.Common.Modules;
using Godot;

namespace Game.Common.AgentControl.BehaviourManagement;

/// <remarks>
///	Todo: Refactor this to use IAgentBehaviour properly and to use the <see cref="GridNavigation"/>
/// </remarks>
public partial class ChaseBehaviour : Node, IAgentBehaviour
{
	[Export] TopdownMovementModule movementModule;

	public IAgentBehaviour.Priority Priority { get; set; } = IAgentBehaviour.Priority.High;
	GameManager manager;
	GridNavigation gridNav;
	GridNavigation gridDef;
	Node2D target, root;
	Vector2I[] path = [];
	Vector2I nextPos;
	int chaseIndex = 1;

	public override void _Ready()
	{
		gridNav = GameManager.Instance.GridNav;
		gridDef = GameManager.Instance.GridDef;
		target = GameManager.Instance.Player;
	}
	
	   public override void _Process(double delta)
	   {
		UpdateChasePath();
	   }
	
	   void UpdateChasePath()
	   {
	       if (path.Length > chaseIndex)
	       {
				gridDef.Cell
	        	// Advance one cell in the path
				CellPosition = path[chaseIndex];
	       }
	
	       float width = mesh.CellSize / 2;
	       Vector2I currentCell = CellPosition;
	       Vector2I targetCell = mesh.WorldToGrid(target.Position + new Vector2(width, width));
	
	       manager.ClearPath(path);
	       path = mesh.GetShortestPathBFS(currentCell, targetCell, manager.Walls, true);
	       manager.PaintPath(path);
	   }

    public IAgentBehaviour.Priority GetPriority()
    {
        throw new System.NotImplementedException();
    }

    public void StartBehavior()
    {
        throw new System.NotImplementedException();
    }

    public void StopBehavior()
    {
        throw new System.NotImplementedException();
    }
}