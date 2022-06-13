using System;
using System.Collections.Generic;
using UnityEngine;

public static class ArrayExtensions
{
	public static T GetSafe<T>(this T[] array, int index)
	{
		if (array == null || index < 0 || index > array.Length)
		{
			return default(T);
		}
		return array[index];
	}

	public static T RandomChoice<T>(this List<T> list)
	{
		if (list == null || list.Count == 0)
		{
			Debug.LogError("List is null or empty");
			return default(T);
		}
		return list[Random.Range(0, list.Count)];
	}
}
