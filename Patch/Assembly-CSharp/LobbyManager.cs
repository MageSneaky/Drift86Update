using System;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class LobbyManager : WindowWithShowHideAnimators, ILobbyCallbacks, IMatchmakingCallbacks, IConnectionCallbacks
{
	public bool InRoom
	{
		get
		{
			return PhotonNetwork.IsConnectedAndReady && PhotonNetwork.InRoom;
		}
	}

	public static void ConnectToServer()
	{
		if (PhotonNetwork.IsConnectedAndReady)
		{
			PhotonNetwork.Disconnect();
		}
		PlayerProfile.ServerToken = PlayerPrefs.GetString("Settings_MyRegion");
		PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = PlayerPrefs.GetString("Settings_MyRegion");
		PhotonNetwork.ConnectUsingSettings();
	}

	private void Start()
	{
		Action leaveAction = delegate()
		{
			PhotonNetwork.LeaveRoom(true);
		};
		this.CustomBackAction = delegate()
		{
			if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom)
			{
				if (!MessageBox.HasActiveMessageBox)
				{
					MessageBox.Show(this.LeaveRoomMessage, leaveAction, null, "Yes", "Cancel");
					return;
				}
			}
			else
			{
				Singleton<WindowsController>.Instance.OnBack(true);
			}
		};
	}

	private void OnEnable()
	{
		if (PhotonNetwork.NickName != PlayerProfile.NickName)
		{
			PhotonNetwork.NickName = PlayerProfile.NickName;
		}
		if (!PhotonNetwork.IsConnectedAndReady)
		{
			LobbyManager.ConnectToServer();
		}
		else if (!PhotonNetwork.InLobby)
		{
			PhotonNetwork.JoinLobby();
		}
		PhotonNetwork.AddCallbackTarget(this);
		this.UpdateHolders();
	}

	private void OnDisable()
	{
		PhotonNetwork.RemoveCallbackTarget(this);
	}

	public void CheckCurrentConnection()
	{
		if (this.InRoom)
		{
			Singleton<WindowsController>.Instance.OpenWindow(this);
			if (PhotonNetwork.LocalPlayer.IsMasterClient)
			{
				PhotonNetwork.CurrentRoom.IsOpen = true;
				PhotonNetwork.CurrentRoom.IsVisible = true;
			}
		}
	}

	public void UpdateHolders()
	{
		bool inRoom = PhotonNetwork.InRoom;
		bool flag = inRoom && PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("rr");
		this.SelectorMultiMode.SetActive(!inRoom);
		this.RoomListHolder.SetActive(!inRoom && flag);
		this.InRoomHolder.SetActive(inRoom && !flag);
		this.InRandomRoomHolder.SetActive(flag);
		if (PhotonNetwork.InRoom)
		{
			if (PhotonNetwork.CurrentRoom.Name.Contains("Persistant"))
			{
				this.InRoomHolder.SetActive(false);
				return;
			}
			GameObject.FindGameObjectWithTag("backui").GetComponent<Animator>().Play("MultiMenuAnim_ClassicMenuBack");
		}
	}

	public void OnJoinedRoom()
	{
		Debug.Log("On joined room");
		this.UpdateHolders();
	}

	public void OnLeftRoom()
	{
		Debug.Log("On left room");
		this.UpdateHolders();
		this.InRoomHolder.RemoveAllPlayers();
	}

	public void OnRoomListUpdate(List<RoomInfo> roomList)
	{
		if (this.RoomListHolder.gameObject.activeInHierarchy)
		{
			this.RoomListHolder.OnRoomListUpdate(roomList);
		}
	}

	public void OnCreateRoomFailed(short returnCode, string message)
	{
		Debug.LogErrorFormat("Create room failed, error message: {0}", new object[]
		{
			message
		});
		MessageBox.Show(message);
	}

	public void OnJoinRoomFailed(short returnCode, string message)
	{
		Debug.LogErrorFormat("Join room failed, error message: {0}", new object[]
		{
			message
		});
		MessageBox.Show(message);
	}

	public void OnJoinRandomFailed(short returnCode, string message)
	{
		if (returnCode == 32760)
		{
			this.RoomListHolder.CreateRandomRoom();
			return;
		}
		Debug.LogErrorFormat("Join random room failed, error message: {0}", new object[]
		{
			message
		});
		MessageBox.Show(message);
	}

	public void OnCreatedRoom()
	{
		Debug.Log("Room is created");
	}

	public void OnConnectedToMaster()
	{
		Debug.Log("Connected to master");
		PhotonNetwork.JoinLobby();
	}

	public void OnJoinedLobby()
	{
		Debug.Log("On joined lobby");
	}

	public void OnLeftLobby()
	{
		Debug.Log("On left lobby");
	}

	public void OnConnected()
	{
		Debug.Log("Connected to Photon");
	}

	public void OnDisconnected(DisconnectCause cause)
	{
		B.MultiplayerSettings.ShowDisconnectCause(cause, null);
	}

	public void OnFriendListUpdate(List<FriendInfo> friendList)
	{
	}

	public void OnLobbyStatisticsUpdate(List<TypedLobbyInfo> lobbyStatistics)
	{
	}

	public void OnRegionListReceived(RegionHandler regionHandler)
	{
	}

	public void OnCustomAuthenticationResponse(Dictionary<string, object> data)
	{
	}

	public void OnCustomAuthenticationFailed(string debugMessage)
	{
	}

	[SerializeField]
	private RoomListUI RoomListHolder;

	[SerializeField]
	private InRoomUI InRoomHolder;

	[SerializeField]
	private InRoomUI InRandomRoomHolder;

	[SerializeField]
	private GameObject PersistentRooomList;

	[SerializeField]
	private GameObject SelectorMultiMode;

	[SerializeField]
	private GameObject SelectorChannel;

	[SerializeField]
	private GameObject Roomlistui2;

	[SerializeField]
	private string LeaveRoomMessage = "leave the room?";
}
