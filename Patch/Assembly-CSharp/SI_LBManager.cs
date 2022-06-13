using System;
using HeathenEngineering.SteamAPI;
using UnityEngine;

public class SI_LBManager : MonoBehaviour
{
	private void Start()
	{
	}

	private void Update()
	{
	}

	public void SendScore()
	{
		this.totaldriftscore.UploadScore(PlayerProfile.TotalScore, 1, null);
		if (PlayerProfile.BestScoreCar == "eg86")
		{
			this.vehiculeUsed = 1;
		}
		else if (PlayerProfile.BestScoreCar == "BMG v92")
		{
			this.vehiculeUsed = 2;
		}
		else if (PlayerProfile.BestScoreCar == "MuscleCar 1969")
		{
			this.vehiculeUsed = 3;
		}
		else if (PlayerProfile.BestScoreCar == "SunLine GTE")
		{
			this.vehiculeUsed = 4;
		}
		else if (PlayerProfile.BestScoreCar == "eg86 Tuned")
		{
			this.vehiculeUsed = 5;
		}
		else if (PlayerProfile.BestScoreCar == "XR-7")
		{
			this.vehiculeUsed = 6;
		}
		else if (PlayerProfile.BestScoreCar == "X13")
		{
			this.vehiculeUsed = 7;
		}
		else if (PlayerProfile.BestScoreCar == "SuprX")
		{
			this.vehiculeUsed = 8;
		}
		else if (PlayerProfile.BestScoreCar == "Eva X")
		{
			this.vehiculeUsed = 9;
		}
		else if (PlayerProfile.BestScoreCar == "RCX")
		{
			this.vehiculeUsed = 10;
		}
		else if (PlayerProfile.BestScoreCar == "6ge")
		{
			this.vehiculeUsed = 11;
		}
		else if (PlayerProfile.BestScoreCar == "Clia V6")
		{
			this.vehiculeUsed = 12;
		}
		else if (PlayerProfile.BestScoreCar == "SunLine GTE35")
		{
			this.vehiculeUsed = 13;
		}
		else if (PlayerProfile.BestScoreCar == "Vez 6012")
		{
			this.vehiculeUsed = 14;
		}
		else if (PlayerProfile.BestScoreCar == "Eva VIII")
		{
			this.vehiculeUsed = 15;
		}
		else if (PlayerProfile.BestScoreCar == "Imprezo")
		{
			this.vehiculeUsed = 16;
		}
		else if (PlayerProfile.BestScoreCar == "SunLine 2000")
		{
			this.vehiculeUsed = 17;
		}
		else if (PlayerProfile.BestScoreCar == "tg68")
		{
			this.vehiculeUsed = 18;
		}
		else if (PlayerProfile.BestScoreCar == "SuprXX")
		{
			this.vehiculeUsed = 19;
		}
		else if (PlayerProfile.BestScoreCar == "R32 Black Reaper")
		{
			this.vehiculeUsed = 20;
		}
		else if (PlayerProfile.BestScoreCar == "NXS")
		{
			this.vehiculeUsed = 21;
		}
		else if (PlayerProfile.BestScoreCar == "xm5")
		{
			this.vehiculeUsed = 22;
		}
		else if (PlayerProfile.BestScoreCar == "S13")
		{
			this.vehiculeUsed = 23;
		}
		else if (PlayerProfile.BestScoreCar == "XR-7C")
		{
			this.vehiculeUsed = 24;
		}
		else if (PlayerProfile.BestScoreCar == "Might 2")
		{
			this.vehiculeUsed = 25;
		}
		else if (PlayerProfile.BestScoreCar == "Sunline 31")
		{
			this.vehiculeUsed = 26;
		}
		else if (PlayerProfile.BestScoreCar == "Sunline 31 MayoMan")
		{
			this.vehiculeUsed = 27;
		}
		else if (PlayerProfile.BestScoreCar == "Falcon 400")
		{
			this.vehiculeUsed = 28;
		}
		else if (PlayerProfile.BestScoreCar == "W4 GTS")
		{
			this.vehiculeUsed = 29;
		}
		else if (PlayerProfile.BestScoreCar == "Vulture")
		{
			this.vehiculeUsed = 30;
		}
		else if (PlayerProfile.BestScoreCar == "Vulcano")
		{
			this.vehiculeUsed = 31;
		}
		else if (PlayerProfile.BestScoreCar == "910E")
		{
			this.vehiculeUsed = 32;
		}
		else if (PlayerProfile.BestScoreCar == "Hot Rod")
		{
			this.vehiculeUsed = 33;
		}
		else if (PlayerProfile.BestScoreCar == "Roadster")
		{
			this.vehiculeUsed = 34;
		}
		else if (PlayerProfile.BestScoreCar == "Fire GT")
		{
			this.vehiculeUsed = 35;
		}
		else if (PlayerProfile.BestScoreCar == "Valior STO")
		{
			this.vehiculeUsed = 36;
		}
		else if (PlayerProfile.BestScoreCar == "Forceur")
		{
			this.vehiculeUsed = 37;
		}
		else if (PlayerProfile.BestScoreCar == "Vanillac")
		{
			this.vehiculeUsed = 38;
		}
		else if (PlayerProfile.BestScoreCar == "Camarax")
		{
			this.vehiculeUsed = 39;
		}
		else if (PlayerProfile.BestScoreCar == "Bunta")
		{
			this.vehiculeUsed = 40;
		}
		else if (PlayerProfile.BestScoreCar == "K9")
		{
			this.vehiculeUsed = 41;
		}
		else
		{
			this.vehiculeUsed = 0;
		}
		int[] array = new int[]
		{
			this.vehiculeUsed
		};
		this.bestdriftscore.UploadScore(PlayerProfile.BestScore, array, 1, null);
		Debug.Log("UPLAOD  de : " + array[0]);
		this.totaldistance.UploadScore((int)PlayerProfile.TotalDistance, 1, null);
	}

	public void CloseDisplayLB()
	{
		this.display_lb1.SetActive(false);
		this.display_lb2.SetActive(false);
		this.display_lb3.SetActive(false);
		this.display_lb4.SetActive(false);
	}

	public void UpdateScore()
	{
		this.totaldriftscore.QueryTopEntries(100, null);
		this.bestdriftscore.QueryTopEntries(100, null);
		this.totaldistance.QueryTopEntries(100, null);
		if (this.totaldriftscore.userEntry != null)
		{
			Debug.Log("The user's rank is: " + this.totaldriftscore.userEntry.Rank.ToString());
			Debug.Log("The user's score is: " + this.totaldriftscore.userEntry.Score.ToString());
			return;
		}
		Debug.Log("PAS DE DATA FRR");
	}

	public LeaderboardObject totaldriftscore;

	public LeaderboardObject bestdriftscore;

	public LeaderboardObject totaldistance;

	public GameObject display_lb1;

	public GameObject display_lb2;

	public GameObject display_lb3;

	public GameObject display_lb4;

	private int[] additionalData;

	public int vehiculeUsed;
}
