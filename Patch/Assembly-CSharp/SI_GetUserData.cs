using System;
using System.Collections;
using CodeStage.AntiCheat.Storage;
using HeathenEngineering.SteamAPI;
using Steamworks;
using UnityEngine;

public class SI_GetUserData : MonoBehaviour
{
	private void Start()
	{
		this.playername = this.udata.DisplayName;
		this.SteamuserID = this.udata.id;
		this.MyAvatar = this.udata.avatar;
		PlayerPrefs.SetString("MyName", this.playername);
		base.StartCoroutine(this.reloadinfo());
		int num = 0;
		if (this.CarsPack.IsDlcInstalled || SneakyManager.dlcToggle)
		{
			num++;
			ObscuredPrefs.SetBool("DLC - CarsPack", true);
		}
		if (this.CarsPack.IsSubscribed || SneakyManager.dlcToggle)
		{
			num++;
			ObscuredPrefs.SetBool("DLC - CarsPack", true);
		}
		if (num == 0)
		{
			ObscuredPrefs.SetBool("DLC - CarsPack", false);
		}
	}

	private IEnumerator reloadinfo()
	{
		yield return new WaitForSeconds(2f);
		this.playername = this.udata.DisplayName;
		this.SteamuserID = this.udata.id;
		this.MyAvatar = this.udata.avatar;
		int num = 0;
		if (this.CarsPack.IsDlcInstalled || SneakyManager.dlcToggle)
		{
			num++;
			ObscuredPrefs.SetBool("DLC - CarsPack", true);
		}
		if (this.CarsPack.IsSubscribed || SneakyManager.dlcToggle)
		{
			num++;
			ObscuredPrefs.SetBool("DLC - CarsPack", true);
		}
		if (num == 0)
		{
			ObscuredPrefs.SetBool("DLC - CarsPack", false);
		}
		yield break;
	}

	private void Update()
	{
	}

	[SerializeField]
	private UserData udata;

	public string playername;

	public CSteamID SteamuserID;

	public Texture MyAvatar;

	[Space]
	public DownloadableContentObject CarsPack;
}
