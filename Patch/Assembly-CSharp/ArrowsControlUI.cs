using System;
using UnityEngine;

public class ArrowsControlUI : ControlUI, IUserControl
{
	private bool LeftPressed
	{
		get
		{
			return this.TurnLeftButton.ButtonIsPressed;
		}
	}

	private bool RightPressed
	{
		get
		{
			return this.TurnRigthButton.ButtonIsPressed;
		}
	}

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
			return this.LeftPressed || this.RightPressed || this.AccelerationPressed || this.DecelerationPressed;
		}
	}

	public float GetHorizontalAxis
	{
		get
		{
			if (this.LeftPressed)
			{
				return -1f;
			}
			if (this.RightPressed)
			{
				return 1f;
			}
			return 0f;
		}
	}

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

	[SerializeField]
	private CustomButton TurnLeftButton;

	[SerializeField]
	private CustomButton TurnRigthButton;

	[SerializeField]
	private CustomButton AccelerationButton;

	[SerializeField]
	private CustomButton DecelerationButton;
}
