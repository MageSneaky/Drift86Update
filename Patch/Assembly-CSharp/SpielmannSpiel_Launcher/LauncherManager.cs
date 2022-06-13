using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace SpielmannSpiel_Launcher
{
	public class LauncherManager : MonoBehaviour
	{
		public FullScreenMode getFullScreenMode()
		{
			switch (this.userDefinedFullScreenMode)
			{
			case LauncherManager.FullScreenModeEn._MaximizedWindow:
				return 2;
			case LauncherManager.FullScreenModeEn._ExclusiveFullScreen:
				return 0;
			}
			return 1;
		}

		private void Reset()
		{
			this.userDefinedFullScreenMode = LauncherManager.FullScreenModeEn._FullScreenWindow;
			this.enforceResolution = true;
			this.playerPrefsPrefix = "launcherSettings_";
			this.width = 450;
			this.height = 500;
		}

		private void Awake()
		{
		}

		private void Start()
		{
			if (this.enforceResolution && SceneManager.GetActiveScene().name == "LauncherSample")
			{
				Screen.SetResolution(this.width, this.height, false);
				if (ScreenHelper.initialLauncherScreenSettings == null)
				{
					ScreenHelper.initialLauncherScreenSettings = new InitialLauncherScreenSettings(this.width, this.height, false);
				}
			}
			else if (SceneManager.GetActiveScene().name == "LauncherSample" && ScreenHelper.initialLauncherScreenSettings == null)
			{
				ScreenHelper.initialLauncherScreenSettings = new InitialLauncherScreenSettings(Screen.currentResolution.width, Screen.currentResolution.height, Screen.fullScreen);
			}
			this.updateVariables();
			this.setUi();
			this.loadSettings();
		}

		public void updateVariables()
		{
			this.resolutions = ScreenHelper.getResolutionInfos();
			this.qualitySettingsNames = QualitySettings.names;
			this.fullScreenModeNames.Clear();
			foreach (string text in Enum.GetNames(typeof(LauncherManager.FullScreenModeEn)))
			{
				this.fullScreenModeNames.Add(text.Replace("_", ""));
			}
		}

		public void setUi()
		{
			if (this.dropdownFps != null)
			{
				this.dropdownFps.ClearOptions();
				Display[] displays = Display.displays;
				Display main = Display.main;
				for (int i = 0; i < this.availableFps.Count; i++)
				{
					this.dropdownFps.options.Add(new Dropdown.OptionData
					{
						text = this.availableFps[i].ToString()
					});
				}
				this.dropdownFps.value = this.defaultFpsIndex;
			}
			if (this.dropdownResolution != null)
			{
				this.dropdownResolution.ClearOptions();
				Vector2 vector;
				vector..ctor((float)Screen.currentResolution.width, (float)Screen.currentResolution.height);
				int value = 0;
				for (int j = 0; j < this.resolutions.Count; j++)
				{
					this.dropdownResolution.options.Add(new Dropdown.OptionData
					{
						text = this.resolutions[j].label
					});
					if (vector == this.resolutions[j].size)
					{
						value = j;
					}
				}
				this.dropdownResolution.value = value;
				this.dropdownResolution.RefreshShownValue();
			}
			if (this.dropdownQuality != null)
			{
				this.dropdownQuality.ClearOptions();
				int qualityLevel = QualitySettings.GetQualityLevel();
				for (int k = 0; k < this.qualitySettingsNames.Length; k++)
				{
					this.dropdownQuality.options.Add(new Dropdown.OptionData
					{
						text = this.qualitySettingsNames[k]
					});
				}
				this.dropdownQuality.value = qualityLevel;
				this.dropdownQuality.RefreshShownValue();
			}
			if (this.dropdownFullScreenMode != null)
			{
				this.dropdownFullScreenMode.ClearOptions();
				int value2 = (int)this.userDefinedFullScreenMode;
				for (int l = 0; l < this.fullScreenModeNames.Count; l++)
				{
					this.dropdownFullScreenMode.options.Add(new Dropdown.OptionData
					{
						text = this.fullScreenModeNames[l]
					});
				}
				this.dropdownFullScreenMode.value = value2;
				this.dropdownFullScreenMode.RefreshShownValue();
			}
		}

		public void updateQuality()
		{
			if (this.dropdownQuality != null)
			{
				QualitySettings.SetQualityLevel(this.dropdownQuality.value, true);
			}
		}

		public void activateSettings()
		{
			Debug.Log("OMG SAVE");
			bool flag = this.dropdownResolution != null;
			bool flag2 = this.toggleFullScreen != null;
			bool flag3 = this.dropdownQuality != null;
			bool flag4 = this.dropdownFps != null;
			bool flag5 = this.toggleVsync != null;
			bool flag6 = this.dropdownFullScreenMode != null;
			bool flag7 = Screen.fullScreen;
			if (flag6)
			{
				this.userDefinedFullScreenMode = (LauncherManager.FullScreenModeEn)this.dropdownFullScreenMode.value;
				PlayerPrefs.SetInt(this.playerPrefsPrefix + "fullScreenModeIndex", this.dropdownFullScreenMode.value);
			}
			if (flag3)
			{
				QualitySettings.SetQualityLevel(this.dropdownQuality.value, true);
				PlayerPrefs.SetInt(this.playerPrefsPrefix + "qualityIndex", this.dropdownQuality.value);
			}
			if (flag2)
			{
				flag7 = this.toggleFullScreen.isOn;
				PlayerPrefs.SetInt(this.playerPrefsPrefix + "setFullScreen", flag7 ? 1 : 0);
				if (!flag)
				{
					Debug.Log("FullScreenMode: Screen.fullScreen " + flag2.ToString());
					if (flag7 && this.getFullScreenMode() == null)
					{
						Screen.SetResolution(8, 6, this.getFullScreenMode());
					}
					else
					{
						Screen.fullScreen = flag7;
					}
				}
			}
			if (flag)
			{
				ResolutionInfo resolutionInfo = this.resolutions[this.dropdownResolution.value];
				if (flag7)
				{
					Debug.Log("FullScreenMode: " + this.getFullScreenMode().ToString());
					Screen.SetResolution(resolutionInfo.resolution.width, resolutionInfo.resolution.height, this.getFullScreenMode());
				}
				else
				{
					Debug.Log("FullScreenMode: FullScreenMode.Windowed");
					Screen.SetResolution(resolutionInfo.resolution.width, resolutionInfo.resolution.height, 3);
				}
				PlayerPrefs.SetInt(this.playerPrefsPrefix + "resolutionIndex", this.dropdownResolution.value);
			}
			if (flag4)
			{
				Application.targetFrameRate = this.availableFps[this.dropdownFps.value].fps;
				PlayerPrefs.SetInt(this.playerPrefsPrefix + "fpsIndex", this.dropdownFps.value);
			}
			if (flag5)
			{
				QualitySettings.vSyncCount = (this.toggleVsync.isOn ? 1 : 0);
				PlayerPrefs.SetInt(this.playerPrefsPrefix + "setVsync", QualitySettings.vSyncCount);
			}
			if (this.dropdownRegion)
			{
				if (this.dropdownRegion.value == 0)
				{
					PlayerPrefs.SetString("Settings_MyRegion", "eu");
				}
				if (this.dropdownRegion.value == 1)
				{
					PlayerPrefs.SetString("Settings_MyRegion", "us");
				}
				if (this.dropdownRegion.value == 2)
				{
					PlayerPrefs.SetString("Settings_MyRegion", "asia");
				}
				if (this.dropdownRegion.value == 3)
				{
					PlayerPrefs.SetString("Settings_MyRegion", "sa");
				}
				PlayerPrefs.SetInt("Settings_regionDD_value", this.dropdownRegion.value);
			}
		}

		private void Update()
		{
		}

		public void SetShadowResolution()
		{
			float value = this.shadowResolutionSlider.value;
			if (0f.Equals(value))
			{
				QualitySettings.shadowResolution = 0;
				this.shadowText.text = "Shadow Resolution (Low)";
				PlayerPrefs.SetInt("SaveShadowRes", 0);
				return;
			}
			if (1f.Equals(value))
			{
				QualitySettings.shadowResolution = 1;
				this.shadowText.text = "Shadow Resolution (Medium)";
				PlayerPrefs.SetInt("SaveShadowRes", 1);
				return;
			}
			if (2f.Equals(value))
			{
				QualitySettings.shadowResolution = 2;
				this.shadowText.text = "Shadow Resolution (High)";
				PlayerPrefs.SetInt("SaveShadowRes", 2);
				return;
			}
			if (!3f.Equals(value))
			{
				return;
			}
			QualitySettings.shadowResolution = 3;
			this.shadowText.text = "Shadow Resolution (Ultra)";
			PlayerPrefs.SetInt("SaveShadowRes", 3);
		}

		private IEnumerator waitshdowres(int jack)
		{
			yield return new WaitForSeconds(1f);
			this.SetShadowResolution2(jack);
			yield break;
		}

		public void SetShadowResolution2(int LOL)
		{
			switch (LOL)
			{
			case 0:
				QualitySettings.shadowResolution = 0;
				return;
			case 1:
				QualitySettings.shadowResolution = 1;
				return;
			case 2:
				QualitySettings.shadowResolution = 2;
				return;
			case 3:
				QualitySettings.shadowResolution = 3;
				return;
			default:
				return;
			}
		}

		public void loadSettings()
		{
			bool flag = this.dropdownQuality != null;
			bool flag2 = this.toggleFullScreen != null;
			bool flag3 = this.dropdownResolution != null;
			bool flag4 = this.dropdownFps != null;
			bool flag5 = this.toggleVsync != null;
			bool flag6 = this.dropdownFullScreenMode != null;
			if (!this.shadowResolutionSlider)
			{
				if (PlayerPrefs.HasKey("SaveShadowRes"))
				{
					base.StartCoroutine(this.waitshdowres(PlayerPrefs.GetInt("SaveShadowRes")));
					Debug.Log("PASSAGE ICI : " + PlayerPrefs.GetInt("SaveShadowRes"));
				}
				else
				{
					QualitySettings.shadowResolution = 3;
					PlayerPrefs.SetInt("SaveShadowRes", 3);
					Debug.Log("PASSAGE ICI : set sur 3");
				}
			}
			else
			{
				this.shadowResolutionSlider.value = (float)PlayerPrefs.GetInt("SaveShadowRes");
				this.SetShadowResolution2(PlayerPrefs.GetInt("SaveShadowRes"));
				Debug.Log("PASSAGE ICI");
			}
			if (flag)
			{
				this.dropdownQuality.value = PlayerPrefs.GetInt(this.playerPrefsPrefix + "qualityIndex", this.dropdownQuality.value);
			}
			if (flag2)
			{
				this.toggleFullScreen.isOn = (PlayerPrefs.GetInt(this.playerPrefsPrefix + "setFullScreen", this.toggleFullScreen.isOn ? 1 : 0) == 1);
			}
			if (flag3)
			{
				this.dropdownResolution.value = PlayerPrefs.GetInt(this.playerPrefsPrefix + "resolutionIndex", this.dropdownResolution.value);
			}
			if (flag4)
			{
				this.dropdownFps.value = PlayerPrefs.GetInt(this.playerPrefsPrefix + "fpsIndex", this.dropdownFps.value);
			}
			if (flag5)
			{
				this.toggleVsync.isOn = (PlayerPrefs.GetInt(this.playerPrefsPrefix + "setVsync", this.toggleVsync.isOn ? 1 : 0) == 1);
			}
			if (flag6)
			{
				this.dropdownFullScreenMode.value = PlayerPrefs.GetInt(this.playerPrefsPrefix + "fullScreenModeIndex", this.dropdownFullScreenMode.value);
			}
			if (this.dropdownRegion)
			{
				this.dropdownRegion.value = PlayerPrefs.GetInt("Settings_regionDD_value");
			}
		}

		public void updateVsyncFps()
		{
			if (this.dropdownFps != null && this.toggleVsync != null)
			{
				this.dropdownFps.interactable = !this.toggleVsync.isOn;
				return;
			}
			Debug.LogWarning("updateVsyncFps: dropdownFps and toggleVsync has to be set or remove the function event");
		}

		public void start()
		{
			this.activateSettings();
			SceneManager.LoadSceneAsync(this.loadNextSceneName, 0);
		}

		public void quit()
		{
			Application.Quit();
		}

		public string playerPrefsPrefix = "";

		public string loadNextSceneName;

		[Tooltip("Sets the resolution of the launcher scene to those values and uses WINDOW mode. Disable this if you want the launcher to be in fullscreen/what Unity has detected on start/what you have set in Project Settings with default resolution.")]
		public bool enforceResolution;

		public int width;

		public int height;

		[Header("Full Screen Mode")]
		[Tooltip("IF you allow full screen mode, this is how it will be set. Please see the manual for more details.")]
		public LauncherManager.FullScreenModeEn userDefinedFullScreenMode;

		[Header("FPS Settings")]
		public List<FpsInfo> availableFps = new List<FpsInfo>();

		public int defaultFpsIndex;

		[Header("Available UI Settings")]
		public Dropdown dropdownResolution;

		public Dropdown dropdownQuality;

		public Dropdown dropdownFps;

		public Dropdown dropdownRegion;

		public Dropdown dropdownFullScreenMode;

		public Toggle toggleFullScreen;

		public Toggle toggleVsync;

		public Slider shadowResolutionSlider;

		public Text shadowText;

		public Slider TexturequalitySlider;

		public Text TexturequalitySlidertxt;

		private List<ResolutionInfo> resolutions = new List<ResolutionInfo>();

		private string[] qualitySettingsNames;

		private List<string> fullScreenModeNames = new List<string>();

		public enum FullScreenModeEn
		{
			_FullScreenWindow,
			_MaximizedWindow,
			_ExclusiveFullScreen
		}
	}
}
