using System;
using GameBalance;
using UnityEngine;

public static class B
{
	private static Settings Settings
	{
		get
		{
			if (B._Settings == null)
			{
				B._Settings = Resources.Load<Settings>("Settings");
			}
			return B._Settings;
		}
	}

	public static GameSettings GameSettings
	{
		get
		{
			return B.Settings.GameSettings;
		}
	}

	public static GraphicsSettings GraphicsSettings
	{
		get
		{
			return B.Settings.GraphicsSettings;
		}
	}

	public static LayerSettings LayerSettings
	{
		get
		{
			return B.Settings.LayerSettings;
		}
	}

	public static SoundSettings SoundSettings
	{
		get
		{
			return B.Settings.SoundSettings;
		}
	}

	public static ResourcesSettings ResourcesSettings
	{
		get
		{
			return B.Settings.ResourcesSettings;
		}
	}

	public static MultiplayerSettings MultiplayerSettings
	{
		get
		{
			return B.Settings.MultiplayerSettings;
		}
	}

	public static float SpeedInHourMultiplier
	{
		get
		{
			return 3.6f;
		}
	}

	private static Settings _Settings;
}
