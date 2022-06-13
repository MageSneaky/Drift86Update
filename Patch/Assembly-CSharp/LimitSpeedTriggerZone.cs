using System;
using UnityEngine;

public class LimitSpeedTriggerZone : MonoBehaviour
{
	public float LimitSpeed
	{
		get
		{
			return this.m_LimitSpeed;
		}
	}

	public bool NeedBrake
	{
		get
		{
			return this.m_NeedBrake;
		}
	}

	[SerializeField]
	private float m_LimitSpeed = 50f;

	[SerializeField]
	private bool m_NeedBrake;
}
