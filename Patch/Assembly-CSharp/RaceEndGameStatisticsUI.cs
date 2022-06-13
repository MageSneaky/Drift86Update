using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RaceEndGameStatisticsUI : MonoBehaviour
{
	private GameController GameController
	{
		get
		{
			return GameController.Instance;
		}
	}

	private RaceEntity RaceEntity
	{
		get
		{
			return GameController.RaceEntity as RaceEntity;
		}
	}

	private CarStatistic PlayerStatistics
	{
		get
		{
			return this.RaceEntity.PlayerStatistics;
		}
	}

	public void Init()
	{
		this.ExitToMainMenuButton.onClick.AddListener(new UnityAction(this.Exit));
		GameController gameController = this.GameController;
		gameController.OnEndGameAction = (Action)Delegate.Combine(gameController.OnEndGameAction, new Action(this.OnEndGame));
		if (WorldLoading.IsMultiplayer)
		{
			this.RestartGameButton.interactable = false;
		}
		else
		{
			this.RestartGameButton.onClick.AddListener(new UnityAction(this.RestartGame));
		}
		base.gameObject.SetActive(false);
	}

	private IEnumerator ShowEndGameCoroutine()
	{
		if (!WorldLoading.HasLoadingParams)
		{
			yield break;
		}
		for (;;)
		{
			if (!GameController.AllCars.Any((CarController c) => !c.PositioningCar.IsFinished))
			{
				break;
			}
			yield return null;
		}
		this.Players.ForEach(delegate(RaceRatingPlayerUI p)
		{
			p.SetActive(false);
		});
		this.RaceEntity.CheckRatingOfPlayers();
		List<CarStatistic> list = (from c in this.RaceEntity.CarsStatistics
		orderby c.TotalRaceTime
		select c).ToList<CarStatistic>();
		float totalRaceTime = list[0].TotalRaceTime;
		for (int i = 0; i < list.Count; i++)
		{
			if (i < this.Players.Count)
			{
				CarStatistic carStatistic = list[i];
				this.Players[i].SetActive(true);
				string time;
				if (i == 0)
				{
					time = carStatistic.TotalRaceTime.ToStringTime();
				}
				else
				{
					time = string.Format("+{0}", (carStatistic.TotalRaceTime - totalRaceTime).ToStringTime());
				}
				this.Players[i].UpdateData(carStatistic.PlayerName, i + 1, time);
			}
		}
		int num = 0;
		if (WorldLoading.HasLoadingParams)
		{
			CarStatistic playerStatistics = this.RaceEntity.PlayerStatistics;
			int num2 = list.IndexOf(this.RaceEntity.PlayerStatistics);
			int count = list.Count;
			num = ((float)(count - num2) / (float)count * WorldLoading.LoadingTrack.MoneyForFirstPlace).ToInt();
			num = Mathf.RoundToInt((float)num * 0.01f) * 100;
			if (num2 == 0)
			{
				PlayerProfile.SetTrackAsComplited(WorldLoading.LoadingTrack);
			}
		}
		PlayerProfile.Money += num;
		if (!WorldLoading.PlayerCar)
		{
			string name = GameController.PlayerCar.gameObject.name;
		}
		else
		{
			string carCaption = WorldLoading.PlayerCar.CarCaption;
		}
		float raceTimeForTrack = PlayerProfile.GetRaceTimeForTrack(WorldLoading.LoadingTrack);
		float bestLapForTrack = PlayerProfile.GetBestLapForTrack(WorldLoading.LoadingTrack);
		float num3 = this.PlayerStatistics.TotalRaceTime - raceTimeForTrack;
		float num4 = this.PlayerStatistics.BestLapTime - bestLapForTrack;
		bool flag = Mathf.Approximately(raceTimeForTrack, 0f);
		bool flag2 = Mathf.Approximately(bestLapForTrack, 0f);
		this.PrevRaceTimeText.text = string.Format("{0}\n{1}{2}", raceTimeForTrack.ToStringTime(), (num3 > 0f && !flag) ? "+" : "-", num3.ToStringTime());
		if (num3 < 0f || flag)
		{
			PlayerProfile.SetRaceTimeForTrack(WorldLoading.LoadingTrack, this.PlayerStatistics.TotalRaceTime);
			this.PrevRaceTimeText.color = this.ResultBetterColor;
		}
		else
		{
			this.PrevRaceTimeText.color = this.ResultWorseColor;
		}
		this.PrevBestLapText.text = string.Format("{0}\n{1}{2}", bestLapForTrack.ToStringTime(), (num4 > 0f && !flag2) ? "+" : "-", num4.ToStringTime());
		if (num4 < 0f || flag2)
		{
			PlayerProfile.SetBestLapForTrack(WorldLoading.LoadingTrack, this.PlayerStatistics.BestLapTime);
			this.PrevBestLapText.color = this.ResultBetterColor;
		}
		else
		{
			this.PrevBestLapText.color = this.ResultWorseColor;
		}
		PlayerProfile.RaceTime += (float)Mathf.RoundToInt(this.PlayerStatistics.TotalRaceTime);
		int num5 = (int)PositioningSystem.PositioningAndAiPath.Length * WorldLoading.LapsCount;
		PlayerProfile.TotalDistance += (float)num5;
		this.MoneyCaptionText.text = string.Format("+${0}", num);
		this.RaceTimeText.text = this.PlayerStatistics.TotalRaceTime.ToStringTime();
		this.BestLapText.text = this.PlayerStatistics.BestLapTime.ToStringTime();
		yield break;
	}

	private void OnEndGame()
	{
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
		LoadingScreenUI.LoadScene(this.MainMenuSceneName, LoadSceneMode.Single);
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
	private string MainMenuSceneName = "MainMenuScene";

	[SerializeField]
	private Animator EndGameStatisticHolder;

	[SerializeField]
	private Button RestartGameButton;

	[SerializeField]
	private Button ExitToMainMenuButton;

	[SerializeField]
	private TextMeshProUGUI RaceTimeText;

	[SerializeField]
	private TextMeshProUGUI PrevRaceTimeText;

	[SerializeField]
	private TextMeshProUGUI BestLapText;

	[SerializeField]
	private TextMeshProUGUI PrevBestLapText;

	[SerializeField]
	private TextMeshProUGUI MoneyCaptionText;

	[SerializeField]
	private Color ResultWorseColor;

	[SerializeField]
	private Color ResultBetterColor;

	[SerializeField]
	private List<RaceRatingPlayerUI> Players = new List<RaceRatingPlayerUI>();
}
