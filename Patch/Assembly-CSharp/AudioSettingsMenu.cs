using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioSettingsMenu : MonoBehaviour
{
	private void Start()
	{
		this.LoadMenuVariables();
	}

	public void Update()
	{
	}

	public void ToggleMute(bool toggleValue)
	{
		this.isMuted = toggleValue;
		if (this.isMuted)
		{
			this.masterMixer.SetFloat("mainVolume", -80f);
			return;
		}
		this.masterMixer.SetFloat("mainVolume", Mathf.Log(this.mainVolumeSlider.value) * 20f);
	}

	public void SetMainVolume(float sliderValue)
	{
		if (!this.isMuted)
		{
			this.masterMixer.SetFloat("mainVolume", Mathf.Log(sliderValue) * 20f);
		}
		PlayerPrefs.SetFloat("mainVolumeF", this.mainVolumeSlider.value);
	}

	public void SetFxVolume(float sliderValue)
	{
		this.masterMixer.SetFloat("fxVolume", Mathf.Log(sliderValue) * 20f);
		PlayerPrefs.SetFloat("fxVolumeF", this.fxVolumeSlider.value);
	}

	public void SetMusicVolume(float sliderValue)
	{
		this.masterMixer.SetFloat("musicVolume", Mathf.Log(sliderValue) * 20f);
		PlayerPrefs.SetFloat("musicVolumeF", this.musicVolumeSLider.value);
	}

	public void SaveMenuVariables()
	{
		PlayerPrefs.SetInt("audioPrefsSaved", 0);
		PlayerPrefs.SetInt("mutedI", this.muteToggle.isOn ? 1 : 0);
		PlayerPrefs.SetFloat("mainVolumeF", this.mainVolumeSlider.value);
		PlayerPrefs.SetFloat("fxVolumeF", this.fxVolumeSlider.value);
		PlayerPrefs.SetFloat("musicVolumeF", this.musicVolumeSLider.value);
	}

	public void LoadMenuVariables()
	{
		if (PlayerPrefs.HasKey("audioPrefsSaved"))
		{
			this.mainVolumeSlider.value = PlayerPrefs.GetFloat("mainVolumeF");
			this.fxVolumeSlider.value = PlayerPrefs.GetFloat("fxVolumeF");
			this.musicVolumeSLider.value = PlayerPrefs.GetFloat("musicVolumeF");
			if (PlayerPrefs.GetInt("mutedI") == 1)
			{
				this.muteToggle.isOn = true;
				return;
			}
			this.muteToggle.isOn = false;
		}
	}

	public AudioMixer masterMixer;

	public Slider mainVolumeSlider;

	public Slider fxVolumeSlider;

	public Slider musicVolumeSLider;

	public Toggle muteToggle;

	private bool isMuted;
}
