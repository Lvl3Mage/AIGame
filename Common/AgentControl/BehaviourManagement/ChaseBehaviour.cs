using Godot;

namespace Game.Common.AgentControl.BehaviourManagement;

public partial class ChaseBehaviour : Node, IAgentBehaviour
{
	[Export] private AgentBlackboard _blackboard;

	public IAgentBehaviour.Priority CurrentPriority { get; private set; } = IAgentBehaviour.Priority.Disabled;

	private bool _isActive = false;
	private PlayerController _player;

	public override void _Ready()
	{
		_player = GameManager.Instance.Player;
	}

	public IAgentBehaviour.Priority GetPriority()
	{
		// La lógica de decisión de prioridad está aquí
		CurrentPriority = _blackboard.CanSeePlayer ? IAgentBehaviour.Priority.High : IAgentBehaviour.Priority.Disabled;
		return CurrentPriority;
	}

	public void StartBehavior()
	{
		if (_isActive) return;
		GD.Print("Agente: ¡Jugador detectado! Iniciando Persecución.");
		_isActive = true;
	}

	public void StopBehavior()
	{
		if (!_isActive) return;
		GD.Print("Agente: Jugador perdido. Finalizando Persecución.");
		_isActive = false;
		_blackboard.MovementModule.Stop();
		// Al detenerse, activamos la investigación en la última posición conocida.
		// El BehaviourTreeController se encargará de activar el InvestigateBehaviour
		// si su prioridad sube a Medium gracias a esto.
	}

	public override void _Process(double delta)
	{
		if (!_isActive) return;

		// Mientras está activo, recalcula constantemente la ruta hacia el jugador
		Vector2I start = (Vector2I)_blackboard.AgentBody.GlobalPosition;
		Vector2I end = (Vector2I)_player.GlobalPosition;

		var path = GameManager.Instance.GridNav?.FindPath(start, end);
		_blackboard.MovementModule.SetPath(path);
	}
}
