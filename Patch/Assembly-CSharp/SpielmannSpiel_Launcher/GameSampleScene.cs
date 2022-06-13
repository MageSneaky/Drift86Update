using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace SpielmannSpiel_Launcher
{
	public class GameSampleScene : MonoBehaviour
	{
		private void Start()
		{
			string[] names = QualitySettings.names;
			this.selectedQuality.text = names[QualitySettings.GetQualityLevel()];
			this.resolution.text = string.Format("{0}x{1}", Screen.width, Screen.height);
			this.otherInfos.text = string.Format("fullScreen: {0}\nfullScreenMode: {1}", Screen.fullScreen.ToString(), Screen.fullScreenMode.ToString());
		}

		private void Update()
		{
			this.fps.text = string.Format("{0:0.00}", 1f / Time.smoothDeltaTime);
		}

		public void back()
		{
			SceneManager.LoadScene(0, LoadSceneMode.Single);
		}

		public void loadExitScene()
		{
			SceneManager.LoadScene(SceneManager.sceneCountInBuildSettings - 1, LoadSceneMode.Single);
		}

		public void quit()
		{
			Application.Quit();
		}

		public Text fps;

		public Text selectedQuality;

		public Text resolution;

		public Text otherInfos;
	}
}
