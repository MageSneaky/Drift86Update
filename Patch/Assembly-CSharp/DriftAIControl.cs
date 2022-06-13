using System;
using GameBalance;
using UnityEngine;

public class DriftAIControl : AIControlBase
{
	private float MaxSpeed
	{
		get
		{
			return this.AiConfig.MaxSpeed;
		}
	}

	private float MinSpeed
	{
		get
		{
			return this.AiConfig.MinSpeed;
		}
	}

	private float AccelSensitivity
	{
		get
		{
			return this.AiConfig.AccelSensitivity;
		}
	}

	private float BrakeSensitivity
	{
		get
		{
			return this.AiConfig.BrakeSensitivity;
		}
	}

	private float ReverceWaitTime
	{
		get
		{
			return this.AiConfig.ReverceWaitTime;
		}
	}

	private float ReverceTime
	{
		get
		{
			return this.AiConfig.ReverceTime;
		}
	}

	private float BetweenReverceTimeForReset
	{
		get
		{
			return this.AiConfig.BetweenReverceTimeForReset;
		}
	}

	private float LookAheadForTargetOffset1
	{
		get
		{
			return this.AiConfig.OffsetToFirstTargetPoint;
		}
	}

	private float LookAheadForTargetFactor1
	{
		get
		{
			return this.AiConfig.SpeedFactorToFirstTargetPoint;
		}
	}

	private float LookAheadForTargetOffset2
	{
		get
		{
			return this.AiConfig.OffsetToSecondTargetPoint;
		}
	}

	private float LookAheadForTargetFactor2
	{
		get
		{
			return this.AiConfig.SpeedFactorToSecondTargetPoint;
		}
	}

	private float LookAngleSppedFactor
	{
		get
		{
			return this.AiConfig.LookAngleSppedFactor;
		}
	}

	private float SetSteerAngleSensitivity
	{
		get
		{
			return this.AiConfig.SetSteerAngleSensitivity;
		}
	}

	public float TargetDist { get; private set; }

	public Vector3 TargetPoint { get; private set; }

	public WaypointCircuit.RoutePoint TargetPoint1 { get; private set; }

	public WaypointCircuit.RoutePoint TargetPoint2 { get; private set; }

	private PositioningCar PositioningCar
	{
		get
		{
			return this.Car.PositioningCar;
		}
	}

	private Rigidbody RB
	{
		get
		{
			return this.Car.RB;
		}
	}

	private WaypointCircuit Circuit
	{
		get
		{
			return PositioningSystem.PositioningAndAiPath;
		}
	}

	private void Start()
	{
		this.Car = base.GetComponent<CarController>();
		this.AiConfig = WorldLoading.RegimeSettings.GetAiConfig;
	}

	private void FixedUpdate()
	{
		if (!GameController.RaceIsStarted)
		{
			return;
		}
		if (this.Reverse)
		{
			this.ReverseMove();
			return;
		}
		this.ForwardMove();
	}

	private void ForwardMove()
	{
		(this.Car.VelocityAngle.Abs() / 55f).Clamp();
		this.TargetPoint1 = this.Circuit.GetRoutePoint(this.PositioningCar.ProgressDistance + this.LookAheadForTargetOffset1 + this.LookAheadForTargetFactor1 * this.Car.CurrentSpeed);
		this.TargetPoint2 = this.Circuit.GetRoutePoint(this.PositioningCar.ProgressDistance + this.LookAheadForTargetOffset2 + this.LookAheadForTargetFactor2 * this.Car.CurrentSpeed);
		this.TargetPoint = (this.TargetPoint1.position + this.TargetPoint2.position) * 0.5f;
		float num = Vector3.SignedAngle(Vector3.forward, base.transform.InverseTransformPoint(this.TargetPoint), Vector3.up).AbsClamp(0f, this.LookAngleSppedFactor);
		float num2;
		if (base.HasLimit)
		{
			num2 = base.SpeedLimit;
		}
		else
		{
			num2 = (1f - num / this.LookAngleSppedFactor).AbsClamp();
			num2 = num2 * (this.MaxSpeed - this.MinSpeed) + this.MinSpeed;
			num2 = num2.Clamp(this.MinSpeed, this.MaxSpeed);
		}
		float num3 = (num2 < this.Car.SpeedInHour) ? this.BrakeSensitivity : this.AccelSensitivity;
		if (base.NeedBrake)
		{
			base.Vertical = ((num2 - this.Car.SpeedInHour) * num3).Clamp(-1f, 1f);
		}
		else
		{
			base.Vertical = ((num2 - this.Car.SpeedInHour) * num3).Clamp(0f, 1f);
		}
		Vector3 vector = base.transform.InverseTransformPoint(this.TargetPoint);
		float num4 = Mathf.Atan2(vector.x, vector.z) * 57.29578f;
		base.Horizontal = Mathf.MoveTowards(base.Horizontal, (num4 / this.Car.GetCarConfig.MaxSteerAngle).Clamp(-1f, 1f), Time.deltaTime * this.SetSteerAngleSensitivity);
		this.Car.UpdateControls(base.Horizontal, base.Vertical, false);
		float num5 = Mathf.Abs(this.Car.SpeedInHour - this.PrevSpeed);
		if (base.Vertical <= 0.1f || num5 >= 1f || this.Car.SpeedInHour >= 10f)
		{
			this.ReverseTimer = 0f;
			return;
		}
		if (this.ReverseTimer < this.ReverceWaitTime)
		{
			this.ReverseTimer += Time.fixedDeltaTime;
			return;
		}
		if (Time.time - this.LastReverceTime <= this.BetweenReverceTimeForReset)
		{
			base.Horizontal = 0f;
			base.Vertical = 0f;
			this.Car.ResetPosition();
			this.ReverseTimer = 0f;
			return;
		}
		base.Horizontal = -base.Horizontal;
		base.Vertical = -base.Vertical;
		this.ReverseTimer = 0f;
		this.Reverse = true;
	}

	private void ReverseMove()
	{
		Mathf.Abs(this.Car.SpeedInHour - this.PrevSpeed);
		if (this.ReverseTimer < this.ReverceTime)
		{
			this.ReverseTimer += Time.fixedDeltaTime;
			this.Car.UpdateControls(base.Horizontal, base.Vertical, false);
			return;
		}
		this.LastReverceTime = Time.time;
		this.ReverseTimer = 0f;
		this.Reverse = false;
	}

	private void OnDrawGizmosSelected()
	{
		if (Application.isPlaying && base.enabled)
		{
			Gizmos.color = Color.green;
			Gizmos.DrawLine(base.transform.position, this.TargetPoint);
			Gizmos.DrawWireSphere(this.TargetPoint, 0.5f);
			Gizmos.color = Color.yellow;
			Gizmos.DrawWireSphere(this.TargetPoint1.position, 0.5f);
			Gizmos.DrawWireSphere(this.TargetPoint2.position, 0.5f);
		}
	}

	private RegimeSettings.AiConfig AiConfig;

	private CarController Car;

	private bool Reverse;

	private float ReverseTimer;

	private float PrevSpeed;

	private float LastReverceTime;
}
