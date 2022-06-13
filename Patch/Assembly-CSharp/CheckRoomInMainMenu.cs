using System;
using UnityEngine;

public class CheckRoomInMainMenu : MonoBehaviour
{
	private void Start()
	{
		this.LobbyManager.CheckCurrentConnection();
	}

	[SerializeField]
	private LobbyManager LobbyManager;
}
