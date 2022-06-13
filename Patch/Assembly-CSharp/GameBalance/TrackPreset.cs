using System;
using UnityEngine;

namespace GameBalance
{
	[CreateAssetMenu(fileName = "Track", menuName = "GameBalance/Game/TrackPreset")]
	public class TrackPreset : LockedContent
	{
		public string TrackName
		{
			get
			{
				return this.m_TrackName;
			}
		}

		public Sprite TrackIcon
		{
			get
			{
				return this.m_TrackIcon;
			}
		}

		public string SceneName
		{
			get
			{
				return this.m_SceneName;
			}
		}

		public GameController GameController
		{
			get
			{
				return this.m_GameController;
			}
		}

		public int LapsCount
		{
			get
			{
				return this.m_LapsCount;
			}
		}

		public int AIsCount
		{
			get
			{
				return this.m_AIsCount;
			}
		}

		public RegimeSettings RegimeSettings
		{
			get
			{
				return this.m_RegimeSettings;
			}
		}

		public float MoneyForFirstPlace
		{
			get
			{
				return this.m_MoneyForFirstPlace;
			}
		}

		public TrackPreset(string trackName, Sprite trackIcon, string sceneName, GameController gameController, int lapsCount, int aisCount, RegimeSettings regimeSettings, int money, LockedContent.UnlockType unlock, int price, TrackPreset completeTrackForUnlock)
		{
			this.m_TrackName = trackName;
			this.m_TrackIcon = trackIcon;
			this.m_SceneName = sceneName;
			this.m_GameController = gameController;
			this.m_LapsCount = lapsCount;
			this.m_AIsCount = aisCount;
			this.m_RegimeSettings = regimeSettings;
			this.m_MoneyForFirstPlace = (float)money;
			this.Unlock = unlock;
			this.Price = price;
			this.CompleteTrackForUnlock = completeTrackForUnlock;
		}

		[SerializeField]
		private string m_TrackName;

		[SerializeField]
		private Sprite m_TrackIcon;

		[SerializeField]
		private string m_SceneName;

		[SerializeField]
		private GameController m_GameController;

		[SerializeField]
		private int m_LapsCount = 1;

		[SerializeField]
		private int m_AIsCount = 3;

		[SerializeField]
		private RegimeSettings m_RegimeSettings;

		[SerializeField]
		private float m_MoneyForFirstPlace = 1000f;
	}
}
