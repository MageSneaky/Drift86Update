using System;
using UnityEngine;

public class GameMenuController : MonoBehaviour
{
	private void Start()
	{
		Object.DontDestroyOnLoad(base.gameObject);
	}

	private void Update()
	{
	}

	public void ButtonToggleMenu()
	{
		if (!this.menuOpen)
		{
			this.previousTimescale = Time.timeScale;
			Time.timeScale = 1f;
			this.menuCanvasObj.SetActive(true);
			this.menuOpen = true;
			return;
		}
		Time.timeScale = 1f;
		this.menuOpen = false;
	}

	public void DeletePlayerprefs()
	{
		PlayerPrefs.DeleteKey("graphicsPrefsSaved");
		PlayerPrefs.DeleteKey("FPSToggle");
		PlayerPrefs.DeleteKey("graphicsSlider");
		PlayerPrefs.DeleteKey("antiAliasSlider");
		PlayerPrefs.DeleteKey("shadowResolutionSlider");
		PlayerPrefs.DeleteKey("textureQualitySlider");
		PlayerPrefs.DeleteKey("anisotropicModeSlider");
		PlayerPrefs.DeleteKey("anisotropicLevelSlider");
		PlayerPrefs.DeleteKey("wantedResolutionX");
		PlayerPrefs.DeleteKey("wantedResolutionY");
		PlayerPrefs.DeleteKey("windowedModeToggle");
		PlayerPrefs.DeleteKey("vSyncToggle");
		PlayerPrefs.DeleteKey("audioPrefsSaved");
		PlayerPrefs.DeleteKey("mainVolumeF");
		PlayerPrefs.DeleteKey("fxVolumeF");
		PlayerPrefs.DeleteKey("musicVolumeF");
	}

	public void ButtonQuitGame()
	{
		Application.Quit();
	}

	public GameObject menuCanvasObj;

	private float previousTimescale;

	private bool menuOpen;
}
