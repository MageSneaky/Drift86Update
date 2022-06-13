using System;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class LobbyManagerPersistant : WindowWithShowHideAnimators, ILobbyCallbacks, IMatchmakingCallbacks, IConnectionCallbacks
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
		PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = PlayerProfile.ServerToken;
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
			LobbyManagerPersistant.ConnectToServer();
		}
		else if (!PhotonNetwork.InLobby)
		{
			PhotonNetwork.JoinLobby();
		}
		PhotonNetwork.AddCallbackTarget(this);
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

	public void OnJoinedRoom()
	{
		Debug.Log("On joined room");
	}

	public void OnLeftRoom()
	{
		Debug.Log("On left room");
	}

	public new void OnPlayerEnteredRoom(Player newPlayer)
	{
		base.OnPlayerEnteredRoom(newPlayer);
		Debug.Log("join  de : " + newPlayer.NickName);
	}

	public void OnPlayerLeftRoom(Player newPlayer)
	{
		base.OnPlayerEnteredRoom(newPlayer);
		Debug.Log("leave  de : " + newPlayer.NickName);
	}

	public void OnRoomListUpdate(List<RoomInfo> roomList)
	{
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
		if (returnCode != 32760)
		{
			Debug.LogErrorFormat("Join random room failed, error message: {0}", new object[]
			{
				message
			});
			MessageBox.Show(message);
		}
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
	private string LeaveRoomMessage = "leave the room?";
}
