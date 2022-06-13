using System;
using System.Collections.Generic;
using System.Linq;
using ExitGames.Client.Photon;
using GameBalance;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class InRoomUI : MonoBehaviour, IInRoomCallbacks, IOnEventCallback
{
	private Room CurrentRoom
	{
		get
		{
			return PhotonNetwork.CurrentRoom;
		}
	}

	private bool IsMaster
	{
		get
		{
			return PhotonNetwork.IsMasterClient;
		}
	}

	private bool IsRandomRoom
	{
		get
		{
			return this.CurrentRoom.CustomProperties.ContainsKey("rr");
		}
	}

	private Player LocalPlayer
	{
		get
		{
			return PhotonNetwork.LocalPlayer;
		}
	}

	private void Awake()
	{
		this.PlayerItemUIRef.SetActive(false);
		this.SelectTrackButton.onClick.AddListener(delegate()
		{
			Singleton<WindowsController>.Instance.OpenWindow(this.SelectTrackUI);
			this.SelectTrackUI.OnSelectTrackAction = new Action<TrackPreset>(this.OnSelectTrack);
		});
		this.SelectCarButton.onClick.AddListener(delegate()
		{
			Singleton<WindowsController>.Instance.OpenWindow(this.SelectCarMenuUI);
			this.SelectCarMenuUI.OnSelectCarAction = new Action<CarPreset>(this.OnSelectCar);
		});
		this.SelectCarButton2.onClick.AddListener(delegate()
		{
			Singleton<WindowsController>.Instance.OpenWindow(this.SelectCarMenuUI);
			this.SelectCarMenuUI.OnSelectCarAction = new Action<CarPreset>(this.OnSelectCar);
		});
		this.ReadyButton.onClick.AddListener(new UnityAction(this.OnReadyClick));
	}

	private void OnEnable()
	{
		PhotonNetwork.AddCallbackTarget(this);
		if (this.CurrentRoom == null)
		{
			return;
		}
		this.SelectTrackButton.SetActive(this.IsMaster || this.IsRandomRoom);
		this.OnRoomPropertiesUpdate(this.CurrentRoom.CustomProperties);
		foreach (KeyValuePair<Player, PlayerItemInRoomUI> keyValuePair in this.Players)
		{
			Object.Destroy(keyValuePair.Value.gameObject);
		}
		this.Players.Clear();
		foreach (KeyValuePair<int, Player> keyValuePair2 in this.CurrentRoom.Players)
		{
			this.TryUpdateOrCreatePlayerItem(keyValuePair2.Value);
		}
		Player localPlayer = PhotonNetwork.LocalPlayer;
		if (WorldLoading.PlayerCar == null || !this.LocalPlayer.CustomProperties.ContainsKey("cn"))
		{
			CarPreset carPreset = WorldLoading.AvailableCars.First<CarPreset>();
			WorldLoading.PlayerCar = carPreset;
			this.LocalPlayer.SetCustomProperties("cn", carPreset.CarCaption);
			this.LocalPlayer.SetCustomProperties(new object[]
			{
				"cn",
				carPreset.CarCaption,
				"cci",
				PlayerProfile.GetCarColorIndex(carPreset),
				"ir",
				false
			});
		}
		if (PhotonNetwork.IsMasterClient && (this.CurrentRoom.CustomProperties == null || !this.CurrentRoom.CustomProperties.ContainsKey("tn")))
		{
			Hashtable hashtable = new Hashtable();
			hashtable.Add("tn", B.GameSettings.Tracks.First<TrackPreset>().name);
			this.CurrentRoom.SetCustomProperties(hashtable, null, null);
		}
		this.ReadyButton.Select();
	}

	public virtual void OnDisable()
	{
		PhotonNetwork.RemoveCallbackTarget(this);
	}

	private void OnSelectCar(CarPreset selectedCar)
	{
		Singleton<WindowsController>.Instance.OnBack(false);
		WorldLoading.PlayerCar = selectedCar;
		this.LocalPlayer.SetCustomProperties(new object[]
		{
			"cn",
			selectedCar.CarCaption,
			"cci",
			PlayerProfile.GetCarColorIndex(selectedCar)
		});
	}

	private void OnSelectTrack(TrackPreset selectedTrack)
	{
		Singleton<WindowsController>.Instance.OnBack(false);
		Hashtable hashtable = new Hashtable();
		hashtable.Add("tn", selectedTrack.name);
		if (this.IsRandomRoom)
		{
			this.LocalPlayer.SetCustomProperties(hashtable, null, null);
			return;
		}
		if (this.IsMaster)
		{
			this.CurrentRoom.SetCustomProperties(hashtable, null, null);
		}
	}

	private void OnReadyClick()
	{
		this.LocalPlayer.SetCustomProperties(new object[]
		{
			"ir",
			!(bool)this.LocalPlayer.CustomProperties["ir"],
			"cci",
			PlayerProfile.GetCarColorIndex(WorldLoading.PlayerCar)
		});
	}

	private void TryUpdateOrCreatePlayerItem(Player targetPlayer)
	{
		PlayerItemInRoomUI playerItemInRoomUI = null;
		if (!this.Players.TryGetValue(targetPlayer, out playerItemInRoomUI))
		{
			playerItemInRoomUI = Object.Instantiate<PlayerItemInRoomUI>(this.PlayerItemUIRef, this.PlayerItemUIRef.transform.parent);
			this.Players.Add(targetPlayer, playerItemInRoomUI);
		}
		Hashtable customProperties = targetPlayer.CustomProperties;
		if (customProperties == null || !customProperties.ContainsKey("cn") || !customProperties.ContainsKey("ir"))
		{
			playerItemInRoomUI.SetActive(false);
			return;
		}
		bool flag = (bool)customProperties["ir"];
		Action kickAction = null;
		if (PhotonNetwork.IsMasterClient && !this.IsRandomRoom)
		{
			kickAction = delegate()
			{
				PhotonNetwork.CloseConnection(targetPlayer);
			};
		}
		playerItemInRoomUI.SetActive(true);
		playerItemInRoomUI.UpdateProperties(targetPlayer, kickAction);
		if (targetPlayer.IsLocal)
		{
			this.ReadyButton.colors = (flag ? this.ReadyColors : this.NotReadyColors);
		}
		if (this.IsMaster && !this.WaitStartGame && (int)this.CurrentRoom.PlayerCount >= this.MinimumPlayersForStart)
		{
			if (this.CurrentRoom.Players.All((KeyValuePair<int, Player> p) => p.Value.CustomProperties.ContainsKey("ir") && (bool)p.Value.CustomProperties["ir"]))
			{
				if (this.CurrentRoom.CustomProperties.ContainsKey("rr"))
				{
					Dictionary<string, int> dictionary = new Dictionary<string, int>();
					foreach (TrackPreset trackPreset in B.MultiplayerSettings.AvailableTracksForMultiplayer)
					{
						dictionary.Add(trackPreset.name, 0);
					}
					foreach (KeyValuePair<int, Player> keyValuePair in this.CurrentRoom.Players)
					{
						string text = keyValuePair.Value.CustomProperties.ContainsKey("tn") ? ((string)keyValuePair.Value.CustomProperties["tn"]) : "";
						if (!string.IsNullOrEmpty(text))
						{
							Dictionary<string, int> dictionary2 = dictionary;
							string key = text;
							int num = dictionary2[key];
							dictionary2[key] = num + 1;
						}
					}
					int num2 = dictionary.Max((KeyValuePair<string, int> kv) => kv.Value);
					List<string> list = new List<string>();
					foreach (KeyValuePair<string, int> keyValuePair2 in dictionary)
					{
						if (keyValuePair2.Value >= num2)
						{
							list.Add(keyValuePair2.Key);
						}
					}
					Hashtable hashtable = new Hashtable();
					hashtable.Add("tn", list.RandomChoice<string>());
					this.CurrentRoom.SetCustomProperties(hashtable, null, null);
					Debug.Log("THIS FONCTION 6");
				}
				PhotonNetwork.RaiseEvent(0, null, new RaiseEventOptions
				{
					Receivers = 1
				}, SendOptions.SendReliable);
				this.WaitStartGame = true;
			}
		}
	}

	public void RemoveAllPlayers()
	{
		foreach (KeyValuePair<Player, PlayerItemInRoomUI> keyValuePair in this.Players)
		{
			Object.Destroy(keyValuePair.Value.gameObject);
		}
		this.Players.Clear();
	}

	public void OnMasterClientSwitched(Player newMasterClient)
	{
		Debug.LogFormat("New master is player [{0}]", new object[]
		{
			newMasterClient.NickName
		});
		this.SelectTrackButton.SetActive(this.IsMaster);
		foreach (KeyValuePair<int, Player> keyValuePair in this.CurrentRoom.Players)
		{
			this.TryUpdateOrCreatePlayerItem(keyValuePair.Value);
		}
		this.UpdateCustomProperties();
	}

	public void OnPlayerEnteredRoom(Player newPlayer)
	{
		this.TryUpdateOrCreatePlayerItem(newPlayer);
		this.UpdateCustomProperties();
	}

	public void OnPlayerLeftRoom(Player otherPlayer)
	{
		if (this.Players.ContainsKey(otherPlayer))
		{
			Object.Destroy(this.Players[otherPlayer].gameObject);
			this.Players.Remove(otherPlayer);
		}
		this.UpdateCustomProperties();
	}

	public void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
	{
		this.TryUpdateOrCreatePlayerItem(targetPlayer);
	}

	public void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
	{
		if (propertiesThatChanged.ContainsKey("tn"))
		{
			string trackName = (string)propertiesThatChanged["tn"];
			TrackPreset trackPreset = B.GameSettings.Tracks.FirstOrDefault((TrackPreset t) => t.name == trackName);
			this.SelectedTrackIcon.sprite = trackPreset.TrackIcon;
			this.RegimeIcon.sprite = trackPreset.RegimeSettings.RegimeImage;
			this.SelectedTrackText.text = string.Format("{0}: {1}", trackPreset.TrackName, trackPreset.RegimeSettings.RegimeCaption);
		}
	}

	private void UpdateCustomProperties()
	{
		if (this.IsMaster)
		{
			Hashtable hashtable = new Hashtable();
			hashtable.Add("rc", PlayerProfile.NickName);
			this.CurrentRoom.SetCustomProperties(hashtable, null, null);
		}
	}

	public void OnEvent(EventData photonEvent)
	{
		if (photonEvent.Code == 0)
		{
			WorldLoading.IsMultiplayer = true;
			PhotonNetwork.LocalPlayer.SetCustomProperties(new object[]
			{
				"ir",
				false,
				"il",
				false,
				"tn",
				""
			});
			if (!this.CurrentRoom.CustomProperties.ContainsKey("tn"))
			{
				return;
			}
			string trackName = (string)this.CurrentRoom.CustomProperties["tn"];
			TrackPreset trackPreset = B.GameSettings.Tracks.FirstOrDefault((TrackPreset t) => t.name == trackName);
			WorldLoading.LoadingTrack = trackPreset;
			LoadingScreenUI.LoadScene(trackPreset.SceneName, trackPreset.RegimeSettings.RegimeSceneName);
			if (this.IsMaster)
			{
				this.CurrentRoom.IsOpen = false;
				this.CurrentRoom.IsVisible = false;
			}
		}
	}

	[SerializeField]
	private PlayerItemInRoomUI PlayerItemUIRef;

	[SerializeField]
	private Button SelectCarButton;

	[SerializeField]
	private Button SelectCarButton2;

	[SerializeField]
	private Button SelectTrackButton;

	[SerializeField]
	private Image SelectedTrackIcon;

	[SerializeField]
	private Image RegimeIcon;

	[SerializeField]
	private TextMeshProUGUI SelectedTrackText;

	[SerializeField]
	private SelectTrackUI SelectTrackUI;

	[SerializeField]
	private SelectCarMenuUI SelectCarMenuUI;

	[SerializeField]
	private int MinimumPlayersForStart = 2;

	[SerializeField]
	private Button ReadyButton;

	[SerializeField]
	private ColorBlock ReadyColors;

	[SerializeField]
	private ColorBlock NotReadyColors;

	private Dictionary<Player, PlayerItemInRoomUI> Players = new Dictionary<Player, PlayerItemInRoomUI>();

	private bool WaitStartGame;
}
