using System;
using System.Collections.Generic;
using Godot;

namespace Game.Common.Managers;

public partial class DebugDrawQueue : Node2D
{
	static DebugDrawQueue instance;
	public override void _EnterTree()
	{
		if (instance != null)
		{
			GD.PushWarning("Multiple DebugDrawManager instances detected. There should only be one DebugDrawManager in the scene.");
			QueueFree();
		}
		instance = this;
	}

	readonly List<Action> drawQueue =[];
	public static void DebugDrawCircle(Vector2 center, float radius, Color color, bool filled = true, float width = -1f, bool antialiased = false)
	{
		instance?.drawQueue.Add(() =>
		{
			instance.DrawCircle(center, radius, color, filled, width, antialiased);
		});
	}
	public static void DebugDrawLine(Vector2 from, Vector2 direction, Color color, float width = 1f, bool antialiased = false)
	{
		instance?.drawQueue.Add(() =>
		{
			instance.DrawLine(from, direction, color, width, antialiased);
		});
	}
	public static void DebugDrawRect(Rect2 rect, Color color, bool filled = false, float width = 1f, bool antialiased = false)
	{
		instance?.drawQueue.Add(() =>
		{
			instance.DrawRect(rect, color, filled, width, antialiased);
		});
	}

	public override void _Draw()
	{
		foreach (var action in drawQueue)
		{
			action();
		}
		drawQueue.Clear();
	}
	public override void _Process(double delta)
	{
		GlobalPosition = Vector2.Zero;
		Rotation = 0f;
		QueueRedraw();
	}
}