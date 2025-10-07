using System;
using Godot;

namespace Game.Common.Utility;

public struct Segment
{
	//todo implement line and segment intersection
	public readonly Line Line;
	public readonly float SegmentStartT;
	public readonly float SegmentEndT;
	public readonly Vector2 SegmentStart;
	public readonly Vector2 SegmentEnd;


	public Segment(Vector2 start, Vector2 end)
	{
		Line = Line.FromTwoPoints(start, end);
		SegmentStart = start;
		SegmentEnd = end;
		SegmentStartT = 0f;
		SegmentEndT = (end - start).Length();
	}

	public Segment(Line line, float segmentStartT, float segmentEndT)
	{
		Line = line;
		SegmentStartT = segmentStartT;
		SegmentEndT = segmentEndT;
		SegmentStart = line.GetPointFromT(segmentStartT);
		SegmentEnd = line.GetPointFromT(segmentEndT);
	}


	public bool PointInSegment(Vector2 point)
	{
		float pointT = Line.GetTFromPoint(point);
		return pointT >= SegmentStartT && pointT <= SegmentEndT;
	}
}

public struct Line
{
	public readonly Vector2 Origin;
	public readonly Vector2 Direction;
	public readonly float Inclination;
	public readonly bool Vertical;
	public readonly bool Horizontal;
	public readonly float? YIntercept;
	public readonly float? XIntercept;

	public Vector2 GetPointFromT(float t)
	{
		return Origin + Direction * t;
	}

	public readonly float GetTFromPoint(Vector2 point)
	{
		return (point - Origin).Dot(Direction);
	}

	public static Line FromTwoPoints(Vector2 origin, Vector2 control)
	{
		return new Line(origin, control);
	}

	public static Line FromPointAndDirection(Vector2 point, Vector2 direction)
	{
		return new Line(point, point + direction);
	}

	public static Line FromPointAndInclination(Vector2 point, float inclination)
	{
		if (float.IsInfinity(inclination)){
			return new Line(point, point + Vector2.Up);
		}

		return new Line(point, point + new Vector2(1f, inclination));
	}

	public static bool operator ==(Line self, Line other)
	{
		return self.Direction == other.Direction;
	}

	public static bool operator !=(Line self, Line other)
	{
		return !(self == other);
	}

	public Line(Vector2 origin, Vector2 control)
	{
		if (origin == control){
			throw new ArgumentException("A line cannot be defined by two identical points.");
		}

		Origin = origin;
		Inclination = (control.Y - origin.Y) / (control.X - origin.X);
		Vertical = Mathf.IsEqualApprox(control.X, origin.X);
		Horizontal = Mathf.IsEqualApprox(control.Y, origin.Y);
		Direction = (control - origin).Normalized();
		YIntercept = Vertical ? null : origin.Y - Inclination * origin.X;
		XIntercept = Horizontal ? null : origin.X - 1f / Inclination * origin.Y;
	}


	public enum IntersectionType
	{
		Point,
		Overlap,
		Parallel
	}

	public static (Vector2? intersectionPoint, IntersectionType intersectionType) LineLineIntersection(Line line1,
		Line line2)
	{
		if (line1 == line2){
			return (null, IntersectionType.Overlap);
		}

		float det = line1.Direction.X * line2.Direction.Y - line1.Direction.Y * line2.Direction.X;
		if (Mathf.IsZeroApprox(det)){
			return (null, IntersectionType.Parallel);
		}

		Vector2 delta = line2.Origin - line1.Origin;
		float t = (delta.X * line2.Direction.Y - delta.Y * line2.Direction.X) / det;
		Vector2 intersectionPoint = line1.GetPointFromT(t);
		return (intersectionPoint, IntersectionType.Point);
	}
}