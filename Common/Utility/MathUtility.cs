using System;
using Godot;

namespace Game.Common.Utility;

public static class MathUtility
{
	public static float ComputeLerpWeight(float smoothingSpeed, float delta)
	{
		float weight = 1f - Mathf.Exp(-smoothingSpeed * delta);
		return weight;
	}

	
}