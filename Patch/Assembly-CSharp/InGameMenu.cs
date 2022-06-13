using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InGameMenu : MonoBehaviour
{
	private WindowsController Windows
	{
		get
		{
			return Singleton<WindowsController>.Instance;
		}
	}

	private void Start()
	{
		Cursor.visible = false;
		this.BlockerImage.SetActive(false);
		this.SetNextCameraButton.onClick.AddListener(delegate()
		{
			Singleton<CameraController>.Instance.SetNextCamera();
		});
		this.RestartCarButton.onClick.AddListener(delegate()
		{
			GameController.PlayerCar.ResetPosition();
		});
		this.PauseButton.onClick.AddListener(new UnityAction(this.Show));
		this.ContinueButton.onClick.AddListener(delegate()
		{
			this.Windows.OnBack(false);
		});
		this.SettingsButton.onClick.AddListener(new UnityAction(this.Settings));
		this.ExitButton.onClick.AddListener(new UnityAction(this.Exit));
		Window inGameMainMenu = this.InGameMainMenu;
		inGameMainMenu.OnDisableAction = (Action)Delegate.Combine(inGameMainMenu.OnDisableAction, new Action(this.OnDisableMainMenu));
		if (WorldLoading.IsMultiplayer)
		{
			this.RestartButton.interactable = false;
			return;
		}
		this.RestartButton.onClick.AddListener(new UnityAction(this.RestartScene));
	}

	private void Update()
	{
		if (this.Windows.CurrentWindow == null && !this.InGameMainMenu.gameObject.activeInHierarchy && !GameController.RaceIsEnded && (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Joystick1Button7)))
		{
			this.Show();
			this.ContinueButton.Select();
		}
	}

	private void OnDestroy()
	{
		base.StopAllCoroutines();
		Window inGameMainMenu = this.InGameMainMenu;
		inGameMainMenu.OnDisableAction = (Action)Delegate.Remove(inGameMainMenu.OnDisableAction, new Action(this.OnDisableMainMenu));
	}

	private void SetTimeScale(float scale)
	{
		if (!WorldLoading.IsMultiplayer)
		{
			Time.timeScale = scale;
			GameOptions.UpdateAudioMixer();
		}
	}

	private void Show()
	{
		Cursor.visible = true;
		this.BlockerImage.SetActive(true);
		this.Windows.OpenWindow(this.InGameMainMenu);
		this.SetTimeScale(0f);
	}

	private void OnDisableMainMenu()
	{
		if (this.Windows.HasWindowsHistory)
		{
			return;
		}
		Cursor.visible = false;
		this.BlockerImage.SetActive(false);
		this.SetTimeScale(1f);
	}

	private void RestartScene()
	{
		this.SetTimeScale(1f);
		LoadingScreenUI.ReloadCurrentScene();
	}

	private void Settings()
	{
		this.Windows.OpenWindow(this.InGameSettings);
	}

	private void Exit()
	{
		this.SetTimeScale(1f);
		GameController.LeaveRoom();
		LoadingScreenUI.LoadScene(B.GameSettings.MainMenuSceneName, LoadSceneMode.Single);
	}

	[SerializeField]
	private Transform BlockerImage;

	[SerializeField]
	private Button SetNextCameraButton;

	[SerializeField]
	private Button RestartCarButton;

	[SerializeField]
	private Button PauseButton;

	[SerializeField]
	private Button ContinueButton;

	[SerializeField]
	private Button RestartButton;

	[SerializeField]
	private Button SettingsButton;

	[SerializeField]
	private Button ExitButton;

	[SerializeField]
	private Window InGameMainMenu;

	[SerializeField]
	private Window InGameSettings;
}
