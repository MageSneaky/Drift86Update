using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DriftPanelUI : MonoBehaviour
{
	private GameController GameController
	{
		get
		{
			return GameController.Instance;
		}
	}

	public CarStatisticsDriftRegime PlayerStatistics
	{
		get
		{
			return this.DriftRaceEntity.PlayerDriftStatistics;
		}
	}

	private void Start()
	{
		this.DriftRaceEntity = (GameController.RaceEntity as DriftRaceEntity);
		if (this.DriftRaceEntity == null)
		{
			Debug.LogError("[DriftPanelUI] RaceEntity is not DriftRaceEntity");
			base.enabled = false;
		}
		this.EndGameStatistics.Init();
		this.InGameStatistics.SetActive(true);
		GameController gameController = this.GameController;
		gameController.OnEndGameAction = (Action)Delegate.Combine(gameController.OnEndGameAction, new Action(this.OnEndGame));
	}

	private void Update()
	{
		if (this.PlayerStatistics == null)
		{
			return;
		}
		this.WrongDirectionObject.SetActive(this.PlayerStatistics.IsWrongDirection);
		this.DriftTimeImage.fillAmount = this.PlayerStatistics.DriftTimeProcent;
		if (this.PlayerStatistics.CurrentMultiplier == B.GameSettings.DriftRegimeSettings.MaxMultiplier)
		{
			this.MultiplierTimeImage.fillAmount = 1f;
		}
		else
		{
			this.MultiplierTimeImage.fillAmount = this.PlayerStatistics.MultiplierProcent;
		}
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
		this.LapText.text = string.Format("{0}/{1}", this.PlayerStatistics.CurrentLap, this.PlayerStatistics.LapsCount);
		this.TotalScoreText.text = this.PlayerStatistics.TotalScore.ToString("########0");
		this.BestScoreText.text = this.PlayerStatistics.BestScore.ToString("########0");
		this.CurrentScoreText.text = this.PlayerStatistics.CurrentScore.ToString("########0");
		this.MultiplierScoreText.SetActive(!Mathf.Approximately(0f, this.PlayerStatistics.CurrentScore));
		this.MultiplierScoreText.text = this.PlayerStatistics.CurrentMultiplier.ToString("x#");
		this.TotalRaceTimeText.text = this.PlayerStatistics.TotalRaceTime.ToStringTime();
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
	private TextMeshProUGUI LapText;

	[SerializeField]
	private TextMeshProUGUI TotalScoreText;

	[SerializeField]
	private TextMeshProUGUI BestScoreText;

	[SerializeField]
	private TextMeshProUGUI CurrentScoreText;

	[SerializeField]
	private TextMeshProUGUI MultiplierScoreText;

	[SerializeField]
	private GameObject WrongDirectionObject;

	[SerializeField]
	private Image DriftTimeImage;

	[SerializeField]
	private Image MultiplierTimeImage;

	[SerializeField]
	private GameObject InGameStatistics;

	[SerializeField]
	private DriftEndGameStatisticsUI EndGameStatistics;

	private int CurrentFrame;

	private DriftRaceEntity DriftRaceEntity;
}
