using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DriftRatingOfPlayersUI : MonoBehaviour
{
	private GameController GameController
	{
		get
		{
			return GameController.Instance;
		}
	}

	private List<CarStatisticsDriftRegime> AllCars
	{
		get
		{
			return this.DriftRaceEntity.DriftStatistics;
		}
	}

	public void Start()
	{
		this.DriftRaceEntity = (GameController.RaceEntity as DriftRaceEntity);
		if (this.DriftRaceEntity == null)
		{
			Debug.LogError("The RaceEntity is not DriftRaceEntity");
			UnityEngine.Object.Destroy(this);
			return;
		}
		if (this.AllCars.Count <= 1 && !WorldLoading.IsMultiplayer)
		{
			base.gameObject.SetActive(false);
			return;
		}
		GameController gameController = this.GameController;
		gameController.RatingOfPlayersChanged = (Action)Delegate.Combine(gameController.RatingOfPlayersChanged, new Action(this.UpdateRating));
		DriftRaceEntity driftRaceEntity = this.DriftRaceEntity;
		driftRaceEntity.MultiplayerCarAdded = (Action)Delegate.Combine(driftRaceEntity.MultiplayerCarAdded, new Action(this.UpdatePlayersList));
		this.UpdatePlayersList();
		this.PlayerUIRef.SetActive(false);
	}

	public void Update()
	{
	}

	public void UpdatePlayersList()
	{
		foreach (CarStatisticsDriftRegime key in this.AllCars)
		{
			if (!this.PanelCarStatisticsDict.ContainsKey(key))
			{
				DriftRatingPlayerUI driftRatingPlayerUI = UnityEngine.Object.Instantiate<DriftRatingPlayerUI>(this.PlayerUIRef, this.PlayerUIRef.transform.parent);
				driftRatingPlayerUI.SetActive(true);
				this.PanelCarStatisticsDict.Add(key, driftRatingPlayerUI);
			}
		}
		this.UpdateRating(true);
	}

	public void TestUpdate()
	{
		this.DriftRaceEntity = (GameController.RaceEntity as DriftRaceEntity);
	}

	public void UpdateRating()
	{
		this.UpdateRating(false);
	}

	private void UpdateRating(bool forceSetPositions = false)
	{
		base.StopAllCoroutines();
		for (int i = 0; i < this.AllCars.Count; i++)
		{
			CarStatisticsDriftRegime carStatisticsDriftRegime = this.AllCars[i];
			DriftRatingPlayerUI driftRatingPlayerUI;
			if (!this.PanelCarStatisticsDict.TryGetValue(carStatisticsDriftRegime, out driftRatingPlayerUI))
			{
				Debug.LogErrorFormat("Panel for player({0}) not found", new object[]
				{
					carStatisticsDriftRegime.PlayerName
				});
			}
			else
			{
				Vector2 vector = this.FirstPanelPosition + this.OffsetToNextPosition * (float)i;
				if (!SceneManager.GetActiveScene().name.Contains("Persistant"))
				{
					driftRatingPlayerUI.UpdateData(string.Format("{0}:{1}", i + 1, carStatisticsDriftRegime.PlayerName), carStatisticsDriftRegime.TotalScore.ToInt());
				}
				else
				{
					driftRatingPlayerUI.UpdateData(string.Format(carStatisticsDriftRegime.PlayerName, Array.Empty<object>()), carStatisticsDriftRegime.TotalScore.ToInt());
				}
				driftRatingPlayerUI.SetActive(i + 1 <= this.MaxPanels);
				if (!base.gameObject.activeInHierarchy || forceSetPositions)
				{
					driftRatingPlayerUI.Rect.anchoredPosition = vector;
				}
				else
				{
					base.StartCoroutine(this.MoveToPosition(driftRatingPlayerUI, vector));
				}
			}
		}
	}

	private IEnumerator MoveToPosition(DriftRatingPlayerUI panel, Vector2 targetPos)
	{
		while (!Mathf.Approximately(panel.Rect.anchoredPosition.x, targetPos.x))
		{
			yield return null;
			panel.Rect.anchoredPosition = Vector2.MoveTowards(panel.Rect.anchoredPosition, targetPos, Time.deltaTime * this.MovePanelSpeed);
		}
		yield break;
	}

	[SerializeField]
	private DriftRatingPlayerUI PlayerUIRef;

	[SerializeField]
	private Vector2 FirstPanelPosition;

	[SerializeField]
	private Vector2 OffsetToNextPosition;

	[SerializeField]
	private float MovePanelSpeed = 400f;

	[SerializeField]
	private int MaxPanels = 6;

	private DriftRaceEntity DriftRaceEntity;

	private Dictionary<CarStatisticsDriftRegime, DriftRatingPlayerUI> PanelCarStatisticsDict = new Dictionary<CarStatisticsDriftRegime, DriftRatingPlayerUI>();
}
