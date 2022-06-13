using System;
using System.Collections.Generic;
using GameBalance;
using UnityEngine;
using UnityEngine.UI;

public class CarSetColorUI : MonoBehaviour
{
	private void Awake()
	{
		this.ColorPresentRef.SetActive(false);
	}

	public void SelectCar(CarPreset newCar, ISetColor carOnScene)
	{
		this.CurrentCar = newCar;
		this.CarOnScene = carOnScene;
		this.UpdateColorForCurrentCar();
	}

	private void UpdateColorForCurrentCar()
	{
		foreach (ColorPresentUI colorPresentUI in this.Colors)
		{
			colorPresentUI.SelectColor(false);
			colorPresentUI.SetActive(false);
		}
		for (int i = 0; i < this.CurrentCar.AvailibleColors.Count; i++)
		{
			if (i + 1 > this.MaxColors)
			{
				Debug.LogErrorFormat("In carPreset({0}) Available colors are greater than the maximum count (Max count is {1})", new object[]
				{
					this.CurrentCar,
					this.MaxColors
				});
				break;
			}
			ColorPresentUI color;
			if (this.Colors.Count <= i)
			{
				color = Object.Instantiate<ColorPresentUI>(this.ColorPresentRef, this.ColorPresentRef.transform.parent);
				RectTransform rectTransform = color.transform as RectTransform;
				Vector2 anchoredPosition = rectTransform.anchoredPosition;
				anchoredPosition.x += (this.OffsetBetweenPresets + rectTransform.sizeDelta.x) * (float)i;
				rectTransform.anchoredPosition = anchoredPosition;
				this.Colors.Add(color);
			}
			else
			{
				color = this.Colors[i];
			}
			color.SetActive(true);
			color.SelectColor(false);
			CarColorPreset newColor = this.CurrentCar.AvailibleColors[i];
			color.InitColor(newColor);
			Button getButton = color.GetButton;
			getButton.onClick.RemoveAllListeners();
			getButton.onClick.AddListener(delegate()
			{
				this.Colors.ForEach(delegate(ColorPresentUI c)
				{
					c.SelectColor(false);
				});
				PlayerProfile.SetCarColor(this.CurrentCar, newColor);
				color.SelectColor(true);
				this.CarOnScene.SetColor(newColor);
			});
		}
		int carColorIndex = PlayerProfile.GetCarColorIndex(this.CurrentCar);
		this.Colors[carColorIndex].SelectColor(true);
		this.CarOnScene.SetColor(this.CurrentCar.AvailibleColors[carColorIndex]);
	}

	[SerializeField]
	private ColorPresentUI ColorPresentRef;

	[SerializeField]
	private float OffsetBetweenPresets = 40f;

	[SerializeField]
	private int MaxColors = 7;

	private CarPreset CurrentCar;

	private ISetColor CarOnScene;

	private List<ColorPresentUI> Colors = new List<ColorPresentUI>();
}
