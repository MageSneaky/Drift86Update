using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class SteerWheelControlUI : ControlUI, IUserControl
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
			return this.CurrentSteerAngle != 0f || this.AccelerationButton.ButtonIsPressed || this.DecelerationButton.ButtonIsPressed;
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
		this.SteerWheelButton.onPointerDown += this.OnSteerDown;
		this.SteerWheelButton.onPointerUp += this.OnSteerUp;
	}

	private void Update()
	{
		float num = base.ControlledCar.VelocityAngle / 90f;
		bool flag = base.ControlledCar.CarDirection >= 0 && base.ControlledCar.SpeedInHour > 20f;
		float num2;
		if (!this.WheelIsPressed)
		{
			num2 = (flag ? num : 0f) * this.MaxSteerWheelAngle;
			this.CurrentSteerAngle = Mathf.MoveTowards(this.CurrentSteerAngle, num2, Time.deltaTime * this.SteerWheelToDefaultSpeed);
		}
		else
		{
			Vector2 vector = this.SteerWheelButton.transform.position;
			if (Application.isMobilePlatform)
			{
				Vector2 position = Input.GetTouch(0).position;
				for (int i = 1; i < Input.touchCount; i++)
				{
					if (position.x > Input.GetTouch(i).position.x)
					{
						position = Input.GetTouch(i).position;
					}
				}
				vector -= position;
			}
			else
			{
				vector -= Input.mousePosition;
			}
			float num3 = Vector2.SignedAngle(this.PrevTouchPos, vector);
			this.PrevTouchPos = vector;
			this.CurrentSteerAngle = Mathf.Clamp(this.CurrentSteerAngle + num3, -this.MaxSteerWheelAngle, this.MaxSteerWheelAngle);
		}
		this.SteerWheelButton.transform.rotation = Quaternion.AngleAxis(this.CurrentSteerAngle, Vector3.forward);
		num2 = -this.CurrentSteerAngle / this.MaxSteerWheelAngle;
		if (flag)
		{
			num2 += num;
		}
		num2 = num2.Clamp(-1f, 1f);
		this.GetHorizontalAxis = num2;
	}

	private void OnSteerDown(PointerEventData eventData)
	{
		this.WheelIsPressed = true;
		this.PrevTouchPos = this.SteerWheelButton.transform.position - eventData.position;
	}

	private void OnSteerUp(PointerEventData eventData)
	{
		this.WheelIsPressed = false;
	}

	[SerializeField]
	private CustomButton AccelerationButton;

	[SerializeField]
	private CustomButton DecelerationButton;

	[SerializeField]
	private CustomButton SteerWheelButton;

	[SerializeField]
	private float DeadZone = 10f;

	[SerializeField]
	private float MaxSteerWheelAngle = 270f;

	[SerializeField]
	private float SteerWheelToDefaultSpeed = 360f;

	private float CurrentSteerAngle;

	private bool WheelIsPressed;

	private Vector2 PrevTouchPos;
}
