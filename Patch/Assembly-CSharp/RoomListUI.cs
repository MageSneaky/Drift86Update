using System;
using System.Collections.Generic;
using System.Linq;
using ExitGames.Client.Photon;
using GameBalance;
using HeathenEngineering.SteamAPI;
using Photon.Pun;
using Photon.Realtime;
using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class RoomListUI : MonoBehaviour
{
	private void Start()
	{
		this.ConnectToRandomRoomButton.onClick.AddListener(new UnityAction(this.ConnectToRandomRoom));
		this.CreateRoomButton.onClick.AddListener(new UnityAction(this.CreateRoom));
		this.RoomItemUIRef.SetActive(false);
		this.CreateRoomButton.interactable = false;
	}

	private void OnEnable()
	{
		this.Timer = 0f;
		this.ServerList.Select();
		this.UpdateServerList("");
	}

	private void Update()
	{
		bool flag = PhotonNetwork.IsConnectedAndReady && PhotonNetwork.InLobby;
		this.CreateRoomButton.interactable = flag;
		this.ConnectToRandomRoomButton.interactable = flag;
		if (this.Timer <= 0f && flag)
		{
			this.UpdatePing();
			this.Timer = B.MultiplayerSettings.PingUpdateSettings;
		}
		this.Timer -= Time.deltaTime;
	}

	private void ConnectToRandomRoom()
	{
		PhotonNetwork.JoinRandomRoom(new Hashtable
		{
			{
				"rr",
				true
			}
		}, 0, MatchmakingMode.RandomMatching, null, null, null);
	}

	public void CreateRandomRoom()
	{
		Hashtable hashtable = new Hashtable();
		hashtable.Add("rr", true);
		string[] customRoomPropertiesForLobby = new string[]
		{
			"rr"
		};
		RoomOptions roomOptions = new RoomOptions
		{
			IsVisible = true,
			IsOpen = true,
			MaxPlayers = B.MultiplayerSettings.MaxPlayersInRoom,
			CustomRoomProperties = hashtable,
			CustomRoomPropertiesForLobby = customRoomPropertiesForLobby
		};
		PhotonNetwork.CreateRoom(Guid.NewGuid().ToString(), roomOptions, null, null);
	}

	private void CreateRoom()
	{
		Hashtable hashtable = new Hashtable();
		hashtable.Add("rc", PhotonNetwork.NickName);
		hashtable.Add("tn", B.MultiplayerSettings.AvailableTracksForMultiplayer.First<TrackPreset>().name);
		string[] customRoomPropertiesForLobby = new string[]
		{
			"rc",
			"tn"
		};
		RoomOptions roomOptions = new RoomOptions
		{
			IsVisible = true,
			IsOpen = true,
			MaxPlayers = B.MultiplayerSettings.MaxPlayersInRoom,
			CustomRoomProperties = hashtable,
			CustomRoomPropertiesForLobby = customRoomPropertiesForLobby
		};
		PhotonNetwork.CreateRoom(Guid.NewGuid().ToString(), roomOptions, null, null);
		SteamUserStats.SetAchievement("CreateRoom");
		SteamSettings.Client.StoreStatsAndAchievements();
	}

	public void OnRoomListUpdate(List<RoomInfo> roomList)
	{
		foreach (RoomItemUI roomItemUI in this.RoomItems)
		{
			if (roomItemUI != null && roomItemUI.Room == null)
			{
				UnityEngine.Object.Destroy(roomItemUI.gameObject);
			}
		}
		this.RoomItems.RemoveAll((RoomItemUI i) => i == null);
		using (List<RoomInfo>.Enumerator enumerator2 = roomList.GetEnumerator())
		{
			while (enumerator2.MoveNext())
			{
				RoomInfo room = enumerator2.Current;
				if (room.CustomProperties == null || !room.CustomProperties.ContainsKey("rr"))
				{
					RoomItemUI roomItemUI2 = this.RoomItems.FirstOrDefault((RoomItemUI r) => r.Room != null && r.Room.Name == room.Name);
					if (roomItemUI2 == null)
					{
						roomItemUI2 = UnityEngine.Object.Instantiate<RoomItemUI>(this.RoomItemUIRef, this.RoomItemUIRef.transform.parent);
						this.RoomItems.Add(roomItemUI2);
					}
					if (room.CustomProperties == null || room.PlayerCount == 0 || !room.IsOpen || !room.CustomProperties.ContainsKey("rc") || !room.CustomProperties.ContainsKey("tn"))
					{
						roomItemUI2.SetActive(false);
					}
					else
					{
						string trackName = (string)room.CustomProperties["tn"];
						TrackPreset trackPreset = B.MultiplayerSettings.AvailableTracksForMultiplayer.Find((TrackPreset t) => t.name == trackName);
						roomItemUI2.UpdateInfo(room, trackPreset.TrackIcon, trackPreset.RegimeSettings.RegimeImage, (string)room.CustomProperties["rc"], trackPreset.TrackName, string.Format("{0}/{1}", room.PlayerCount, room.MaxPlayers), delegate
						{
							PhotonNetwork.JoinRoom(room.Name, null);
						});
						roomItemUI2.SetActive(true);
					}
				}
			}
		}
	}

	private void UpdateServerList(string autoRegion = "")
	{
	}

	private void UpdatePing()
	{
		int ping = PhotonNetwork.GetPing();
		this.PingText.text = ping.ToString();
		if (ping <= B.MultiplayerSettings.VeryGoodPing)
		{
			this.PingIndicatorImage.sprite = B.MultiplayerSettings.VeryGoodPingSprite;
			return;
		}
		if (ping <= B.MultiplayerSettings.GoodPing)
		{
			this.PingIndicatorImage.sprite = B.MultiplayerSettings.GoodPingSprite;
			return;
		}
		if (ping <= B.MultiplayerSettings.MediumPing)
		{
			this.PingIndicatorImage.sprite = B.MultiplayerSettings.MediumPingSprite;
			return;
		}
		this.PingIndicatorImage.sprite = B.MultiplayerSettings.BadPingSprite;
	}

	[SerializeField]
	private RoomItemUI RoomItemUIRef;

	[SerializeField]
	private Button ConnectToRandomRoomButton;

	[SerializeField]
	private Button CreateRoomButton;

	[SerializeField]
	private TMP_Dropdown ServerList;

	[SerializeField]
	private TMP_Dropdown ServerListLautre;

	[SerializeField]
	private GameObject ThisPage;

	[SerializeField]
	private TextMeshProUGUI LabelServer;

	[Space(10f)]
	[SerializeField]
	private TextMeshProUGUI PingText;

	[SerializeField]
	private Image PingIndicatorImage;

	[SerializeField]
	private string AutoText = "(Auto) ";

	private List<RoomItemUI> RoomItems = new List<RoomItemUI>();

	private List<string> Tokens = new List<string>();

	private float Timer;
}
