using System;
using System.Collections.Generic;
using UnityEngine;

public static class OtherExtentions
{
	public static void SetActive(this Component obj, bool value)
	{
		obj.gameObject.SetActive(value);
	}

	public static string ToStringTime(this float time)
	{
		time = Mathf.Abs(time);
		int num = (int)time / 60;
		int num2 = (int)time - 60 * num;
		int num3 = (int)(100f * (time - (float)(num * 60) - (float)num2));
		return string.Format("{0:00}:{1:00}:{2:00}", num, num2, num3);
	}

	public static string ToStringTime(this float time, string format)
	{
		int num = (int)time / 60;
		int num2 = (int)time - 60 * num;
		int num3 = (int)(1000f * (time - (float)(num * 60) - (float)num2));
		return string.Format(format, num, num2, num3);
	}

	public static V TryGetOrSet<K, V>(this Dictionary<K, V> dict, K key, V defaultValue = default(V))
	{
		if (!dict.ContainsKey(key))
		{
			dict[key] = defaultValue;
		}
		return dict[key];
	}
}
