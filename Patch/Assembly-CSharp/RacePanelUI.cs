using System;
using TMPro;
using UnityEngine;

public class RacePanelUI : MonoBehaviour
{
	private RaceEntity RaceEntity
	{
		get
		{
			return GameController.RaceEntity as RaceEntity;
		}
	}

	private GameController GameController
	{
		get
		{
			return GameController.Instance;
		}
	}

	private CarStatistic PlayerStatistics
	{
		get
		{
			return this.RaceEntity.PlayerStatistics;
		}
	}

	private void Start()
	{
		this.EndGameStatistics.Init();
		this.InGameStatistics.SetActive(true);
		GameController gameController = this.GameController;
		gameController.OnEndGameAction = (Action)Delegate.Combine(gameController.OnEndGameAction, new Action(this.OnEndGame));
		this.WrongDirectionObject.SetActive(false);
	}

	private void Update()
	{
		if (this.PlayerStatistics == null)
		{
			return;
		}
		this.WrongDirectionObject.SetActive(this.PlayerStatistics.IsWrongDirection);
		if (this.CurrentFrame >= this.UpdateFrameCount)
		{
			this.UpdateStatistics();
			this.CurrentFrame = 0;
			return;
		}
		this.CurrentFrame++;
	}

	private void UpdateStatistics()
	{
		this.TotalRaceTimeText.text = this.PlayerStatistics.TotalRaceTime.ToStringTime();
		this.CurrentLapTimeText.text = this.PlayerStatistics.CurrentLapTime.ToStringTime();
		this.BestLapTimeText.text = this.PlayerStatistics.BestLapTime.ToStringTime();
		this.LapText.text = string.Format("{0}/{1}", this.PlayerStatistics.CurrentLap, this.PlayerStatistics.LapsCount);
	}

	private void OnEndGame()
	{
		this.InGameStatistics.SetActive(false);
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
	private int UpdateFrameCount = 3;

	[SerializeField]
	private TextMeshProUGUI TotalRaceTimeText;

	[SerializeField]
	private TextMeshProUGUI CurrentLapTimeText;

	[SerializeField]
	private TextMeshProUGUI BestLapTimeText;

	[SerializeField]
	private TextMeshProUGUI LapText;

	[SerializeField]
	private GameObject WrongDirectionObject;

	[SerializeField]
	private GameObject InGameStatistics;

	[SerializeField]
	private RaceEndGameStatisticsUI EndGameStatistics;

	private int CurrentFrame;
}
