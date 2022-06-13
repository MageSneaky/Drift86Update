using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScreenUI : MonoBehaviour
{
	private IEnumerator LoadSceneCoroutine(AsyncOperation asyncOperation, float startProcent = 0f, float procentMultiplier = 1f, bool needDestroyObject = true, Action onCompleteAction = null)
	{
		while (!asyncOperation.isDone)
		{
			float num = startProcent * 100f + (float)Mathf.RoundToInt(asyncOperation.progress * 100f * procentMultiplier);
			this.LoadedProcentText.text = string.Format("Loading: {0}%", num);
			if (asyncOperation.progress >= 0.9f)
			{
				asyncOperation.allowSceneActivation = true;
			}
			yield return null;
		}
		if (needDestroyObject)
		{
			if (WorldLoading.IsMultiplayer && SceneManager.sceneCount > 1)
			{
				this.LoadedProcentText.text = "Wait other players...";
				for (;;)
				{
					if (!PhotonNetwork.CurrentRoom.Players.Any((KeyValuePair<int, Player> p) => !p.Value.CustomProperties.ContainsKey("il") || !(bool)p.Value.CustomProperties["il"]))
					{
						break;
					}
					yield return null;
				}
				Debug.Log("AAAAAAAAAAAAAA");
				yield return new WaitForSeconds(B.MultiplayerSettings.WaitOtherPlayersTime);
			}
			LoadingScreenUI.IsLoaded = true;
			float timer = this.HideObjectsTime;
			while (timer > 0f)
			{
				float newAlpha = timer / this.HideObjectsTime;
				this.BackGroundInage.SetAlpha(newAlpha);
				this.LoadedProcentText.SetAlpha(newAlpha);
				timer -= Time.deltaTime;
				yield return null;
			}
			Object.Destroy(base.gameObject);
			LoadingScreenUI.Instance = null;
		}
		onCompleteAction.SafeInvoke();
		yield break;
	}

	public static bool IsLoaded { get; private set; }

	public static void LoadScene(string sceneName, LoadSceneMode mode = 0)
	{
		if (LoadingScreenUI.Instance != null)
		{
			Debug.LogError("Loading in process!");
			return;
		}
		LoadingScreenUI.IsLoaded = false;
		LoadingScreenUI.Instance = Object.Instantiate<LoadingScreenUI>(B.ResourcesSettings.LoadingScreenUI);
		Object.DontDestroyOnLoad(LoadingScreenUI.Instance);
		if (sceneName == "DriftRegimeScene")
		{
			AsyncOperation asyncOperation = SceneManager.LoadSceneAsync("FreeDriftRegimeScenePort", mode);
			asyncOperation.allowSceneActivation = false;
			LoadingScreenUI.Instance.StartCoroutine(LoadingScreenUI.Instance.LoadSceneCoroutine(asyncOperation, 0f, 1f, true, null));
			return;
		}
		AsyncOperation asyncOperation2 = SceneManager.LoadSceneAsync(sceneName, mode);
		asyncOperation2.allowSceneActivation = false;
		LoadingScreenUI.Instance.StartCoroutine(LoadingScreenUI.Instance.LoadSceneCoroutine(asyncOperation2, 0f, 1f, true, null));
	}

	public static void LoadScene(string sceneName, string regimeSceneName)
	{
		if (LoadingScreenUI.Instance != null)
		{
			Debug.LogError("Loading in process!");
			return;
		}
		LoadingScreenUI.IsLoaded = false;
		LoadingScreenUI.Instance = Object.Instantiate<LoadingScreenUI>(B.ResourcesSettings.LoadingScreenUI);
		Object.DontDestroyOnLoad(LoadingScreenUI.Instance);
		LoadingScreenUI.CurrentLevelName = sceneName;
		LoadingScreenUI.CurrentRegimeSceneName = regimeSceneName;
		AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName, 0);
		asyncOperation.allowSceneActivation = false;
		Action onCompleteAction = delegate()
		{
			asyncOperation = SceneManager.LoadSceneAsync(regimeSceneName, 1);
			asyncOperation.allowSceneActivation = false;
			LoadingScreenUI.Instance.StartCoroutine(LoadingScreenUI.Instance.LoadSceneCoroutine(asyncOperation, 0.7f, 0.3f, true, null));
		};
		LoadingScreenUI.Instance.StartCoroutine(LoadingScreenUI.Instance.LoadSceneCoroutine(asyncOperation, 0f, 0.7f, false, onCompleteAction));
	}

	public static void ReloadCurrentScene()
	{
		LoadingScreenUI.LoadScene(LoadingScreenUI.CurrentLevelName, LoadingScreenUI.CurrentRegimeSceneName);
	}

	[SerializeField]
	private TextMeshProUGUI LoadedProcentText;

	[SerializeField]
	private Image BackGroundInage;

	[SerializeField]
	private float HideObjectsTime = 0.5f;

	private static LoadingScreenUI Instance;

	private static string CurrentLevelName;

	private static string CurrentRegimeSceneName;
}
