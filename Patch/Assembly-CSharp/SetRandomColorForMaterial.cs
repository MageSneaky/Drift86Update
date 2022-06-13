using System;
using System.Collections.Generic;
using UnityEngine;

public class SetRandomColorForMaterial : MonoBehaviour
{
	public void Awake()
	{
		if (this.Colors.Count == 0)
		{
			return;
		}
		Color color = this.Colors.RandomChoice<Color>();
		MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
		materialPropertyBlock.SetColor("_Color", color);
		base.GetComponent<Renderer>().SetPropertyBlock(materialPropertyBlock);
	}

	[SerializeField]
	private List<Color> Colors = new List<Color>();
}
