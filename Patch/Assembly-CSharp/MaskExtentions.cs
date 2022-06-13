using System;
using UnityEngine;

public static class MaskExtentions
{
	public static bool LayerInMask(this LayerMask mask, int layer)
	{
		return (mask.value & 1 << layer) != 0;
	}
}
