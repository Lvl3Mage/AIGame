#nullable enable
using System.Collections.Generic;
using Godot;

namespace Game.Common.Utility;

public static class GodotExtensions
{
	public static SceneTree? TryGetSceneTree()
	{
		return Engine.GetMainLoop() as SceneTree;
	}
	public static T? TryGetParentOfType<T>(this Node node) where T : Node
	{
		Node parent = node.GetParent();
		if (parent is T tParent)
		{
			return tParent;
		}
		return null;
	}
	public static T InstantiateUnderAs<T>(this PackedScene scene, Node parent) where T : Node
	{
		var instance = scene.Instantiate<T>();
		parent.AddChild(instance);
		return instance;
	}
	public static IEnumerable<T> GetChildrenOfType<T>(this Node node) where T : Node
	{
		foreach (Node child in node.GetChildren())
		{
			if (child is T tChild)
				yield return tChild;
		}
	}
	public static IEnumerable<T> GetAllChildrenOfType<T>(this Node node)
	{
		foreach (Node child in node.GetChildren())
		{
			if (child is T tChild)
				yield return tChild;

			foreach (T grandChild in child.GetAllChildrenOfType<T>())
				yield return grandChild;
		}
	}
}