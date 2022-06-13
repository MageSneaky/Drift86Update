using System;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class RoomItemUI : MonoBehaviour
{
	public RoomInfo Room { get; private set; }

	public void UpdateInfo(RoomInfo room, Sprite trackIcon, Sprite regimeIcon, string hostNickNameText, string selectedTrackText, string playersText, UnityAction onClickAction)
	{
		this.Room = room;
		this.TrackIcon.sprite = trackIcon;
		this.RegimeIcon.sprite = regimeIcon;
		this.HostNickNameText.text = hostNickNameText;
		this.SelectedTrackText.text = selectedTrackText;
		this.PlayersText.text = playersText;
		this.OnClickButton.onClick.RemoveAllListeners();
		this.OnClickButton.onClick.AddListener(onClickAction);
	}

	[SerializeField]
	private Image TrackIcon;

	[SerializeField]
	private Image RegimeIcon;

	[SerializeField]
	private TextMeshProUGUI HostNickNameText;

	[SerializeField]
	private TextMeshProUGUI SelectedTrackText;

	[SerializeField]
	private TextMeshProUGUI PlayersText;

	[SerializeField]
	private Button OnClickButton;
}
