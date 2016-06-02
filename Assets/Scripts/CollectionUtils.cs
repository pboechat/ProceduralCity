using System;
using System.Collections;
using System.Collections.Generic;

public class CollectionUtils
{
	private CollectionUtils ()
	{
	}
	
	public static ICollection<T> Disjoint<T> (ICollection<T> collection, ICollection<T> items)
	{
		List<T> list = new List<T> ();
		foreach (T item in collection) {
			if (items.Contains (item)) {
				continue;
			}
			list.Add (item);
		}
		return list;
	}
}