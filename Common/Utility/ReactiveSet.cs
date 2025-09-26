using System;
using System.Collections.Generic;
using System.Linq;

namespace Game.Common.Utility;

public class ReactiveSet<T>
{
	public record SetChange(IReadOnlyList<T> Added, IReadOnlyList<T> Removed);
	public event Action<SetChange> Changed;

	readonly HashSet<T> set =[];
	public bool TryAdd(T item)
	{
		if (!set.Add(item)) return false;
		Changed?.Invoke(new SetChange([item],[]));
		return true;
	}
	public bool TryRemove(T item)
	{
		if (!set.Remove(item)) return false;
		Changed?.Invoke(new SetChange([], [item]));
		return true;
	}
	public int Count => set.Count;
	public IEnumerable<T> Values => set;
	public bool Contains(T item) => set.Contains(item);
	public void Clear()
	{
		T[] removed = set.ToArray();
		set.Clear();
		Changed?.Invoke(new SetChange([],removed));
	}

	public void AddRange(IEnumerable<T> items)
	{
		var added = items.Where(set.Add).ToArray();
		if (added.Length > 0)
			Changed?.Invoke(new SetChange(added, []));
	}

	public void RemoveRange(IEnumerable<T> items)
	{
		var removed = items.Where(set.Remove).ToArray();
		if (removed.Length > 0)
			Changed?.Invoke(new SetChange([], removed));
	}

}