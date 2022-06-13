using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GameBalance;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SI_PersistantLobby : MonoBehaviour
{
	private Player LocalPlayer
	{
		get
		{
			return PhotonNetwork.LocalPlayer;
		}
	}

	private void Start()
	{
	}

	private void Awake()
	{
		this.SelectCarButton.onClick.AddListener(delegate()
		{
			Singleton<WindowsController>.Instance.OpenWindow(this.SelectCarMenuUI);
			this.SelectCarMenuUI.OnSelectCarAction = new Action<CarPreset>(this.OnSelectCar);
		});
	}

	private void OnEnable()
	{
		this.Timer = 0f;
		Player localPlayer = PhotonNetwork.LocalPlayer;
		if (PlayerPrefs.GetString("REGIONN") == "asia" && this.ServerList.value != 2)
		{
			this.ServerList.value = 2;
			return;
		}
		if (PlayerPrefs.GetString("REGIONN") == "eu" && this.ServerList.value != 1)
		{
			this.ServerList.value = 1;
			return;
		}
		if (PlayerPrefs.GetString("REGIONN") == "us" && this.ServerList.value != 0)
		{
			this.ServerList.value = 0;
			return;
		}
		if (PlayerPrefs.GetString("REGIONN") == "sa" && this.ServerList.value != 3)
		{
			this.ServerList.value = 3;
		}
	}

	public void DisableAllBtn()
	{
		Button[] array = this.allbtn;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].interactable = false;
		}
		base.StartCoroutine(this.enablebtn());
	}

	private IEnumerator enablebtn()
	{
		yield return new WaitForSeconds(5f);
		Button[] array = this.allbtn;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].interactable = true;
		}
		yield break;
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

	private void Update()
	{
		bool flag = PhotonNetwork.IsConnectedAndReady && PhotonNetwork.InLobby;
		if (this.Timer <= 0f && flag)
		{
			this.UpdatePing();
			this.Timer = B.MultiplayerSettings.PingUpdateSettings;
		}
		this.Timer -= Time.deltaTime;
		this.checkcars();
	}

	public void checkcars()
	{
		int num = 0;
		foreach (string value in this.CarsName)
		{
			if (!WorldLoading.PlayerCar)
			{
				this.carsDisplay.sprite = this.AllCarsSrpite[0];
				this.carsDisplay2.sprite = this.AllCarsSrpite[0];
			}
			else if (WorldLoading.PlayerCar.ToString().Contains("SunLineGTER35"))
			{
				this.carsDisplay.sprite = this.AllCarsSrpite[16];
				this.carsDisplay2.sprite = this.AllCarsSrpite[16];
			}
			else if (WorldLoading.PlayerCar.ToString().Contains("SunLineGTE"))
			{
				this.carsDisplay.sprite = this.AllCarsSrpite[3];
				this.carsDisplay2.sprite = this.AllCarsSrpite[3];
			}
			else if (WorldLoading.PlayerCar.ToString().Contains("GTR31moyo"))
			{
				this.carsDisplay.sprite = this.AllCarsSrpite[26];
				this.carsDisplay2.sprite = this.AllCarsSrpite[26];
			}
			else if (WorldLoading.PlayerCar.ToString().Contains("GTR31"))
			{
				this.carsDisplay.sprite = this.AllCarsSrpite[25];
				this.carsDisplay2.sprite = this.AllCarsSrpite[25];
			}
			else if (WorldLoading.PlayerCar.ToString().Contains("SupraNew"))
			{
				this.carsDisplay.sprite = this.AllCarsSrpite[18];
				this.carsDisplay2.sprite = this.AllCarsSrpite[18];
			}
			else if (WorldLoading.PlayerCar.ToString().Contains(value))
			{
				this.carsDisplay.sprite = this.AllCarsSrpite[num];
				this.carsDisplay2.sprite = this.AllCarsSrpite[num];
				return;
			}
			num++;
		}
	}

	public void senddataupdate()
	{
		B.MultiplayerSettings.Servers.FirstOrDefault((ServerName s) => s.ServerToken == PhotonNetwork.NetworkingClient.CloudRegion);
	}

	private void UpdateServerList(string autoRegion = "")
	{
		this.Tokens = new List<string>();
		this.ServerList.ClearOptions();
		List<TMP_Dropdown.OptionData> list = new List<TMP_Dropdown.OptionData>();
		list.Add(new TMP_Dropdown.OptionData(this.AutoText + autoRegion));
		this.Tokens.Add("");
		foreach (ServerName serverName in B.MultiplayerSettings.Servers)
		{
			list.Add(new TMP_Dropdown.OptionData(serverName.ServerCaption));
			this.Tokens.Add(serverName.ServerToken);
		}
		this.ServerList.AddOptions(list);
		this.ServerList.value = this.Tokens.FindIndex((string t) => t == PlayerProfile.ServerToken);
		this.ServerList.onValueChanged.AddListener(delegate(int value)
		{
			PlayerProfile.ServerToken = this.Tokens[value];
			LobbyManager.ConnectToServer();
			this.Timer = 0f;
		});
	}

	public void SetRegionServer(GameObject lego)
	{
		Debug.Log("SALUT A TOUS C MAGIC");
		string text = this.LabelServer.text;
		string text2;
		if (this.ServerList.value == 2)
		{
			text2 = "asia";
		}
		else if (this.ServerList.value == 1)
		{
			text2 = "eu";
		}
		else if (this.ServerList.value == 0)
		{
			text2 = "us";
		}
		else if (this.ServerList.value == 3)
		{
			text2 = "sa";
		}
		else
		{
			Debug.Log("[NO PERSISTANT] SALUT A TOUS C MAGIC");
			this.LabelServer.text = "(Auto)";
			text2 = "";
		}
		this.ServerListLautre.value = this.ServerList.value;
		PlayerPrefs.SetString("REGIONN", text2);
		PlayerProfile.ServerToken = text2;
		base.StopAllCoroutines();
		base.StartCoroutine(this.tempo());
	}

	public void Settxttkt(string lol)
	{
		Debug.Log("555555555");
		this.LabelServer.text = "(Auto) " + lol;
		base.StartCoroutine(this.tempo22(lol));
	}

	private IEnumerator tempo22(string jackk)
	{
		yield return new WaitForSeconds(0.1f);
		this.LabelServer.text = "(Auto) " + jackk;
		yield return new WaitForSeconds(0.1f);
		this.LabelServer.text = "(Auto) " + jackk;
		yield return new WaitForSeconds(1f);
		this.LabelServer.text = "(Auto) " + jackk;
		yield break;
	}

	private IEnumerator tempo()
	{
		yield return new WaitForSeconds(0.2f);
		LobbyManager.ConnectToServer();
		yield break;
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
	private TMP_Dropdown ServerList;

	[SerializeField]
	private TMP_Dropdown ServerListLautre;

	private List<string> Tokens = new List<string>();

	[SerializeField]
	private string AutoText = "(Auto) ";

	private float Timer;

	public Button[] allbtn;

	[SerializeField]
	private Button SelectCarButton;

	[SerializeField]
	private SelectCarMenuUI SelectCarMenuUI;

	[SerializeField]
	private TextMeshProUGUI PingText;

	[SerializeField]
	private TextMeshProUGUI LabelServer;

	[SerializeField]
	private Image PingIndicatorImage;

	[Space]
	public Sprite[] AllCarsSrpite;

	public string[] CarsName;

	public Image carsDisplay;

	public Image carsDisplay2;
}
