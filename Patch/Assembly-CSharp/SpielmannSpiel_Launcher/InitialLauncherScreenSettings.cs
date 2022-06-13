using System;

namespace SpielmannSpiel_Launcher
{
	public class InitialLauncherScreenSettings
	{
		public InitialLauncherScreenSettings()
		{
		}

		public InitialLauncherScreenSettings(int width, int height, bool fullScreen)
		{
			this.launcherWidth = width;
			this.launcherHeight = height;
			this.launcherFullScreen = fullScreen;
		}

		public int launcherWidth;

		public int launcherHeight;

		public bool launcherFullScreen;
	}
}
