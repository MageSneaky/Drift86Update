using System;
using UnityEngine;

public class AccelerometerControlUI : ControlUI, IUserControl
{
	private bool AccelerationPressed
	{
		get
		{
			return this.AccelerationButton.ButtonIsPressed;
		}
	}

	private bool DecelerationPressed
	{
		get
		{
			return this.DecelerationButton.ButtonIsPressed;
		}
	}

	public bool ControlInUse
	{
		get
		{
			return SystemInfo.supportsAccelerometer;
		}
	}

	public float GetHorizontalAxis { get; private set; }

	public float GetVerticalAxis
	{
		get
		{
			if (this.AccelerationPressed)
			{
				return 1f;
			}
			if (this.DecelerationPressed)
			{
				return -1f;
			}
			return 0f;
		}
	}

	protected override void Awake()
	{
		base.Awake();
		this.AccelerometerNotSupportObject.SetActive(!SystemInfo.supportsAccelerometer);
	}

	private void Update()
	{
		if (SystemInfo.supportsAccelerometer)
		{
			float num = Input.acceleration.x * 90f;
			float num2 = 0f;
			if (num > this.DeadZone || num < -this.DeadZone)
			{
				num2 = Mathf.Clamp((num + ((num > 0f) ? (-this.DeadZone) : this.DeadZone)) / this.MaxAngle, -1f, 1f) * 90f;
			}
			if (base.ControlledCar.CarDirection >= 0 && base.ControlledCar.SpeedInHour > 20f)
			{
				num2 += base.ControlledCar.VelocityAngle;
			}
			num2 = num2.Clamp(-90f, 90f) / 90f;
			this.GetHorizontalAxis = Mathf.Lerp(this.GetHorizontalAxis, num2, Time.deltaTime * this.AccelerometerLerpSpeed);
		}
	}

	[SerializeField]
	private CustomButton AccelerationButton;

	[SerializeField]
	private CustomButton DecelerationButton;

	[SerializeField]
	private GameObject AccelerometerNotSupportObject;

	[SerializeField]
	private float DeadZone = 5f;

	[SerializeField]
	private float MaxAngle = 45f;

	[SerializeField]
	private float AccelerometerLerpSpeed = 500f;
}
