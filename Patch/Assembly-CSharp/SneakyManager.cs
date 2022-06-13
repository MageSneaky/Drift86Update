using System;
using UnityEngine;

internal class SneakyManager
{
	public static bool GetMouseFree()
	{
		return SneakyManager.mouseFree;
	}

	public static void SetMouseFree()
	{
		SneakyManager.mouseFree = !SneakyManager.mouseFree;
		Cursor.visible = SneakyManager.mouseFree;
		if (SneakyManager.mouseFree)
		{
			Cursor.lockState = 0;
			return;
		}
		Cursor.lockState = 1;
	}

	public static bool GetChat()
	{
		return SneakyManager.chatOpen;
	}

	public static void SetChat(bool b)
	{
		SneakyManager.chatOpen = b;
	}

	public static void DrawMenu()
	{
		Texture2D texture2D = new Texture2D(128, 128);
		for (int i = 0; i < texture2D.height; i++)
		{
			for (int j = 0; j < texture2D.width; j++)
			{
				texture2D.SetPixel(j, i, Color.black);
			}
		}
		texture2D.Apply();
		GUIStyle guistyle = new GUIStyle();
		guistyle.fontSize = 20;
		guistyle.normal.textColor = Color.white;
		guistyle.alignment = 3;
		GUIStyle guistyle2 = new GUIStyle();
		guistyle2.fontSize = 20;
		guistyle2.normal.textColor = Color.white;
		guistyle2.normal.background = texture2D;
		guistyle2.alignment = 3;
		GUI.Label(new Rect(30f, (float)Screen.height - 75f, 120f, 25f), "DisplayName:", guistyle);
		SneakyManager.nameField = GUI.TextArea(new Rect(160f, (float)Screen.height - 75f, 170f, 25f), SneakyManager.nameField, 15, guistyle2);
		SneakyManager.dlcToggle = GUI.Toggle(new Rect(30f, (float)Screen.height - 55f, 160f, 25f), SneakyManager.dlcToggle, "dlcUnlocked");
		if (SneakyManager.nameField.Length >= 1)
		{
			PlayerPrefs.SetString("DisplayName", SneakyManager.nameField);
		}
		else
		{
			PlayerPrefs.DeleteKey("DisplayName");
		}
		if (SneakyManager.dlcToggle)
		{
			PlayerPrefs.SetInt("DLCUnlocked", 1);
			return;
		}
		PlayerPrefs.SetInt("DLCUnlocked", 0);
	}

	public static void GetPlayerPrefs()
	{
		if (PlayerPrefs.HasKey("DisplayName"))
		{
			SneakyManager.nameField = PlayerPrefs.GetString("DisplayName");
		}
		if (PlayerPrefs.HasKey("DLCUnlocked"))
		{
			int @int = PlayerPrefs.GetInt("DLCUnlocked");
			if (@int != 0)
			{
				if (@int == 1)
				{
					SneakyManager.dlcToggle = true;
					return;
				}
			}
			else
			{
				SneakyManager.dlcToggle = false;
			}
		}
	}

	public static bool chatOpen;

	public static bool mouseFree;

	public static string nameField = "";

	public static bool dlcToggle;
}
