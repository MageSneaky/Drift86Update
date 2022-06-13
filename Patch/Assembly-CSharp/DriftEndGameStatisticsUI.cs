using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DriftEndGameStatisticsUI : MonoBehaviour
{
	private GameController GameController
	{
		get
		{
			return GameController.Instance;
		}
	}

	private CarStatisticsDriftRegime PlayerStatistics
	{
		get
		{
			return this.DriftRaceEntity.PlayerDriftStatistics;
		}
	}

	public void Init()
	{
		this.DriftRaceEntity = (GameController.RaceEntity as DriftRaceEntity);
		if (this.DriftRaceEntity == null)
		{
			Debug.LogError("[DriftPanelUI] RaceEntity is not DriftRaceEntity");
			base.enabled = false;
		}
		if (WorldLoading.IsMultiplayer)
		{
			this.RestartGameButton.interactable = false;
		}
		else
		{
			this.RestartGameButton.onClick.AddListener(new UnityAction(this.RestartGame));
		}
		this.ExitToMainMenuButton.onClick.AddListener(new UnityAction(this.Exit));
		GameController gameController = this.GameController;
		gameController.OnEndGameAction = (Action)Delegate.Combine(gameController.OnEndGameAction, new Action(this.OnEndGame));
		base.gameObject.SetActive(false);
	}

	private IEnumerator ShowEndGameCoroutine()
	{
		for (;;)
		{
			if (!this.PlayerStatistics.InDrift)
			{
				if (!GameController.AllCars.Any((CarController c) => c != null && !c.PositioningCar.IsFinished))
				{
					break;
				}
			}
			yield return null;
		}
		this.Players.ForEach(delegate(DriftRatingPlayerUI p)
		{
			p.SetActive(false);
		});
		for (int i = 0; i < this.DriftRaceEntity.DriftStatistics.Count; i++)
		{
			if (i < this.Players.Count)
			{
				CarStatisticsDriftRegime carStatisticsDriftRegime = this.DriftRaceEntity.DriftStatistics[i];
				this.Players[i].SetActive(true);
				string playerName = string.Format("{0}: {1}", i + 1, carStatisticsDriftRegime.PlayerName);
				this.Players[i].UpdateData(playerName, carStatisticsDriftRegime.TotalScore.ToInt());
			}
		}
		int num = 0;
		if (WorldLoading.HasLoadingParams)
		{
			int num2 = this.DriftRaceEntity.DriftStatistics.IndexOf(this.PlayerStatistics);
			int count = this.DriftRaceEntity.DriftStatistics.Count;
			num = ((float)(count - num2) / (float)count * WorldLoading.LoadingTrack.MoneyForFirstPlace).ToInt();
			num += (this.PlayerStatistics.TotalScore * B.GameSettings.DriftRegimeSettings.MoneyForDriftMultiplier).ToInt();
			num = Mathf.RoundToInt((float)num * 0.01f) * 100;
			CarStatisticsDriftRegime playerStatistics = this.DriftRaceEntity.PlayerDriftStatistics;
			if (this.DriftRaceEntity.DriftStatistics.All((CarStatisticsDriftRegime s) => s == playerStatistics || s.TotalScore < playerStatistics.TotalScore))
			{
				PlayerProfile.SetTrackAsComplited(WorldLoading.LoadingTrack);
			}
		}
		PlayerProfile.Money += num;
		string bestScoreCar = WorldLoading.PlayerCar ? WorldLoading.PlayerCar.CarCaption : GameController.PlayerCar.gameObject.name;
		int totalScore = PlayerProfile.TotalScore;
		int bestScore = PlayerProfile.BestScore;
		float raceTime = PlayerProfile.RaceTime;
		float totalDistance = PlayerProfile.TotalDistance;
		int num3 = Mathf.RoundToInt(this.PlayerStatistics.BestScore - (float)bestScore);
		this.PrevTotalScoreText.text = string.Format("{0}\n + {1}", totalScore, this.PlayerStatistics.TotalScore.ToString("########0"));
		PlayerProfile.TotalScore += Mathf.RoundToInt(this.PlayerStatistics.TotalScore);
		this.PrevTotalScoreText.color = this.ResultBetterColor;
		this.PrevBestScoreText.text = string.Format("{0}\n{1}{2}", bestScore, (num3 > 0) ? "+" : "", num3);
		if ((float)PlayerProfile.BestScore < this.PlayerStatistics.BestScore)
		{
			PlayerProfile.BestScore = Mathf.RoundToInt(this.PlayerStatistics.BestScore);
			PlayerProfile.BestScoreCar = bestScoreCar;
			this.PrevBestScoreText.color = this.ResultBetterColor;
		}
		else
		{
			this.PrevBestScoreText.color = this.ResultWorseColor;
		}
		this.PrevRaceTimeText.text = string.Format("{0}\n+{1}", raceTime.ToStringTime(), this.PlayerStatistics.TotalRaceTime.ToStringTime());
		PlayerProfile.RaceTime += (float)Mathf.RoundToInt(this.PlayerStatistics.TotalRaceTime);
		this.PrevRaceTimeText.color = this.ResultBetterColor;
		int num4 = (int)PositioningSystem.PositioningAndAiPath.Length * WorldLoading.LapsCount;
		this.PrevTotalDistanceText.text = string.Format("{0}\n+{1}", totalDistance.ToString(), num4.ToString(""));
		PlayerProfile.TotalDistance += (float)num4;
		this.PrevTotalDistanceText.color = this.ResultBetterColor;
		this.MoneyCaptionText.text = string.Format("+${0}", num);
		this.EndTotalScoreText.text = this.PlayerStatistics.TotalScore.ToString("########0");
		this.EndBestScoreText.text = this.PlayerStatistics.BestScore.ToString("########0");
		this.EndTotalTimeText.text = this.PlayerStatistics.TotalRaceTime.ToStringTime();
		this.TotalDistanceText.text = num4.ToString();
		yield break;
	}

	private void OnEndGame()
	{
		Cursor.visible = true;
		base.gameObject.SetActive(true);
		this.ExitToMainMenuButton.Select();
		this.EndGameStatisticHolder.SetTrigger("Show");
		base.StartCoroutine(this.ShowEndGameCoroutine());
	}

	private void RestartGame()
	{
		LoadingScreenUI.ReloadCurrentScene();
	}

	private void Exit()
	{
		LoadingScreenUI.LoadScene(B.GameSettings.MainMenuSceneName, LoadSceneMode.Single);
	}

	private void OnDestroy()
	{
		if (GameController.Instance != null)
		{
			GameController gameController = this.GameController;
			gameController.OnEndGameAction = (Action)Delegate.Remove(gameController.OnEndGameAction, new Action(this.OnEndGame));
		}
	}

	[SerializeField]
	private Animator EndGameStatisticHolder;

	[SerializeField]
	private Button RestartGameButton;

	[SerializeField]
	private Button ExitToMainMenuButton;

	[SerializeField]
	private TextMeshProUGUI MoneyCaptionText;

	[SerializeField]
	private TextMeshProUGUI EndTotalScoreText;

	[SerializeField]
	private TextMeshProUGUI PrevTotalScoreText;

	[SerializeField]
	private TextMeshProUGUI EndBestScoreText;

	[SerializeField]
	private TextMeshProUGUI PrevBestScoreText;

	[SerializeField]
	private TextMeshProUGUI EndTotalTimeText;

	[SerializeField]
	private TextMeshProUGUI PrevRaceTimeText;

	[SerializeField]
	private TextMeshProUGUI TotalDistanceText;

	[SerializeField]
	private TextMeshProUGUI PrevTotalDistanceText;

	[SerializeField]
	private Color ResultWorseColor;

	[SerializeField]
	private Color ResultBetterColor;

	[SerializeField]
	private List<DriftRatingPlayerUI> Players = new List<DriftRatingPlayerUI>();

	private DriftRaceEntity DriftRaceEntity;
}
