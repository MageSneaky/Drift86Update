using System;
using System.Collections;
using System.Collections.Generic;
using CodeStage.AntiCheat.Storage;
using GameBalance;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PersitanteRoomListUI : MonoBehaviourPunCallbacks
{
	private void Start()
	{
	}

	public override void OnRoomListUpdate(List<RoomInfo> roomList)
	{
		base.OnRoomListUpdate(roomList);
		foreach (RoomInfo roomInfo in roomList)
		{
			if (roomInfo.Name == this.RoomName)
			{
				this.playercountx = roomInfo.PlayerCount;
				this.RoomPlayerCount.text = string.Concat(new object[]
				{
					this.NameUI,
					" [",
					this.playercountx,
					" / ",
					12,
					"]"
				});
				if (this.playercountx >= 12)
				{
					base.GetComponent<Button>().interactable = false;
				}
				else
				{
					base.GetComponent<Button>().interactable = true;
				}
			}
			Debug.Log(string.Concat(new object[]
			{
				"MDR T KI : ",
				roomInfo.Name,
				" count : ",
				this.playercountx
			}));
		}
	}

	public void JoinLobbyNum()
	{
		if (PhotonNetwork.InLobby && this.playercountx < this.Maxplayerr)
		{
			ObscuredPrefs.SetString("MYROOM", this.RoomName);
			UnityEngine.Object.FindObjectOfType<SI_PersistantLobby>().DisableAllBtn();
			base.StartCoroutine(this.GoMap());
			return;
		}
		base.GetComponent<Button>().interactable = false;
		Debug.Log("JOIN IMPOSSIBLE");
	}

	private IEnumerator GoMap()
	{
		string.Concat(WorldLoading.PlayerCar);
		if (!WorldLoading.PlayerCar)
		{
			WorldLoading.PlayerCar = this.SelectedCar;
		}
		yield return new WaitForSeconds(1f);
		WorldLoading.LoadingTrack = this.CurrentTrackPreset;
		yield return new WaitForSeconds(1f);
		Debug.Log("STARTING...");
		WorldLoading.IsMultiplayer = true;
		LoadingScreenUI.LoadScene(WorldLoading.LoadingTrack.SceneName + " - Persistant", LoadSceneMode.Single);
		yield break;
	}

	public Text RoomPlayerCount;

	public string RoomName;

	public string NameUI;

	public string Mapname;

	public int playercountx;

	public int Maxplayerr = 12;

	public TrackPreset CurrentTrackPreset;

	public CarPreset SelectedCar;

	public SelectCarMenuUI carselector;
}
