using System;
using UnityEngine;

[Serializable]
public struct Layer
{
	public static implicit operator int(Layer layer)
	{
		return layer.value;
	}

	[SerializeField]
	private int value;
}
