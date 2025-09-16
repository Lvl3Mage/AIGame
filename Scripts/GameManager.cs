using Godot;
using System.Collections.Generic;
using System;

public partial class GameManager : Node2D
{
	[ExportGroup("References")]
	[Export] public PlayerController Player { get; set; }
	[Export] public GridModule Grid { get; set; }

	public HashSet<Vector2I> Walls { get; set; } = [];

	Vector2I cellA = Vector2I.Zero;
	Vector2I cellB;

	void CellLeftClicked(Vector2I cell)
	{
		// Add wall
		
		if (cell == cellA) return;

		if (Walls.Contains(cell))
		{
			Walls.Remove(cell);
			Grid.ClearCell(cell);
		}
		else
		{
			Walls.Add(cell);
			Grid.ColorCell(cell, Colors.Black, 0.8f);
		}
	}

	public void PaintPath(Vector2I[] path)
	{
		foreach (Vector2I c in path)
			if (!Walls.Contains(c)) Grid.ColorCell(c, Colors.Blue);
	}
	
	public void ClearPath(Vector2I[] path)
	{
		foreach (Vector2I c in path)
			if (!Walls.Contains(c)) Grid.ClearCell(c);		
	}
}
