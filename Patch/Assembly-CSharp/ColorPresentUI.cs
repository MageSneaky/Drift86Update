using System;
using UnityEngine;
using UnityEngine.UI;

public class ColorPresentUI : MonoBehaviour
{
	public Button GetButton
	{
		get
		{
			return base.GetComponent<Button>();
		}
	}

	public void InitColor(CarColorPreset color)
	{
		this.MainColorImage.color = color.Color;
		this.SmoothnessImage.SetAlpha(color.Smoothness * this.MaxSmoothness);
	}

	public void SelectColor(bool select)
	{
		this.SelectedObject.SetActive(select);
		base.transform.localScale = Vector3.one * (select ? this.InSelectScale : this.InDeselectScale);
	}

	[SerializeField]
	private Image MainColorImage;

	[SerializeField]
	private Image SmoothnessImage;

	[SerializeField]
	private GameObject SelectedObject;

	[SerializeField]
	private float MaxSmoothness = 0.7f;

	[SerializeField]
	private float InSelectScale = 1.2f;

	[SerializeField]
	private float InDeselectScale = 0.8f;
}
