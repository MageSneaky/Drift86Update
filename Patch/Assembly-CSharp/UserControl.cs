using System;
using UnityEngine;

[RequireComponent(typeof(CarController))]
public class UserControl : MonoBehaviour, ICarControl
{
	public float Horizontal { get; private set; }

	public float Vertical { get; private set; }

	public bool Brake { get; private set; }

	public static IUserControl CurrentUIControl { get; set; }

	private void Awake()
	{
		this.ControlledCar = base.GetComponent<CarController>();
	}

	private void Start()
	{
		PositioningCar positioningCar = this.ControlledCar.PositioningCar;
		positioningCar.OnFinishRaceAction = (Action)Delegate.Combine(positioningCar.OnFinishRaceAction, new Action(delegate()
		{
			base.gameObject.AddComponent<DriftAIControl>();
			base.enabled = false;
		}));
	}

	private void Update()
	{
		this.Horizontal = 0f;
		this.Vertical = 0f;
		this.Brake = false;
		if (!GameController.InPause)
		{
			if (UserControl.CurrentUIControl != null && UserControl.CurrentUIControl.ControlInUse)
			{
				this.Horizontal = UserControl.CurrentUIControl.GetHorizontalAxis;
				this.Vertical = UserControl.CurrentUIControl.GetVerticalAxis;
			}
			else
			{
				this.Horizontal = Input.GetAxis("Horizontal");
				this.Vertical = Input.GetAxis("Vertical");
				this.Brake = Input.GetButton("Jump");
			}
			if ((Input.GetKeyDown(114) && !SneakyManager.GetChat()) || Input.GetKeyDown(353))
			{
				this.ControlledCar.ResetPosition();
			}
		}
		this.ControlledCar.UpdateControls(this.Horizontal, this.Vertical, this.Brake);
	}

	private void OnDestroy()
	{
		UserControl.CurrentUIControl = null;
	}

	private CarController ControlledCar;
}
