using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GameBalance;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CarParamsUI : MonoBehaviour
{
	private List<CarPreset> CanSelectCars
	{
		get
		{
			return WorldLoading.AvailableCars;
		}
	}

	private void UpdateMaxValues()
	{
		this.MaxPower = this.CanSelectCars.Max((CarPreset c) => c.GetPower);
		this.MaxControl = this.CanSelectCars.Max((CarPreset c) => c.GetControl);
		this.MaxMass = this.CanSelectCars.Max((CarPreset c) => c.GetMass);
	}

	public void SelectCar(CarPreset newCar)
	{
		if (Mathf.Approximately(this.MaxPower, 0f))
		{
			this.UpdateMaxValues();
		}
		this.DescriptionText.text = newCar.Description;
		if (this.SelectCarCoroutine != null)
		{
			base.StopCoroutine(this.SelectCarCoroutine);
		}
		if (base.gameObject.activeInHierarchy)
		{
			this.SelectCarCoroutine = base.StartCoroutine(this.DoSelectCar(newCar));
			return;
		}
		this.DoForseSelectCar(newCar);
	}

	private void DoForseSelectCar(CarPreset newCar)
	{
		float value = newCar.GetPower / this.MaxPower;
		float value2 = newCar.GetControl / this.MaxControl;
		float value3 = newCar.GetMass / this.MaxMass;
		this.PowerSlider.value = value;
		this.ControlSlider.value = value2;
		this.MassSlider.value = value3;
	}

	private IEnumerator DoSelectCar(CarPreset newCar)
	{
		float targetPower = newCar.GetPower / this.MaxPower;
		float targetControl = newCar.GetControl / this.MaxControl;
		float targetMass = newCar.GetMass / this.MaxMass;
		float currentPower = this.PowerSlider.value;
		float currentControl = this.ControlSlider.value;
		float currentMass = this.MassSlider.value;
		while (!Mathf.Approximately(targetPower, this.PowerSlider.value) || !Mathf.Approximately(targetControl, this.ControlSlider.value) || !Mathf.Approximately(targetMass, this.MassSlider.value))
		{
			currentPower = Mathf.MoveTowards(currentPower, targetPower, Time.deltaTime * this.ChangeParamsSpeed);
			currentControl = Mathf.MoveTowards(currentControl, targetControl, Time.deltaTime * this.ChangeParamsSpeed);
			currentMass = Mathf.MoveTowards(currentMass, targetMass, Time.deltaTime * this.ChangeParamsSpeed);
			this.PowerSlider.value = currentPower;
			this.ControlSlider.value = currentControl;
			this.MassSlider.value = currentMass;
			yield return null;
		}
		this.SelectCarCoroutine = null;
		yield break;
	}

	[SerializeField]
	private Slider PowerSlider;

	[SerializeField]
	private Slider ControlSlider;

	[SerializeField]
	private Slider MassSlider;

	[SerializeField]
	private float ChangeParamsSpeed;

	[SerializeField]
	private TextMeshProUGUI DescriptionText;

	private float MaxPower;

	private float MaxControl;

	private float MaxMass;

	private Coroutine SelectCarCoroutine;
}
