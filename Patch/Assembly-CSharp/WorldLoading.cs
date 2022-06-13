using System;
using System.Collections.Generic;
using GameBalance;
using UnityEngine;

public class WorldLoading : MonoBehaviour
{
	public static string PlayerName
	{
		get
		{
			return PlayerProfile.NickName;
		}
	}

	public static bool HasLoadingParams
	{
		get
		{
			return WorldLoading.PlayerCar != null;
		}
	}

	public static int LapsCount
	{
		get
		{
			if (!WorldLoading.HasLoadingParams)
			{
				return 3;
			}
			return WorldLoading.LoadingTrack.LapsCount;
		}
	}

	public static int AIsCount
	{
		get
		{
			if (!GameOptions.EnableAI)
			{
				return 0;
			}
			return WorldLoading.LoadingTrack.AIsCount;
		}
	}

	public static List<CarPreset> AvailableCars
	{
		get
		{
			return WorldLoading.RegimeSettings.AvailableCars;
		}
	}

	public static RegimeSettings RegimeSettings
	{
		get
		{
			RegimeSettings result;
			if (!(WorldLoading.LoadingTrack != null))
			{
				if ((result = WorldLoading.RegimeForDebug) == null)
				{
					return B.GameSettings.DefaultRegimeSettings;
				}
			}
			else
			{
				result = WorldLoading.LoadingTrack.RegimeSettings;
			}
			return result;
		}
	}

	public static CarColorPreset SelectedColor
	{
		get
		{
			return PlayerProfile.GetCarColor(WorldLoading.PlayerCar);
		}
	}

	public static RegimeSettings RegimeForDebug { get; set; }

	public static string BotName = "Bot";

	public static CarPreset PlayerCar;

	public static TrackPreset LoadingTrack;

	public static bool IsMultiplayer = false;
}
