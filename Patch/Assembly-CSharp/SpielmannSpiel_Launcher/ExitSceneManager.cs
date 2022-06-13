using System;
using UnityEngine;

namespace SpielmannSpiel_Launcher
{
	public class ExitSceneManager : MonoBehaviour
	{
		private void Awake()
		{
			if (ScreenHelper.initialLauncherScreenSettings != null)
			{
				Screen.SetResolution(ScreenHelper.initialLauncherScreenSettings.launcherWidth, ScreenHelper.initialLauncherScreenSettings.launcherHeight, ScreenHelper.initialLauncherScreenSettings.launcherFullScreen);
			}
		}

		private void Start()
		{
			Application.Quit();
		}
	}
}
