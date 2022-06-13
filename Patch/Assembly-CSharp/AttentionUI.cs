using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AttentionUI : MonoBehaviour
{
	private void Awake()
	{
		this.PCTextObject.SetActive(!Application.isMobilePlatform);
		this.MobileTextObject.SetActive(Application.isMobilePlatform);
	}

	private void Update()
	{
		if (Input.touchCount > 0 || Input.anyKey)
		{
			UnityEngine.Object.Destroy(this);
			SoundControllerInUI.PlayAudioClip(this.ClickClip);
			LoadingScreenUI.LoadScene(this.NextSceneName, LoadSceneMode.Single);
		}
	}

	[SerializeField]
	private AudioClip ClickClip;

	[SerializeField]
	private string NextSceneName = "MainMenuScene";

	[SerializeField]
	private GameObject PCTextObject;

	[SerializeField]
	private GameObject MobileTextObject;
}
