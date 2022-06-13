using System;
using System.Collections.Generic;
using UnityEngine;

public class CarInSelectMenuPrefab : MonoBehaviour, ISetColor
{
	public void SetColor(CarColorPreset colorPreset)
	{
		this.SetColorObjects.ForEach(delegate(SetColorForMaskMaterial c)
		{
			c.SetColor(colorPreset);
		});
	}

	[SerializeField]
	private List<SetColorForMaskMaterial> SetColorObjects = new List<SetColorForMaskMaterial>();
}
