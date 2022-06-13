using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class GraphicsSettingsMenu : MonoBehaviour
{
	private void Start()
	{
		this.fpsNextPeriod = Time.realtimeSinceStartup + 0.2f;
		this.resolutionsPanelParent = this.resolutionsPanel.transform.parent.parent.gameObject;
		if (!this.usePersistentDatapath)
		{
			this.saveFileDataPath = Application.dataPath + "/QualitySettings.ini";
		}
		else
		{
			this.saveFileDataPath = Application.persistentDataPath + "/QualitySettings.ini";
		}
		this.SetValues();
	}

	private void Update()
	{
		if (this.setTextQual)
		{
			if (this.setTextQualDelay <= 0f)
			{
				switch (Mathf.RoundToInt(this.textureQualitySlider.value))
				{
				case 0:
					QualitySettings.masterTextureLimit = 3;
					break;
				case 1:
					QualitySettings.masterTextureLimit = 2;
					break;
				case 2:
					QualitySettings.masterTextureLimit = 1;
					break;
				case 3:
					QualitySettings.masterTextureLimit = 0;
					break;
				}
				this.setTextQual = false;
			}
			else
			{
				this.setTextQualDelay -= Time.deltaTime;
			}
		}
		if (this.showFPS)
		{
			this.fpsAccumulator++;
			if (Time.realtimeSinceStartup > this.fpsNextPeriod)
			{
				this.currentFps = (int)((float)this.fpsAccumulator / 0.2f);
				this.fpsAccumulator = 0;
				this.fpsNextPeriod += 0.2f;
				this.fpsCounterText.text = "FPS:" + this.currentFps;
				return;
			}
		}
		else
		{
			this.fpsCounterText.text = "";
		}
	}

	public void SetQuality()
	{
		int num = Mathf.RoundToInt(this.qualityLevelSlider.value);
		QualitySettings.SetQualityLevel(num, true);
		this.qualityText.text = QualitySettings.names[num];
		this.SetWindowedMode();
		this.SetVSync();
		this.SetAntiAlias();
		this.SetShadowResolution();
		this.SetTextureQuality();
		this.SetAnisotropicFiltering();
		this.SetAnisotropicFilteringLevel();
	}

	public void ShowFPS()
	{
		this.showFPS = !this.showFPS;
	}

	public void SetWindowedMode()
	{
		if (this.windowedModeToggle.isOn)
		{
			this.fullScreenMode = false;
		}
		else
		{
			this.fullScreenMode = true;
		}
		Screen.SetResolution(this.wantedResX, this.wantedResY, this.fullScreenMode);
	}

	public void SetVSync()
	{
		if (this.vSyncToggle.isOn)
		{
			QualitySettings.vSyncCount = 1;
			return;
		}
		QualitySettings.vSyncCount = 0;
	}

	public void SetAntiAlias()
	{
		switch (Mathf.RoundToInt(this.antiAliasSlider.value))
		{
		case 0:
			QualitySettings.antiAliasing = 0;
			this.antiAliasText.text = "Off";
			return;
		case 1:
			QualitySettings.antiAliasing = 2;
			this.antiAliasText.text = QualitySettings.antiAliasing.ToString() + "x ";
			return;
		case 2:
			QualitySettings.antiAliasing = 4;
			this.antiAliasText.text = QualitySettings.antiAliasing.ToString() + "x ";
			return;
		case 3:
			QualitySettings.antiAliasing = 8;
			this.antiAliasText.text = QualitySettings.antiAliasing.ToString() + "x ";
			return;
		default:
			return;
		}
	}

	public void SetShadowResolution()
	{
		switch (Mathf.RoundToInt(this.shadowResolutionSlider.value))
		{
		case 0:
			QualitySettings.shadowResolution = ShadowResolution.Low;
			this.shadowText.text = "Low";
			return;
		case 1:
			QualitySettings.shadowResolution = ShadowResolution.Medium;
			this.shadowText.text = "Medium";
			return;
		case 2:
			QualitySettings.shadowResolution = ShadowResolution.High;
			this.shadowText.text = "High";
			return;
		case 3:
			QualitySettings.shadowResolution = ShadowResolution.VeryHigh;
			this.shadowText.text = "Ultra";
			return;
		default:
			return;
		}
	}

	public void SetTextureQuality()
	{
		this.setTextQualDelay = 0.1f;
		this.setTextQual = true;
		switch (Mathf.RoundToInt(this.textureQualitySlider.value))
		{
		case 0:
			this.textureText.text = "Eighth";
			return;
		case 1:
			this.textureText.text = "Quarter";
			return;
		case 2:
			this.textureText.text = "Half";
			return;
		case 3:
			this.textureText.text = "Full";
			return;
		default:
			return;
		}
	}

	public void SetAnisotropicFiltering()
	{
		switch (Mathf.RoundToInt(this.anisotropicModeSlider.value))
		{
		case 0:
			QualitySettings.anisotropicFiltering = AnisotropicFiltering.Disable;
			this.anisotropicModeText.text = "Disabled";
			return;
		case 1:
			QualitySettings.anisotropicFiltering = AnisotropicFiltering.Enable;
			this.anisotropicModeText.text = "Enabled";
			return;
		case 2:
			QualitySettings.anisotropicFiltering = AnisotropicFiltering.ForceEnable;
			this.anisotropicModeText.text = "ForceEnabled";
			return;
		default:
			return;
		}
	}

	public void SetAnisotropicFilteringLevel()
	{
		int num = Mathf.RoundToInt(this.anisotropicLevelSlider.value);
		Texture.SetGlobalAnisotropicFilteringLimits(num, num);
		this.anisotropicLevelText.text = num.ToString();
	}

	private void SetValues()
	{
		this.resolutions = Screen.resolutions;
		int num = 0;
		int num2 = 0;
		for (int i = 0; i < this.resolutions.Length; i++)
		{
			if (this.resolutions[i].width != num && this.resolutions[i].height != num2)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.resButtonPrefab);
				gameObject.GetComponentInChildren<Text>().text = this.resolutions[i].width + "x" + this.resolutions[i].height;
				int index = i;
				gameObject.GetComponent<Button>().onClick.AddListener(delegate()
				{
					this.SetResolution(index);
				});
				gameObject.transform.SetParent(this.resolutionsPanel.transform, false);
				num = this.resolutions[i].width;
				num2 = this.resolutions[i].height;
			}
		}
		this.LoadMenuVariables();
		QualitySettings.SetQualityLevel(Mathf.RoundToInt(this.qualityLevelSlider.value), true);
		this.SetVSync();
		this.SetAntiAlias();
		this.SetShadowResolution();
		this.SetTextureQuality();
		this.SetAnisotropicFiltering();
		this.SetAnisotropicFilteringLevel();
	}

	public void SetResolution(int index)
	{
		this.wantedResX = this.resolutions[index].width;
		this.wantedResY = this.resolutions[index].height;
		Screen.SetResolution(this.wantedResX, this.wantedResY, this.fullScreenMode);
		this.currentResolutionText.text = this.wantedResX + "x" + this.wantedResY;
	}

	public void ShowResolutionOptions()
	{
		this.resolutionsPanelParent.SetActive(!this.resolutionsPanelParent.activeSelf);
	}

	public void SaveMenuVariables()
	{
		if (this.saveAs == GraphicsSettingsMenu.saveFormat.playerprefs)
		{
			PlayerPrefs.SetInt("graphicsPrefsSaved", 0);
			PlayerPrefs.SetInt("graphicsSlider", Mathf.RoundToInt(this.qualityLevelSlider.value));
			PlayerPrefs.SetInt("antiAliasSlider", Mathf.RoundToInt(this.antiAliasSlider.value));
			PlayerPrefs.SetInt("shadowResolutionSlider", Mathf.RoundToInt(this.shadowResolutionSlider.value));
			PlayerPrefs.SetInt("textureQualitySlider", Mathf.RoundToInt(this.textureQualitySlider.value));
			PlayerPrefs.SetInt("anisotropicModeSlider", Mathf.RoundToInt(this.anisotropicModeSlider.value));
			PlayerPrefs.SetInt("anisotropicLevelSlider", Mathf.RoundToInt(this.anisotropicLevelSlider.value));
			PlayerPrefs.SetInt("wantedResolutionX", this.wantedResX);
			PlayerPrefs.SetInt("wantedResolutionY", this.wantedResY);
			int value;
			if (!this.showFPS)
			{
				value = 0;
			}
			else
			{
				value = 1;
			}
			PlayerPrefs.SetInt("FPSToggle", value);
			if (this.vSyncToggle.isOn)
			{
				value = 1;
			}
			else
			{
				value = 0;
			}
			PlayerPrefs.SetInt("vSyncToggle", value);
			if (this.windowedModeToggle.isOn)
			{
				value = 1;
			}
			else
			{
				value = 0;
			}
			PlayerPrefs.SetInt("windowedModeToggle", value);
			return;
		}
		if (this.saveAs == GraphicsSettingsMenu.saveFormat.iniFile)
		{
			this.saveVars = new GraphicsSettingsMenu.MenuVariables
			{
				Qualitylevel = Mathf.RoundToInt(this.qualityLevelSlider.value),
				ShowFPS = this.showFPS,
				ResolutionX = this.wantedResX,
				ResolutionY = this.wantedResY,
				WindowedMode = this.windowedModeToggle.isOn,
				VSync = this.vSyncToggle.isOn,
				AntiAliaslevel = Mathf.RoundToInt(this.antiAliasSlider.value),
				ShadowResolution = Mathf.RoundToInt(this.shadowResolutionSlider.value),
				TextureQuality = Mathf.RoundToInt(this.textureQualitySlider.value),
				AnisotropicMode = Mathf.RoundToInt(this.anisotropicModeSlider.value),
				AnisotropicLevel = Mathf.RoundToInt(this.anisotropicLevelSlider.value),
				Warning = "Edit this file at your own risk!"
			};
			string contents = JsonUtility.ToJson(this.saveVars, true);
			File.WriteAllText(this.saveFileDataPath, contents);
			this.saveVars = null;
		}
	}

	private void LoadMenuVariables()
	{
		if (this.saveAs != GraphicsSettingsMenu.saveFormat.playerprefs)
		{
			if (this.saveAs == GraphicsSettingsMenu.saveFormat.iniFile)
			{
				if (File.Exists(this.saveFileDataPath))
				{
					string json = File.ReadAllText(this.saveFileDataPath);
					this.saveVars = JsonUtility.FromJson<GraphicsSettingsMenu.MenuVariables>(json);
					this.qualityLevelSlider.value = (float)this.saveVars.Qualitylevel;
					this.antiAliasSlider.value = (float)this.saveVars.AntiAliaslevel;
					this.anisotropicModeSlider.value = (float)this.saveVars.AnisotropicMode;
					this.anisotropicLevelSlider.value = (float)this.saveVars.AnisotropicLevel;
					this.shadowResolutionSlider.value = (float)this.saveVars.ShadowResolution;
					this.textureQualitySlider.value = (float)this.saveVars.TextureQuality;
					this.wantedResX = this.saveVars.ResolutionX;
					this.wantedResY = this.saveVars.ResolutionY;
					this.currentResolutionText.text = this.wantedResX + "x" + this.wantedResY;
					this.FPSToggle.isOn = this.saveVars.ShowFPS;
					this.showFPS = this.saveVars.ShowFPS;
					this.windowedModeToggle.isOn = this.saveVars.WindowedMode;
					this.vSyncToggle.isOn = this.saveVars.VSync;
					this.saveVars = null;
					return;
				}
				this.wantedResX = Screen.width;
				this.wantedResY = Screen.height;
				this.currentResolutionText.text = Screen.width + "x" + Screen.height;
			}
			return;
		}
		if (!PlayerPrefs.HasKey("graphicsPrefsSaved"))
		{
			this.wantedResX = Screen.width;
			this.wantedResY = Screen.height;
			this.currentResolutionText.text = Screen.width + "x" + Screen.height;
			return;
		}
		this.qualityLevelSlider.value = (float)PlayerPrefs.GetInt("graphicsSlider");
		this.antiAliasSlider.value = (float)PlayerPrefs.GetInt("antiAliasSlider");
		this.shadowResolutionSlider.value = (float)PlayerPrefs.GetInt("shadowResolutionSlider");
		this.textureQualitySlider.value = (float)PlayerPrefs.GetInt("textureQualitySlider");
		this.anisotropicModeSlider.value = (float)PlayerPrefs.GetInt("anisotropicModeSlider");
		this.anisotropicLevelSlider.value = (float)PlayerPrefs.GetInt("anisotropicLevelSlider");
		this.wantedResX = PlayerPrefs.GetInt("wantedResolutionX");
		this.wantedResY = PlayerPrefs.GetInt("wantedResolutionY");
		this.currentResolutionText.text = this.wantedResX + "x" + this.wantedResY;
		if (PlayerPrefs.GetInt("FPSToggle") == 1)
		{
			this.FPSToggle.isOn = true;
			this.showFPS = true;
		}
		else
		{
			this.FPSToggle.isOn = false;
			this.showFPS = false;
		}
		if (PlayerPrefs.GetInt("windowedModeToggle") == 1)
		{
			this.windowedModeToggle.isOn = true;
		}
		else
		{
			this.windowedModeToggle.isOn = false;
		}
		if (PlayerPrefs.GetInt("vSyncToggle") == 1)
		{
			this.vSyncToggle.isOn = true;
			return;
		}
		this.vSyncToggle.isOn = false;
	}

	public GraphicsSettingsMenu.saveFormat saveAs;

	[Tooltip("Check for IOS and Windows Store Apps.")]
	public bool usePersistentDatapath;

	[Header("THESE NEED TO BE DRAGGED IN")]
	public Slider qualityLevelSlider;

	public Slider antiAliasSlider;

	public Slider shadowResolutionSlider;

	public Slider textureQualitySlider;

	public Slider anisotropicModeSlider;

	public Slider anisotropicLevelSlider;

	public Text qualityText;

	public Text antiAliasText;

	public Text shadowText;

	public Text textureText;

	public Text anisotropicModeText;

	public Text anisotropicLevelText;

	public Text fpsCounterText;

	public GameObject resolutionsPanel;

	public GameObject resButtonPrefab;

	public GameObject menuTransform;

	public Text currentResolutionText;

	public Toggle FPSToggle;

	public Toggle windowedModeToggle;

	public Toggle vSyncToggle;

	private GameObject resolutionsPanelParent;

	private Resolution[] resolutions;

	private bool setMenu;

	private bool openMenu;

	private bool showFPS;

	private bool fullScreenMode;

	private bool toggleVSync;

	private const float fpsMeasurePeriod = 0.2f;

	private float fpsNextPeriod;

	private int fpsAccumulator;

	private int currentFps;

	private int wantedResX;

	private int wantedResY;

	private string saveFileDataPath;

	private GraphicsSettingsMenu.MenuVariables saveVars;

	private bool setTextQual;

	private float setTextQualDelay;

	public enum saveFormat
	{
		playerprefs,
		iniFile
	}

	private class MenuVariables
	{
		public int Qualitylevel;

		public bool ShowFPS;

		public int ResolutionX;

		public int ResolutionY;

		public bool WindowedMode;

		public bool VSync;

		public int AntiAliaslevel;

		public int ShadowResolution;

		public int TextureQuality;

		public int AnisotropicMode;

		public int AnisotropicLevel;

		public string Warning;
	}
}
