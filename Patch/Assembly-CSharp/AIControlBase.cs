using System;
using UnityEngine;

[RequireComponent(typeof(CarController))]
public class AIControlBase : MonoBehaviour, ICarControl
{
	public float Vertical { get; protected set; }

	public float Horizontal { get; protected set; }

	public bool Brake { get; protected set; }

	public bool HasLimit
	{
		get
		{
			return this.CurrentLimitZone != null;
		}
	}

	private protected float SpeedLimit { protected get; private set; }

	private protected bool NeedBrake { protected get; private set; }

	public void OnTriggerEnter(Collider other)
	{
		LimitSpeedTriggerZone component = other.GetComponent<LimitSpeedTriggerZone>();
		if (component != null)
		{
			this.CurrentLimitZone = component;
			this.SpeedLimit = this.CurrentLimitZone.LimitSpeed;
			this.NeedBrake = this.CurrentLimitZone.NeedBrake;
		}
	}

	public void OnTriggerExit(Collider other)
	{
		if (this.HasLimit && other.gameObject == this.CurrentLimitZone.gameObject)
		{
			this.CurrentLimitZone = null;
			this.SpeedLimit = 0f;
			this.NeedBrake = false;
		}
	}

	private LimitSpeedTriggerZone CurrentLimitZone;
}
