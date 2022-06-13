using System;
using UnityEngine;
using UnityEngine.UI;

public static class UIExtentions
{
	public static void SetAlpha(this Graphic g, float newAlpha)
	{
		Color color = g.color;
		color.a = newAlpha;
		g.color = color;
	}
}
