using System;
using UnityEngine;

[RequireComponent(typeof(CarController))]
public class BodyTilt : MonoBehaviour
{
	private void Awake()
	{
		this.Car = base.GetComponent<CarController>();
	}

	private void Update()
	{
		if (this.Car.CarDirection == 1)
		{
			this.Angle = -this.Car.VelocityAngle * this.AngleVelocityMultiplier;
		}
		else if (this.Car.CarDirection == -1)
		{
			this.Angle = MathExtentions.LoopClamp(this.Car.VelocityAngle + 180f, -180f, 180f) * this.RearAngleVelocityMultiplier;
		}
		else
		{
			this.Angle = 0f;
		}
		this.Angle *= Mathf.Clamp01(this.Car.SpeedInHour / this.MaxTiltOnSpeed);
		this.Angle = Mathf.Clamp(this.Angle, -this.MaxAngle, this.MaxAngle);
		this.Body.localRotation = Quaternion.AngleAxis(this.Angle, Vector3.forward);
	}

	[SerializeField]
	private Transform Body;

	[SerializeField]
	private float MaxAngle = 10f;

	[SerializeField]
	private float AngleVelocityMultiplier = 0.2f;

	[SerializeField]
	private float RearAngleVelocityMultiplier = 0.4f;

	[SerializeField]
	private float MaxTiltOnSpeed = 60f;

	private CarController Car;

	private float Angle;
}
