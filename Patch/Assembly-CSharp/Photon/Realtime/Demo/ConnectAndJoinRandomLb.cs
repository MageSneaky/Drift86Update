using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using UnityEngine;
using UnityEngine.UI;

namespace Photon.Realtime.Demo
{
	public class ConnectAndJoinRandomLb : MonoBehaviour, IConnectionCallbacks, IMatchmakingCallbacks, ILobbyCallbacks
	{
		public void Start()
		{
			this.lbc = new LoadBalancingClient(ConnectionProtocol.Udp);
			this.lbc.AppId = this.AppId;
			this.lbc.AddCallbackTarget(this);
			this.lbc.ConnectToNameServer();
			this.ch = base.gameObject.GetComponent<ConnectionHandler>();
			if (this.ch != null)
			{
				this.ch.Client = this.lbc;
				this.ch.StartFallbackSendAckThread();
			}
		}

		public void Update()
		{
			LoadBalancingClient loadBalancingClient = this.lbc;
			if (loadBalancingClient != null)
			{
				loadBalancingClient.Service();
				Text stateUiText = this.StateUiText;
				string text = loadBalancingClient.State.ToString();
				if (stateUiText != null && !stateUiText.text.Equals(text))
				{
					stateUiText.text = "State: " + text;
				}
			}
		}

		public void OnConnected()
		{
		}

		public void OnConnectedToMaster()
		{
			Debug.Log("OnConnectedToMaster");
			this.lbc.OpJoinRandomRoom(null);
		}

		public void OnDisconnected(DisconnectCause cause)
		{
			Debug.Log("OnDisconnected(" + cause + ")");
		}

		public void OnCustomAuthenticationResponse(Dictionary<string, object> data)
		{
		}

		public void OnCustomAuthenticationFailed(string debugMessage)
		{
		}

		public void OnRegionListReceived(RegionHandler regionHandler)
		{
			Debug.Log("OnRegionListReceived");
			regionHandler.PingMinimumOfRegions(new Action<RegionHandler>(this.OnRegionPingCompleted), null);
		}

		public void OnRoomListUpdate(List<RoomInfo> roomList)
		{
		}

		public void OnLobbyStatisticsUpdate(List<TypedLobbyInfo> lobbyStatistics)
		{
		}

		public void OnJoinedLobby()
		{
		}

		public void OnLeftLobby()
		{
		}

		public void OnFriendListUpdate(List<FriendInfo> friendList)
		{
		}

		public void OnCreatedRoom()
		{
		}

		public void OnCreateRoomFailed(short returnCode, string message)
		{
		}

		public void OnJoinedRoom()
		{
			Debug.Log("OnJoinedRoom");
		}

		public void OnJoinRoomFailed(short returnCode, string message)
		{
		}

		public void OnJoinRandomFailed(short returnCode, string message)
		{
			Debug.Log("OnJoinRandomFailed");
			this.lbc.OpCreateRoom(new EnterRoomParams());
		}

		public void OnLeftRoom()
		{
		}

		private void OnRegionPingCompleted(RegionHandler regionHandler)
		{
			Debug.Log("OnRegionPingCompleted " + regionHandler.BestRegion);
			Debug.Log("RegionPingSummary: " + regionHandler.SummaryToCache);
			this.lbc.ConnectToRegionMaster(regionHandler.BestRegion.Code);
		}

		public string AppId;

		private LoadBalancingClient lbc;

		private ConnectionHandler ch;

		public Text StateUiText;
	}
}
