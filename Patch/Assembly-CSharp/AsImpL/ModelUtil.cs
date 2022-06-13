using System;
using UnityEngine;

namespace AsImpL
{
	public class ModelUtil
	{
		public static void SetupMaterialWithBlendMode(Material mtl, ModelUtil.MtlBlendMode mode)
		{
			switch (mode)
			{
			case ModelUtil.MtlBlendMode.OPAQUE:
				mtl.SetOverrideTag("RenderType", "Opaque");
				mtl.SetFloat("_Mode", 0f);
				mtl.SetInt("_SrcBlend", 1);
				mtl.SetInt("_DstBlend", 0);
				mtl.SetInt("_ZWrite", 1);
				mtl.DisableKeyword("_ALPHATEST_ON");
				mtl.DisableKeyword("_ALPHABLEND_ON");
				mtl.DisableKeyword("_ALPHAPREMULTIPLY_ON");
				mtl.renderQueue = -1;
				return;
			case ModelUtil.MtlBlendMode.CUTOUT:
				mtl.SetOverrideTag("RenderType", "TransparentCutout");
				mtl.SetFloat("_Mode", 1f);
				mtl.SetFloat("_Mode", 1f);
				mtl.SetInt("_SrcBlend", 1);
				mtl.SetInt("_DstBlend", 0);
				mtl.SetInt("_ZWrite", 1);
				mtl.EnableKeyword("_ALPHATEST_ON");
				mtl.DisableKeyword("_ALPHABLEND_ON");
				mtl.DisableKeyword("_ALPHAPREMULTIPLY_ON");
				mtl.renderQueue = 2450;
				return;
			case ModelUtil.MtlBlendMode.FADE:
				mtl.SetOverrideTag("RenderType", "Transparent");
				mtl.SetFloat("_Mode", 2f);
				mtl.SetInt("_SrcBlend", 5);
				mtl.SetInt("_DstBlend", 10);
				mtl.SetInt("_ZWrite", 0);
				mtl.DisableKeyword("_ALPHATEST_ON");
				mtl.EnableKeyword("_ALPHABLEND_ON");
				mtl.DisableKeyword("_ALPHAPREMULTIPLY_ON");
				mtl.renderQueue = 3000;
				return;
			case ModelUtil.MtlBlendMode.TRANSPARENT:
				mtl.SetOverrideTag("RenderType", "Transparent");
				mtl.SetFloat("_Mode", 3f);
				mtl.SetInt("_SrcBlend", 1);
				mtl.SetInt("_DstBlend", 10);
				mtl.SetInt("_ZWrite", 0);
				mtl.DisableKeyword("_ALPHATEST_ON");
				mtl.DisableKeyword("_ALPHABLEND_ON");
				mtl.EnableKeyword("_ALPHAPREMULTIPLY_ON");
				mtl.renderQueue = 3000;
				return;
			default:
				return;
			}
		}

		public static bool ScanTransparentPixels(Texture2D texture, ref ModelUtil.MtlBlendMode mode)
		{
			if (texture != null && (texture.format == 5 || texture.format == 4 || texture.format == 12 || texture.format == 2 || texture.format == 14 || texture.format == 29))
			{
				bool flag = false;
				int num = 1;
				int num2 = 0;
				while (num2 < texture.width && !flag)
				{
					int num3 = 0;
					while (num3 < texture.height && !flag)
					{
						ModelUtil.DetectMtlBlendFadeOrCutout(texture.GetPixel(num2, num3).a, ref mode, ref flag);
						if (flag)
						{
							return mode == ModelUtil.MtlBlendMode.FADE || mode == ModelUtil.MtlBlendMode.CUTOUT;
						}
						num3 += num;
					}
					num2 += num;
				}
			}
			return mode == ModelUtil.MtlBlendMode.FADE || mode == ModelUtil.MtlBlendMode.CUTOUT;
		}

		public static void DetectMtlBlendFadeOrCutout(float alpha, ref ModelUtil.MtlBlendMode mode, ref bool noDoubt)
		{
			if (noDoubt)
			{
				return;
			}
			if (alpha < 1f)
			{
				if (alpha == 0f)
				{
					mode = ModelUtil.MtlBlendMode.CUTOUT;
					return;
				}
				if (mode != ModelUtil.MtlBlendMode.FADE)
				{
					mode = ModelUtil.MtlBlendMode.FADE;
					noDoubt = true;
				}
			}
		}

		public static Texture2D HeightToNormalMap(Texture2D bumpMap, float amount = 1f)
		{
			int height = bumpMap.height;
			int width = bumpMap.width;
			Texture2D texture2D = new Texture2D(width, height, 5, true);
			Color black = Color.black;
			for (int i = 0; i < bumpMap.height; i++)
			{
				for (int j = 0; j < bumpMap.width; j++)
				{
					Vector3 vector = Vector3.zero;
					float grayscale = bumpMap.GetPixel(ModelUtil.WrapInt(j - 1, width), i).grayscale;
					float grayscale2 = bumpMap.GetPixel(j, i).grayscale;
					float grayscale3 = bumpMap.GetPixel(ModelUtil.WrapInt(j + 1, width), i).grayscale;
					float num = grayscale2 - grayscale;
					float num2 = grayscale3 - grayscale2;
					vector.x = -(num2 + num) / 255f;
					grayscale = bumpMap.GetPixel(j, ModelUtil.WrapInt(i - 1, height)).grayscale;
					grayscale2 = bumpMap.GetPixel(j, i).grayscale;
					float grayscale4 = bumpMap.GetPixel(j, ModelUtil.WrapInt(i + 1, height)).grayscale;
					num = grayscale2 - grayscale;
					num2 = grayscale4 - grayscale2;
					vector.y = -(num2 + num);
					if (amount != 1f)
					{
						vector *= amount;
					}
					vector.z = Mathf.Sqrt(1f - (vector.x * vector.x + vector.y * vector.y));
					vector *= 0.5f;
					black.r = Mathf.Clamp01(vector.x + 0.5f);
					black.g = Mathf.Clamp01(vector.y + 0.5f);
					black.b = Mathf.Clamp01(vector.z + 0.5f);
					black.a = black.r;
					texture2D.SetPixel(j, i, black);
				}
			}
			texture2D.Apply();
			return texture2D;
		}

		private static int WrapInt(int pos, int boundary)
		{
			if (pos < 0)
			{
				pos = boundary + pos;
			}
			else if (pos >= boundary)
			{
				pos -= boundary;
			}
			return pos;
		}

		public enum MtlBlendMode
		{
			OPAQUE,
			CUTOUT,
			FADE,
			TRANSPARENT
		}
	}
}
