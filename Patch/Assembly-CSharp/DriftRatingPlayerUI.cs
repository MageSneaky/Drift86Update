using System;
using TMPro;
using UnityEngine;

public class DriftRatingPlayerUI : MonoBehaviour
{
	public RectTransform Rect { get; private set; }

	private void Awake()
	{
		this.Rect = (base.transform as RectTransform);
	}

	public void UpdateData(string playerName, int score)
	{
		this.PlayerNameText.text = playerName;
		this.ScoreText.text = score.ToString();
	}

	public void Bye()
	{
		Object.Destroy(this);
	}

	[SerializeField]
	private TextMeshProUGUI PlayerNameText;

	[SerializeField]
	private TextMeshProUGUI ScoreText;
}
