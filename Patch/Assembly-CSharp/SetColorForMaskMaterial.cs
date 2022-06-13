using System;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class SetColorForMaskMaterial : MonoBehaviour
{
	public void SetColor(CarColorPreset color)
	{
		MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
		materialPropertyBlock.SetColor("_Color", color.Color);
		materialPropertyBlock.SetFloat("_Smoothness", color.Smoothness);
		base.GetComponent<Renderer>().SetPropertyBlock(materialPropertyBlock);
	}
}
