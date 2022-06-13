using System;
using System.IO;
using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;

namespace D86Plugin
{
	// Token: 0x02000003 RID: 3
	[BepInPlugin("_D86Plugin", "D86Plugin", "1.0.0.0")]
	public class Main : BaseUnityPlugin
	{
		// Token: 0x06000003 RID: 3 RVA: 0x00002158 File Offset: 0x00000358
		public Main()
		{
			this.log = base.Logger;
			this.harmony = new Harmony("_D86Plugin");
			this.assembly = Assembly.GetExecutingAssembly();
			this.modFolder = Path.GetDirectoryName(this.assembly.Location);
		}

		// Token: 0x06000004 RID: 4 RVA: 0x00002058 File Offset: 0x00000258
		public void Start()
		{
			this.harmony.PatchAll(this.assembly);
			this.InitConfig();
		}

		// Token: 0x06000005 RID: 5 RVA: 0x000021A8 File Offset: 0x000003A8
		private void Update()
		{
			if (Main.flyMode)
			{
				Vector3 position = Main.holder.transform.position + new Vector3(0f, 0f, 0f);
				float d = Main.flyval.Value;
				if (Input.GetKey(KeyCode.LeftShift))
				{
					d = Main.flyval.Value * 2f;
				}
				if (Input.GetKey(KeyCode.LeftControl))
				{
					d = Main.flyval.Value / 2f;
				}
				if (Input.GetKey(KeyCode.W))
				{
					position = (Main.holder.transform.position += Main.holder.transform.forward * Time.deltaTime * d);
				}
				if (Input.GetKey(KeyCode.S))
				{
					position = (Main.holder.transform.position -= Main.holder.transform.forward * Time.deltaTime * d);
				}
				if (Input.GetKey(KeyCode.A))
				{
					position = (Main.holder.transform.position -= Main.holder.transform.right * Time.deltaTime * d);
				}
				if (Input.GetKey(KeyCode.D))
				{
					position = (Main.holder.transform.position += Main.holder.transform.right * Time.deltaTime * d);
				}
				if (Input.GetKey(KeyCode.Q))
				{
					position = (Main.holder.transform.position += Main.holder.transform.up * Time.deltaTime * d);
				}
				if (Input.GetKey(KeyCode.E))
				{
					position = (Main.holder.transform.position -= Main.holder.transform.up * Time.deltaTime * d);
				}
				if (Input.GetKey(KeyCode.Mouse1))
				{
					Quaternion rotation = Camera.main.transform.rotation;
					rotation.x = 0f;
					rotation.z = 0f;
					Main.holder.transform.rotation = rotation;
				}
				Main.holder.transform.position = position;
				Main.holder.transform.rotation = Quaternion.LookRotation(Main.holder.transform.forward);
				float num = 2f;
				float num2 = 2f;
				float yAngle = num * Input.GetAxis("Mouse X");
				float num3 = num2 * Input.GetAxis("Mouse Y");
				Main.holder.transform.Rotate(-num3, yAngle, 0f);
			}
			if (Input.GetKeyDown(Main.FREECAM.Value))
			{
				Main.flyMode = !Main.flyMode;
				if (Main.Game == null)
				{
					Main.Game = UnityEngine.Object.FindObjectOfType<GameController>();
				}
				if (Main.L_Car == null)
				{
					Main.L_Car = Main.Game.m_PlayerCar;
				}
				if (Main.L_Car.gameObject == null)
				{
					GameObject gameObject = Main.L_Car.gameObject;
				}
				if (Main.L_Car.gameObject.GetComponent<FX>() == null)
				{
					Main.L_Car.gameObject.AddComponent<FX>();
				}
				if (Main.c == null)
				{
					Main.holder = new GameObject();
					Main.holder.name = "FREECAM";
					Main.holder.gameObject.AddComponent<Camera>();
					Main.c = Main.holder.gameObject.GetComponent<Camera>();
				}
				if (!Main.flyMode)
				{
					Main.c.enabled = false;
					Main.L_Car.enabled = true;
					return;
				}
				if (Main.flyMode)
				{
					Main.c.enabled = true;
					Main.L_Car.enabled = false;
					Main.L_Car.RB.velocity = new Vector3(0f, 0f, 0f);
				}
			}
		}

		// Token: 0x06000006 RID: 6 RVA: 0x000025D4 File Offset: 0x000007D4
		public void InitConfig()
		{
			Main.FREECAM = base.Config.Bind<KeyCode>("Player", "FREECAM", KeyCode.F, "FREECAM");
			Main.flyval = base.Config.Bind<float>("Player", "Fly Speed", 25f, "Fly Speed");
			FX.Realtime = base.Config.Bind<bool>("Player", "Realtime reflactions", true, "Car/world reflections");
		}

		// Token: 0x04000003 RID: 3
		public const string MODNAME = "D86Plugin";

		// Token: 0x04000004 RID: 4
		public const string AUTHOR = "Va1idUser, MageSneaky";

		// Token: 0x04000005 RID: 5
		public const string GUID = "_D86Plugin";

		// Token: 0x04000006 RID: 6
		public const string VERSION = "1.0.0.0";

		// Token: 0x04000007 RID: 7
		internal readonly ManualLogSource log;

		// Token: 0x04000008 RID: 8
		internal readonly Harmony harmony;

		// Token: 0x04000009 RID: 9
		internal readonly Assembly assembly;

		// Token: 0x0400000A RID: 10
		public readonly string modFolder;

		// Token: 0x0400000B RID: 11
		public static GameController Game;

		// Token: 0x0400000C RID: 12
		public static CarController L_Car;

		// Token: 0x0400000D RID: 13
		public static ConfigEntry<KeyCode> FREECAM;

		// Token: 0x0400000E RID: 14
		public static ConfigEntry<float> flyval;

		// Token: 0x0400000F RID: 15
		public static bool flyMode;

		// Token: 0x04000010 RID: 16
		public static GameObject holder;

		// Token: 0x04000011 RID: 17
		public static Camera c;
	}
}
