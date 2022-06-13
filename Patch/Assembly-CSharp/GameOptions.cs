using System;
using UnityEngine;
using UnityEngine.Audio;

public static class GameOptions
{
	[RuntimeInitializeOnLoadMethod(0)]
	private static void OnLoadScene()
	{
		GameOptions.UpdateAudioMixer();
		GameOptions.CurrentQuality = GameOptions.CurrentQuality;
	}

	public static event Action<ControlType> OnControlChanged;

	public static ControlType CurrentControl
	{
		get
		{
			return (ControlType)PlayerPrefs.GetInt("Control", 0);
		}
		set
		{
			PlayerPrefs.SetInt("Control", (int)value);
			GameOptions.OnControlChanged.SafeInvoke(value);
		}
	}

	public static int ActiveCameraIndex
	{
		get
		{
			return PlayerPrefs.GetInt("CameraIndex", 0);
		}
		set
		{
			PlayerPrefs.SetInt("CameraIndex", value);
		}
	}

	public static event Action OnQualityChanged;

	public static int CurrentQuality
	{
		get
		{
			int result;
			if (PlayerPrefs.HasKey("Quality"))
			{
				result = PlayerPrefs.GetInt("Quality");
			}
			else
			{
				result = QualitySettings.GetQualityLevel();
			}
			return result;
		}
		set
		{
			if (QualitySettings.GetQualityLevel() != value)
			{
				QualitySettings.SetQualityLevel(value);
				PlayerPrefs.SetInt("Quality", value);
				GameOptions.OnQualityChanged.SafeInvoke();
			}
			Application.targetFrameRate = B.GraphicsSettings.TargetFPS;
			Shader.globalMaximumLOD = (value + 2) * 100;
		}
	}

	public static bool SoundIsMute
	{
		get
		{
			return PlayerPrefs.GetInt("Mute", 0) == 1;
		}
		set
		{
			PlayerPrefs.SetInt("Mute", value ? 1 : 0);
			GameOptions.UpdateAudioMixer();
		}
	}

	public static void UpdateAudioMixer()
	{
		AudioMixerSnapshot audioMixerSnapshot = B.SoundSettings.StandartSnapshot;
		if (GameOptions.SoundIsMute)
		{
			audioMixerSnapshot = B.SoundSettings.MuteSnapshot;
		}
		else if (Mathf.Approximately(Time.timeScale, 0f))
		{
			audioMixerSnapshot = B.SoundSettings.PauseSnapshot;
		}
		audioMixerSnapshot.TransitionTo(0.5f);
	}

	public static bool EnableAI
	{
		get
		{
			return PlayerPrefs.GetInt("EnableAI", 1) == 1;
		}
		set
		{
			PlayerPrefs.SetInt("EnableAI", value ? 1 : 0);
		}
	}
}
