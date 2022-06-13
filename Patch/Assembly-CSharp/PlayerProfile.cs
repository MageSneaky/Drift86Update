using System;
using CodeStage.AntiCheat.Storage;
using GameBalance;
using UnityEngine;

public static class PlayerProfile
{
	public static string NickName
	{
		get
		{
			if (PlayerPrefs.HasKey("DisplayName"))
			{
				Debug.Log("Used Display Name: " + PlayerPrefs.GetString("DisplayName"));
				return PlayerPrefs.GetString("DisplayName");
			}
			if (!PlayerPrefs.HasKey("nn"))
			{
				Debug.Log("Created nn");
				PlayerPrefs.SetString("nn", string.Format("Player {0}", Random.Range(0, 99999)));
			}
			Debug.Log("Used nn");
			return PlayerPrefs.GetString("nn");
		}
		set
		{
			if (!string.IsNullOrEmpty(value))
			{
				PlayerPrefs.SetString("nn", value);
			}
		}
	}

	public static string ServerToken
	{
		get
		{
			if (!PlayerPrefs.HasKey("st"))
			{
				return string.Empty;
			}
			return PlayerPrefs.GetString("st");
		}
		set
		{
			PlayerPrefs.SetString("st", value);
		}
	}

	public static int TotalScore
	{
		get
		{
			if (ObscuredPrefs.GetInt("TotalScore_ob", 0) != 0)
			{
				return ObscuredPrefs.GetInt("TotalScore_ob", 0);
			}
			return PlayerPrefs.GetInt("TotalScore", 0);
		}
		set
		{
			ObscuredPrefs.SetInt("TotalScore_ob", value);
		}
	}

	public static float RaceTime
	{
		get
		{
			return PlayerPrefs.GetFloat("TimeInRace", 0f);
		}
		set
		{
			PlayerPrefs.SetFloat("TimeInRace", value);
		}
	}

	public static float TotalDistance
	{
		get
		{
			if (ObscuredPrefs.GetFloat("TotalDistance_ob", 0f) != 0f)
			{
				return ObscuredPrefs.GetFloat("TotalDistance_ob", 0f);
			}
			return PlayerPrefs.GetFloat("TotalDistance", 0f);
		}
		set
		{
			ObscuredPrefs.SetFloat("TotalDistance_ob", value);
		}
	}

	public static void SetRaceTimeForTrack(TrackPreset track, float time)
	{
		PlayerPrefs.SetFloat(string.Format("{0}_{1}", "RaceTime", track.name), time);
	}

	public static float GetRaceTimeForTrack(TrackPreset track)
	{
		return PlayerPrefs.GetFloat(string.Format("{0}_{1}", "RaceTime", track.name));
	}

	public static void SetBestLapForTrack(TrackPreset track, float time)
	{
		PlayerPrefs.SetFloat(string.Format("{0}_{1}", "LapTime", track.name), time);
	}

	public static float GetBestLapForTrack(TrackPreset track)
	{
		return PlayerPrefs.GetFloat(string.Format("{0}_{1}", "LapTime", track.name));
	}

	public static int Money
	{
		get
		{
			return PlayerPrefs.GetInt("Money");
		}
		set
		{
			if (PlayerProfile.Money != value)
			{
				PlayerProfile.OnMoneyChanged.SafeInvoke(value);
				PlayerPrefs.SetInt("Money", value);
			}
		}
	}

	public static void SetObjectAsBought(LockedContent obj)
	{
		PlayerPrefs.SetInt(string.Format("{0}_{1}", obj.name, "Bought"), 0);
	}

	public static bool ObjectIsBought(LockedContent obj)
	{
		return PlayerPrefs.HasKey(string.Format("{0}_{1}", obj.name, "Bought"));
	}

	public static void SetTrackAsComplited(TrackPreset track)
	{
		PlayerPrefs.SetInt(string.Format("{0}_{1}", track.name, "Comleted"), 0);
	}

	public static bool TrackIsComplited(TrackPreset track)
	{
		return PlayerPrefs.HasKey(string.Format("{0}_{1}", track.name, "Comleted"));
	}

	public static int BestScore
	{
		get
		{
			if (ObscuredPrefs.GetInt("BestScore_ob", 0) != 0)
			{
				return ObscuredPrefs.GetInt("BestScore_ob", 0);
			}
			return PlayerPrefs.GetInt("BestScore", 0);
		}
		set
		{
			Debug.Log("lllll");
			ObscuredPrefs.SetInt("BestScore_ob", value);
		}
	}

	public static string BestScoreCar
	{
		get
		{
			return PlayerPrefs.GetString("BestScore_car", "--");
		}
		set
		{
			PlayerPrefs.SetString("BestScore_car", value);
		}
	}

	public static void SetCarColor(CarPreset car, CarColorPreset color)
	{
		PlayerPrefs.SetInt(string.Format("{0}_{1}", "cci", car.name), car.AvailibleColors.IndexOf(color));
	}

	public static int GetCarColorIndex(CarPreset car)
	{
		return PlayerPrefs.GetInt(string.Format("{0}_{1}", "cci", car.name), 0);
	}

	public static CarColorPreset GetCarColor(CarPreset car)
	{
		return car.AvailibleColors[PlayerProfile.GetCarColorIndex(car)];
	}

	public static Action<int> OnMoneyChanged;
}
