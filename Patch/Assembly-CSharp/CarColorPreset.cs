using System;
using UnityEngine;

[Serializable]
public struct CarColorPreset
{
	public Color Color
	{
		get
		{
			return this.m_Color;
		}
	}

	public float Smoothness
	{
		get
		{
			return this.m_Smoothness;
		}
	}

	[SerializeField]
	private Color m_Color;

	[SerializeField]
	[Range(0f, 1f)]
	private float m_Smoothness;
}
