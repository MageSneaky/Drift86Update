using System;
using UnityEngine;

public class ControlUI : MonoBehaviour
{
	protected CarController ControlledCar
	{
		get
		{
			return GameController.PlayerCar;
		}
	}

	protected virtual void Awake()
	{
		GameOptions.OnControlChanged += this.OnControlChanged;
		this.OnControlChanged(GameOptions.CurrentControl);
	}

	private void OnEnable()
	{
		if (this.ShowOnlyOnMobile)
		{
			base.gameObject.SetActive(Application.isMobilePlatform);
		}
	}

	private void OnDestroy()
	{
		GameOptions.OnControlChanged -= this.OnControlChanged;
	}

	private void OnControlChanged(ControlType type)
	{
		base.gameObject.SetActive(this.ShowControlType == type);
		if (this.ShowControlType == type)
		{
			UserControl.CurrentUIControl = (this as IUserControl);
		}
	}

	[SerializeField]
	private ControlType ShowControlType;

	[SerializeField]
	private bool ShowOnlyOnMobile = true;
}
