using System;
using HeathenEngineering.SteamAPI;
using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BestResultsUI : WindowWithShowHideAnimators
{
	public void SetPP()
	{
		PlayerPrefs.SetInt("TotalScore_ob", this.TotalScorePrefName);
		PlayerPrefs.SetInt("BestScore_ob", this.BestScorePrefName);
		PlayerPrefs.SetFloat("TotalDistance_ob", (float)this.TotalDistance);
	}

	public override void Open()
	{
		base.Open();
		int num = (int)PlayerProfile.TotalDistance;
		this.BestTotalScoreText.text = PlayerProfile.TotalScore.ToString();
		this.BestScoreText.text = string.Format("{0}: {1}", PlayerProfile.BestScoreCar, PlayerProfile.BestScore);
		this.BestRaceTimeText.text = PlayerProfile.RaceTime.ToStringTime();
		this.TotalDistanceText.text = num.ToString();
		this.CheckForSuccess();
		Object.FindObjectOfType<SI_LBManager>().SendScore();
		this.RankBestTotalScoreText.text = this.totaldriftscore.userEntry.Rank.ToString();
		this.RankBestScoreText.text = this.bestdriftscore.userEntry.Rank.ToString();
		this.RankTotalDistanceText.text = this.totaldistance.userEntry.Rank.ToString();
	}

	public void Start()
	{
		this.MyFace = Object.FindObjectOfType<SI_GetUserData>().MyAvatar;
		this.MyFaceUI.texture = this.MyFace;
		this.MyName = Object.FindObjectOfType<SI_GetUserData>().playername;
		this.MyNameUI.text = this.MyName;
	}

	public void UUpdateScore()
	{
		Object.FindObjectOfType<SI_LBManager>().UpdateScore();
	}

	public void CheckForSuccess()
	{
		if (PlayerProfile.TotalScore > 1000000)
		{
			SteamUserStats.SetAchievement("TDS_1M");
			SteamSettings.Client.StoreStatsAndAchievements();
			if (PlayerProfile.TotalScore > 5000000)
			{
				SteamUserStats.SetAchievement("TDS_5M");
				SteamSettings.Client.StoreStatsAndAchievements();
				if (PlayerProfile.TotalScore > 10000000)
				{
					SteamUserStats.SetAchievement("TDS_10M");
					SteamSettings.Client.StoreStatsAndAchievements();
					if (PlayerProfile.TotalScore > 100000000)
					{
						SteamUserStats.SetAchievement("TDS_100M");
						SteamSettings.Client.StoreStatsAndAchievements();
					}
				}
			}
		}
		if (PlayerProfile.BestScore > 50000)
		{
			SteamUserStats.SetAchievement("BDS_50K");
			SteamSettings.Client.StoreStatsAndAchievements();
			if (PlayerProfile.BestScore > 100000)
			{
				SteamUserStats.SetAchievement("BDS_100K");
				SteamSettings.Client.StoreStatsAndAchievements();
				if (PlayerProfile.BestScore > 500000)
				{
					SteamUserStats.SetAchievement("BDS_500K");
					SteamSettings.Client.StoreStatsAndAchievements();
					if (PlayerProfile.BestScore > 1000000)
					{
						SteamUserStats.SetAchievement("BDS_1M");
						SteamSettings.Client.StoreStatsAndAchievements();
					}
				}
			}
		}
		if (PlayerProfile.RaceTime > 3600f)
		{
			SteamUserStats.SetAchievement("TRT_1H");
			SteamSettings.Client.StoreStatsAndAchievements();
			if (PlayerProfile.RaceTime > 36000f)
			{
				SteamUserStats.SetAchievement("TRT_10H");
				SteamSettings.Client.StoreStatsAndAchievements();
				if (PlayerProfile.RaceTime > 360000f)
				{
					SteamUserStats.SetAchievement("TRT_100H");
					SteamSettings.Client.StoreStatsAndAchievements();
				}
			}
		}
	}

	[SerializeField]
	private TextMeshProUGUI BestTotalScoreText;

	[SerializeField]
	private TextMeshProUGUI BestScoreText;

	[SerializeField]
	private TextMeshProUGUI BestRaceTimeText;

	[SerializeField]
	private TextMeshProUGUI TotalDistanceText;

	[Space]
	[SerializeField]
	private Text RankBestTotalScoreText;

	[SerializeField]
	private Text RankBestScoreText;

	[SerializeField]
	private Text RankTotalDistanceText;

	[Space]
	[SerializeField]
	private LeaderboardObject totaldriftscore;

	[Space]
	[SerializeField]
	private LeaderboardObject bestdriftscore;

	[Space]
	[SerializeField]
	private LeaderboardObject totaldistance;

	[Space]
	[SerializeField]
	private Texture MyFace;

	[SerializeField]
	private string MyName;

	[SerializeField]
	private Text MyNameUI;

	[SerializeField]
	private RawImage MyFaceUI;

	[Space]
	public int TotalScorePrefName;

	public int BestScorePrefName;

	public int TotalDistance;
}
