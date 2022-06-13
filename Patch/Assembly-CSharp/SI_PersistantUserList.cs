using System;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SI_PersistantUserList : MonoBehaviour
{
	private void Start()
	{
	}

	private void Awake()
	{
		this.nbJoueur = 0;
		this.LastCheckNbJoueur = 0;
		this.tempo = 10;
	}

	private void Update()
	{
		if (PhotonNetwork.InRoom && this.tempo == 10)
		{
			this.PlayerNamePhoton = UnityEngine.Object.FindObjectsOfType<PhotonView>();
			this.pcount.text = string.Concat(PhotonNetwork.CurrentRoom.PlayerCount);
			this.UpdatePlayerList();
		}
	}

	public void UpdatePlayerList()
	{
		this.actualplayer = "";
		for (int num = 0; num != this.PlayerNamePhoton.Length; num++)
		{
			this.actualplayer = this.actualplayer + "|" + this.PlayerNamePhoton[num].ViewID.ToString() + "|";
		}
		for (int num2 = 0; num2 != this.PlayerNamePhoton.Length; num2++)
		{
			Debug.Log("BEFORE HECKING NUMERO JOUEUR :  " + num2);
			string text = "|" + this.PlayerNamePhoton[num2].ViewID.ToString() + "|";
			if (!this.jack.Contains(text))
			{
				Debug.Log("HECKING ID :  " + text);
				this.jack = string.Concat(new object[]
				{
					this.jack,
					"|",
					this.PlayerNamePhoton[num2].ViewID,
					"|"
				});
				UnityEngine.Object.Instantiate<GameObject>(this.NewPlayerList, this.NewPlayerList.transform.parent);
				this.NewPlayerList.SetActive(true);
				this.NewPlayerList.GetComponentInChildren<TextMeshProUGUI>().text = (this.PlayerNamePhoton[num2].Owner.NickName ?? "");
				this.NewPlayerList.GetComponentInChildren<Shadow>().gameObject.GetComponent<Text>().text = string.Concat(this.PlayerNamePhoton[num2].ViewID);
			}
		}
		this.newPL = GameObject.FindGameObjectsWithTag("newPL");
		foreach (GameObject gameObject in this.newPL)
		{
			if (!this.actualplayer.Contains(gameObject.GetComponent<Text>().text))
			{
				this.jack.Replace("|" + gameObject.GetComponent<Text>().text + "|", "");
				if (gameObject.GetComponentInParent<Image>().gameObject.name != "PlayerlistGO(Clone)")
				{
					gameObject.GetComponentInParent<Image>().gameObject.SetActive(false);
				}
				else
				{
					UnityEngine.Object.Destroy(gameObject.GetComponentInParent<Image>().gameObject);
				}
			}
		}
	}

	public GameObject[] newPL;

	public PhotonView[] PlayerNamePhoton;

	public GameObject NewPlayerList;

	public string jack;

	public string actualplayer;

	public TextMeshProUGUI pcount;

	public int nbJoueur;

	public int LastCheckNbJoueur;

	public int tempo;
}
