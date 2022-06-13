using System;
using TMPro;
using UnityEngine;

public class CarStateUI : MonoBehaviour
{
	private CarController SelectedCar
	{
		get
		{
			return GameController.PlayerCar;
		}
	}

	private void Update()
	{
		if (this.SelectedCar == null)
		{
			return;
		}
		if (this.CurrentFrame >= this.UpdateFrameCount)
		{
			this.UpdateGamePanel();
			this.CurrentFrame = 0;
		}
		else
		{
			this.CurrentFrame++;
		}
		this.UpdateArrow();
	}

	private void UpdateArrow()
	{
		float num = this.SelectedCar.EngineRPM / this.SelectedCar.GetMaxRPM;
		float num2 = (this.MaxArrowAngle - this.MinArrowAngle) * num + this.MinArrowAngle;
		this.TahometerArrow.rotation = Quaternion.AngleAxis(num2, Vector3.forward);
	}

	private void UpdateGamePanel()
	{
		this.SpeedText.text = this.SelectedCar.SpeedInHour.ToString("000");
		this.CurrentGearText.text = this.SelectedCar.CurrentGear.ToString();
	}

	[SerializeField]
	private int UpdateFrameCount = 3;

	[SerializeField]
	private TextMeshProUGUI SpeedText;

	[SerializeField]
	private TextMeshProUGUI CurrentGearText;

	[SerializeField]
	private RectTransform TahometerArrow;

	[SerializeField]
	private float MinArrowAngle;

	[SerializeField]
	private float MaxArrowAngle = -315f;

	private int CurrentFrame;
}
