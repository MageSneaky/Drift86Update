using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MainMenuUI : WindowWithShowHideAnimators
{
	protected override void Awake()
	{
		this.StartMultiplayerButton.onClick.AddListener(new UnityAction(this.StartMultiplayer));
		this.StartGameButton.onClick.AddListener(new UnityAction(this.StartGame));
		this.SettingsButton.onClick.AddListener(new UnityAction(this.Settings));
		this.ResultsButton.onClick.AddListener(new UnityAction(this.Results));
		this.QuitButton.onClick.AddListener(new UnityAction(this.Quit));
		base.Awake();
		GameObject[] array = GameObject.FindGameObjectsWithTag("SM");
		if (Object.FindObjectOfType<SI_GetUserData>() && array.Length >= 1)
		{
			this.MyFace = Object.FindObjectOfType<SI_GetUserData>().MyAvatar;
			this.MyFaceUI.texture = this.MyFace;
			this.MyName = Object.FindObjectOfType<SI_GetUserData>().playername;
			this.MyNameUI.text = this.MyName;
		}
	}

	private void StartGame()
	{
		WorldLoading.IsMultiplayer = false;
		Singleton<WindowsController>.Instance.OpenWindow(this.SelectTrackWindow);
	}

	private void StartMultiplayer()
	{
		WorldLoading.IsMultiplayer = true;
		Singleton<WindowsController>.Instance.OpenWindow(this.MultiplayerWindow);
	}

	private void Settings()
	{
		Singleton<WindowsController>.Instance.OpenWindow(this.SettingsWindow);
	}

	private void Results()
	{
		Singleton<WindowsController>.Instance.OpenWindow(this.ResultsWindow);
	}

	private void Quit()
	{
		Application.Quit();
	}

	private void OnGUI()
	{
		SneakyManager.GetPlayerPrefs();
		SneakyManager.DrawMenu();
	}

	[SerializeField]
	private Button StartMultiplayerButton;

	[SerializeField]
	private Button StartGameButton;

	[SerializeField]
	private Button SettingsButton;

	[SerializeField]
	private Button ResultsButton;

	[SerializeField]
	private Button QuitButton;

	[SerializeField]
	private Window MultiplayerWindow;

	[SerializeField]
	private Window SelectTrackWindow;

	[SerializeField]
	private Window SettingsWindow;

	[SerializeField]
	private Window ResultsWindow;

	[Space]
	[SerializeField]
	private Texture MyFace;

	[SerializeField]
	private string MyName;

	[SerializeField]
	private Text MyNameUI;

	[SerializeField]
	private RawImage MyFaceUI;
}
