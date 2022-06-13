using System;
using UnityEngine;

public class HideObjectIfShadowsEnabled : MonoBehaviour
{
	private void Awake()
	{
		GameOptions.OnQualityChanged += this.OnQualityChanged;
		this.OnQualityChanged();
	}

	private void OnDestroy()
	{
		GameOptions.OnQualityChanged -= this.OnQualityChanged;
	}

	private void OnQualityChanged()
	{
		base.gameObject.SetActive(QualitySettings.shadows == 0);
	}
}
