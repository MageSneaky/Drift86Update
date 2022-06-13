using System;
using BepInEx.Configuration;
using UnityEngine;
using UnityEngine.Rendering;

namespace D86Plugin
{
	// Token: 0x02000002 RID: 2
	public class FX : MonoBehaviour
	{
		// Token: 0x06000001 RID: 1 RVA: 0x00002074 File Offset: 0x00000274
		public void Update()
		{
			if (FX.Realtime.Value)
			{
				if (FX.rp == null)
				{
					FX.rp = UnityEngine.Object.FindObjectOfType<ReflectionProbe>();
					return;
				}
				FX.rp.farClipPlane = 10000f;
				FX.rp.mode = ReflectionProbeMode.Realtime;
				FX.rp.refreshMode = ReflectionProbeRefreshMode.EveryFrame;
				FX.rp.resolution = 512;
				FX.rp.size = new Vector3(10000f, 10000f, 10000f);
				Vector3 position = Main.L_Car.gameObject.transform.position;
				Vector3 position2 = FX.rp.gameObject.transform.position;
				FX.rp.gameObject.transform.position = new Vector3(position.x, position.y + 15f, position.z);
			}
		}

		// Token: 0x04000001 RID: 1
		public static ReflectionProbe rp;

		// Token: 0x04000002 RID: 2
		public static ConfigEntry<bool> Realtime;
	}
}
