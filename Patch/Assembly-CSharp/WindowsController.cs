using System;
using System.Collections;
using System.Collections.Generic;
using SpielmannSpiel_Launcher;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WindowsController : Singleton<WindowsController>
{
	public Window CurrentWindow { get; private set; }

	public bool HasWindowsHistory
	{
		get
		{
			return this.WindowsHistory.Count > 0;
		}
	}

	protected override void AwakeSingleton()
	{
		Window[] array = Object.FindObjectsOfType<Window>();
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(false);
		}
	}

	private void Start()
	{
		if (this.MainWindow != null)
		{
			this.MainWindow.Open();
			this.CurrentWindow = this.MainWindow;
		}
		Cursor.visible = true;
	}

	private void Update()
	{
		if (this.HasNewWindowInFrame)
		{
			this.HasNewWindowInFrame = false;
		}
		else if ((Input.GetKeyDown(27) || Input.GetKeyDown(351)) && this.CurrentWindow != null && this.CurrentWindow != this.MainWindow)
		{
			this.OnBack(false);
		}
		if (this.multiplayerlobby)
		{
			if (this.multiplayerlobby.activeSelf)
			{
				this.ChatCanvas.SetActive(true);
			}
			else
			{
				this.ChatCanvas.SetActive(false);
			}
		}
		this.BackButton.SetActive(this.HasWindowsHistory);
	}

	public void OpenWindow(Window window)
	{
		if (this.CurrentWindow == window)
		{
			return;
		}
		this.CloseCurrent();
		this.WindowsHistory.Add(window);
		this.CurrentWindow = window;
		this.CurrentWindow.Open();
		this.HasNewWindowInFrame = true;
	}

	public void OnBack(bool ignoreCustomBackAction = false)
	{
		if (Object.FindObjectOfType<LauncherManager>())
		{
			Object.FindObjectOfType<LauncherManager>().activateSettings();
			Debug.Log("OK SALUT");
		}
		AudioSettingsMenu[] array = Object.FindObjectsOfType<AudioSettingsMenu>();
		if (array.Length != 0 && SceneManager.GetActiveScene().name == "MainMenuScene")
		{
			foreach (AudioSettingsMenu audioSettingsMenu in array)
			{
				if (audioSettingsMenu.gameObject.tag == "ui")
				{
					audioSettingsMenu.SaveMenuVariables();
				}
			}
		}
		else if (Object.FindObjectOfType<AudioSettingsMenu>())
		{
			Object.FindObjectOfType<AudioSettingsMenu>().SaveMenuVariables();
		}
		if (!ignoreCustomBackAction && this.CurrentWindow != null && this.CurrentWindow.CustomBackAction != null)
		{
			this.CurrentWindow.CustomBackAction.SafeInvoke();
			return;
		}
		if (this.WindowsHistory.Count > 0)
		{
			this.WindowsHistory.RemoveAt(this.WindowsHistory.Count - 1);
		}
		this.CloseCurrent();
		if (this.WindowsHistory.Count > 0)
		{
			this.CurrentWindow = this.WindowsHistory[this.WindowsHistory.Count - 1];
		}
		else
		{
			this.CurrentWindow = this.MainWindow;
			if (base.GetComponent<SI_LBManager>())
			{
				base.GetComponent<SI_LBManager>().CloseDisplayLB();
			}
		}
		if (this.CurrentWindow != null)
		{
			this.CurrentWindow.Open();
		}
	}

	private void CloseCurrent()
	{
		if (this.CurrentWindow != null)
		{
			this.CurrentWindow.Close();
			this.CurrentWindow = null;
		}
	}

	private IEnumerator waiting()
	{
		yield return new WaitForSeconds(0.5f);
		if (this.MainWindow2 && !this.MainWindow2.gameObject.activeSelf)
		{
			this.BackButton.SetActive(false);
		}
		yield break;
	}

	[SerializeField]
	private Window MainWindow;

	[SerializeField]
	private Window MainWindow2;

	[SerializeField]
	private GameObject BackButton;

	[SerializeField]
	private GameObject OptimizedAudio;

	[SerializeField]
	private GameObject ChatCanvas;

	[SerializeField]
	private GameObject multiplayerlobby;

	private List<Window> WindowsHistory = new List<Window>();

	private bool HasNewWindowInFrame;
}
