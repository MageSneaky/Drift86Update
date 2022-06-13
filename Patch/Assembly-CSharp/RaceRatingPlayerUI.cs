using System;
using TMPro;
using UnityEngine;

public class RaceRatingPlayerUI : MonoBehaviour
{
	public RectTransform Rect { get; private set; }

	private void Awake()
	{
		this.Rect = (base.transform as RectTransform);
	}

	public void UpdateData(string playerName, int position, string time = "")
	{
		this.PlayerNameText.text = playerName;
		this.PositionText.text = position.ToString();
		if (this.TimeText != null)
		{
			this.TimeText.text = time;
		}
	}

	[SerializeField]
	private TextMeshProUGUI PlayerNameText;

	[SerializeField]
	private TextMeshProUGUI PositionText;

	[SerializeField]
	private TextMeshProUGUI TimeText;
}
