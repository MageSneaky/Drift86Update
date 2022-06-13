using System;
using UnityEngine;

namespace AsImpL
{
	public class MaterialData
	{
		public string materialName;

		public Color ambientColor;

		public Color diffuseColor;

		public Color specularColor;

		public Color emissiveColor;

		public float shininess;

		public float overallAlpha = 1f;

		public int illumType;

		public bool hasReflectionTex;

		public string diffuseTexPath;

		public Texture2D diffuseTex;

		public string bumpTexPath;

		public Texture2D bumpTex;

		public string specularTexPath;

		public Texture2D specularTex;

		public string opacityTexPath;

		public Texture2D opacityTex;
	}
}
