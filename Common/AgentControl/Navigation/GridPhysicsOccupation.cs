using System;
using System.Collections.Generic;
using System.Linq;
using Game.Common.Managers;
using Game.Common.Utility;
using Godot;

namespace Game.Common.AgentControl.Navigation;

[GlobalClass]
public partial class GridPhysicsOccupation : Node2D, IGridOccupationProvider
{
	[Export] GridDefinition grid;
	[Export] TileMapLayer collisionLayer;


	//Todo moving the tilemap breaks this
	bool ComputeOccupancy(Vector2 position)
	{
		Vector2I cellCoord = GetTileMapCellCoord(position);
		Vector2I tileSize = collisionLayer.TileSet.TileSize;
		Vector2 fracPos = new(
			position.X - cellCoord.X * tileSize.X,
			position.Y - cellCoord.Y * tileSize.Y
		);
		TileData cell = collisionLayer.GetCellTileData(cellCoord);
		if (cell == null){
			return false;
		}

		int polyCount = cell.GetCollisionPolygonsCount(0);
		for (int i = 0; i < polyCount; i++){
			Vector2[] polygon = cell.GetCollisionPolygonPoints(0, i);
			polygon = polygon.Select(p=> p+ (Vector2)tileSize*0.5f).ToArray();
			if (PointInPolygon(polygon, fracPos)){
				return true;
			}
		}

		return false;
	}


	bool PointInPolygon(Vector2[] polygon, Vector2 point)
	{
		int intersections = 0;
		Line horizontalRay = Line.FromTwoPoints(point, point + new Vector2(1, 0));
		for (int j = 0; j < polygon.Length - 1; j++){
			Segment edge = new(polygon[j], polygon[j + 1]);
			// DebugDrawQueue.DebugDrawLine(edge.SegmentStart, edge.SegmentEnd, Colors.Yellow, -1F);
			if (CheckIntersection(edge)){

				intersections++;
			}
		}
		Segment lastEdge = new(polygon[polygon.Length - 1], polygon[0]);

		// DebugDrawQueue.DebugDrawLine(lastEdge.SegmentStart, lastEdge.SegmentEnd, Colors.Yellow, -1F);
		if (CheckIntersection(lastEdge)){
			intersections++;
		}
		// if(intersections % 2 == 1){
		// 	DebugDrawQueue.DebugDrawCircle(point, 3f, Colors.Blue);
		// } else {
		// 	DebugDrawQueue.DebugDrawCircle(point, 3f, Colors.Gray);
		// }
		return intersections % 2 == 1;
		bool CheckIntersection(Segment edge)
		{
			(Vector2? intersection, Line.IntersectionType type) = Line.LineLineIntersection(horizontalRay, edge.Line);
			if (type != Line.IntersectionType.Point) return false;
			if (edge.PointInSegment(intersection.Value)){
				if(horizontalRay.GetTFromPoint(intersection.Value) < 0){
					// DebugDrawQueue.DebugDrawCircle(intersection.Value, 0.5f, Colors.Red);

					return false;
				}
				// DebugDrawQueue.DebugDrawCircle(intersection.Value, 0.5f, Colors.Green);
				return true;
			}

			// DebugDrawQueue.DebugDrawCircle(intersection.Value, 0.5f, Colors.Red);
			return false;

		}
	}
	Vector2I GetTileMapCellCoord(Vector2 globalPositions)
	{
		Vector2 local = collisionLayer.ToLocal(globalPositions);
		Vector2I size = collisionLayer.TileSet.TileSize;
		return new Vector2I(
			Mathf.FloorToInt(local.X / size.X),
			Mathf.FloorToInt(local.Y / size.Y)
		);
	}

	bool[,] collisionMap;
	HashSet<Vector2I> overrides = new();

	public override void _Ready()
	{
		UpdateCollisionMap();
	}

	void UpdateCollisionMap()
	{
		collisionMap = new bool[grid.Width, grid.Height];
		foreach (Vector2I gridCell in grid.GridPositions()){
			bool occupied =
				ComputeOccupancy(grid.GridToWorld(gridCell) + new Vector2(grid.CellSize / 2, grid.CellSize / 2));
			collisionMap[gridCell.X, gridCell.Y] = occupied;
		}
	}


	public override void _Process(double delta)
	{
		// UpdateCollisionMap();
		foreach (Vector2I gridPosition in grid.GridPositions()){
			Color color = IsCellOccupied(gridPosition)
				? new Color(1, 0, 0, 0.5f)
				: new Color(0, 1, 0, 0.5f);
			grid.DrawTile(gridPosition, color);
		}
	}

	public bool IsCellOccupied(Vector2I cell)
	{
		return collisionMap[cell.X, cell.Y] || overrides.Contains(cell);
	}
	public void SetOccupancyOverride(Vector2I cell, bool occupied)
	{
		if(occupied){
			overrides.Add(cell);
		} else {
			overrides.Remove(cell);
		}
	}
}