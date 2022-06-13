using System;
using ExitGames.Client.Photon;
using GameBalance;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerItemInRoomUI : MonoBehaviour
{
	private void Update()
	{
		if (!this.TargetPlayer.IsLocal)
		{
			return;
		}
		if (this.Timer <= 0f)
		{
			this.UpdatePing();
			this.Timer = B.MultiplayerSettings.PingUpdateSettings;
		}
		this.Timer -= Time.deltaTime;
	}

	private void UpdatePing()
	{
		Hashtable hashtable = new Hashtable();
		hashtable["pt"] = PhotonNetwork.GetPing();
		this.TargetPlayer.SetCustomProperties(hashtable, null, null);
	}

	public void UpdateProperties(Player targetPlayer, Action kickAction)
	{
		this.TargetPlayer = targetPlayer;
		Hashtable customProperties = targetPlayer.CustomProperties;
		this.ReadyObject.SetActive((bool)customProperties["ir"]);
		this.NotReadyObject.SetActive(!(bool)customProperties["ir"]);
		this.PlayerNickNameText.text = this.TargetPlayer.NickName;
		this.PlayerCarText.text = (string)customProperties["cn"];
		if (kickAction == null)
		{
			this.KickButton.SetActive(false);
		}
		else
		{
			this.KickButton.interactable = !this.TargetPlayer.IsMasterClient;
			this.KickButton.SetActive(true);
			this.KickButton.onClick.RemoveAllListeners();
			this.KickButton.onClick.AddListener(delegate()
			{
				kickAction.SafeInvoke();
			});
		}
		if (this.SelectedTrackHolder != null)
		{
			this.trackName = (customProperties.ContainsKey("tn") ? ((string)customProperties["tn"]) : string.Empty);
			if (!string.IsNullOrEmpty(this.trackName))
			{
				TrackPreset trackPreset = B.MultiplayerSettings.AvailableTracksForMultiplayer.Find((TrackPreset t) => t.name == this.trackName);
				this.SelectedTrackHolder.SetActive(true);
				this.TrackIconImage.sprite = trackPreset.TrackIcon;
				this.RegimeIconImage.sprite = trackPreset.RegimeSettings.RegimeImage;
				this.TrackNameText.text = string.Format("{0}: {1}", trackPreset.TrackName, trackPreset.RegimeSettings.RegimeCaption);
			}
			else
			{
				this.SelectedTrackHolder.SetActive(false);
			}
		}
		if (!customProperties.ContainsKey("pt"))
		{
			this.PingIndicatorImage.SetActive(false);
			return;
		}
		this.PingIndicatorImage.SetActive(true);
		int num = (int)customProperties["pt"];
		this.PingText.text = num.ToString();
		if (num <= B.MultiplayerSettings.VeryGoodPing)
		{
			this.PingIndicatorImage.sprite = B.MultiplayerSettings.VeryGoodPingSprite;
			return;
		}
		if (num <= B.MultiplayerSettings.GoodPing)
		{
			this.PingIndicatorImage.sprite = B.MultiplayerSettings.GoodPingSprite;
			return;
		}
		if (num <= B.MultiplayerSettings.MediumPing)
		{
			this.PingIndicatorImage.sprite = B.MultiplayerSettings.MediumPingSprite;
			return;
		}
		this.PingIndicatorImage.sprite = B.MultiplayerSettings.BadPingSprite;
	}

	[SerializeField]
	private TextMeshProUGUI PlayerNickNameText;

	[SerializeField]
	private TextMeshProUGUI PlayerCarText;

	[SerializeField]
	private Button KickButton;

	[SerializeField]
	private GameObject ReadyObject;

	[SerializeField]
	private GameObject NotReadyObject;

	[SerializeField]
	private Image PingIndicatorImage;

	[SerializeField]
	private TextMeshProUGUI PingText;

	[SerializeField]
	private GameObject SelectedTrackHolder;

	[SerializeField]
	private Image TrackIconImage;

	[SerializeField]
	private Image RegimeIconImage;

	[SerializeField]
	private TextMeshProUGUI TrackNameText;

	private float Timer;

	private Player TargetPlayer;

	private string trackName;
}
