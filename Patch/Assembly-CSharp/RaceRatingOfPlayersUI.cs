using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaceRatingOfPlayersUI : MonoBehaviour
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

	private List<CarStatistic> AllCars
	{
		get
		{
			return this.RaceEntity.CarsStatistics;
		}
	}

	private void Start()
	{
		if (this.AllCars.Count <= 1 && !WorldLoading.IsMultiplayer)
		{
			base.gameObject.SetActive(false);
			return;
		}
		this.PlayerUIRef.SetActive(false);
		GameController gameController = this.GameController;
		gameController.RatingOfPlayersChanged = (Action)Delegate.Combine(gameController.RatingOfPlayersChanged, new Action(this.UpdateRating));
		RaceEntity raceEntity = this.RaceEntity;
		raceEntity.MultiplayerCarAdded = (Action)Delegate.Combine(raceEntity.MultiplayerCarAdded, new Action(this.UpdatePlayersList));
		this.UpdatePlayersList();
	}

	private void UpdatePlayersList()
	{
		foreach (CarStatistic key in this.AllCars)
		{
			if (!this.PanelCarStatisticsDict.ContainsKey(key))
			{
				RaceRatingPlayerUI raceRatingPlayerUI = Object.Instantiate<RaceRatingPlayerUI>(this.PlayerUIRef, this.PlayerUIRef.transform.parent);
				raceRatingPlayerUI.SetActive(true);
				this.PanelCarStatisticsDict.Add(key, raceRatingPlayerUI);
			}
		}
		this.UpdateRating(true);
	}

	private void UpdateRating()
	{
		this.UpdateRating(false);
	}

	private void UpdateRating(bool forceSetPositions = false)
	{
		base.StopAllCoroutines();
		for (int i = 0; i < this.AllCars.Count; i++)
		{
			CarStatistic carStatistic = this.AllCars[i];
			RaceRatingPlayerUI raceRatingPlayerUI;
			if (!this.PanelCarStatisticsDict.TryGetValue(carStatistic, out raceRatingPlayerUI))
			{
				Debug.LogErrorFormat("Panel for player({0}) not found", new object[]
				{
					carStatistic.PlayerName
				});
			}
			else
			{
				Vector2 vector = this.FirstPanelPosition + this.OffsetToNextPosition * (float)i;
				raceRatingPlayerUI.UpdateData(carStatistic.PlayerName, i + 1, "");
				raceRatingPlayerUI.SetActive(i + 1 <= this.MaxPanels);
				if (!base.gameObject.activeInHierarchy || forceSetPositions)
				{
					raceRatingPlayerUI.Rect.anchoredPosition = vector;
				}
				else
				{
					base.StartCoroutine(this.MoveToPosition(raceRatingPlayerUI, vector));
				}
			}
		}
	}

	private IEnumerator MoveToPosition(RaceRatingPlayerUI panel, Vector2 targetPos)
	{
		while (!Mathf.Approximately(panel.Rect.anchoredPosition.x, targetPos.x))
		{
			yield return null;
			panel.Rect.anchoredPosition = Vector2.MoveTowards(panel.Rect.anchoredPosition, targetPos, Time.deltaTime * this.MovePanelSpeed);
		}
		yield break;
	}

	[SerializeField]
	private RaceRatingPlayerUI PlayerUIRef;

	[SerializeField]
	private Vector2 FirstPanelPosition;

	[SerializeField]
	private Vector2 OffsetToNextPosition;

	[SerializeField]
	private float MovePanelSpeed = 400f;

	[SerializeField]
	private int MaxPanels = 6;

	private Dictionary<CarStatistic, RaceRatingPlayerUI> PanelCarStatisticsDict = new Dictionary<CarStatistic, RaceRatingPlayerUI>();
}
