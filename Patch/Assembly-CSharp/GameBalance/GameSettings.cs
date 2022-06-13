using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameBalance
{
	[CreateAssetMenu(fileName = "GameSettings", menuName = "GameBalance/Settings/GameSettings")]
	public class GameSettings : ScriptableObject
	{
		public string MainMenuSceneName
		{
			get
			{
				return this.m_MainMenuSceneName;
			}
		}

		public RegimeSettings DefaultRegimeSettings
		{
			get
			{
				return this.m_DefaultRegimeSettings;
			}
		}

		public RegimeSettings RaceRegimeSettings
		{
			get
			{
				return this.m_RaceRegimeSettings;
			}
		}

		public DriftRegimeSettings DriftRegimeSettings
		{
			get
			{
				return this.m_DriftRegimeSettings;
			}
		}

		public List<string> BotNames
		{
			get
			{
				return this.m_BotNames;
			}
		}

		public List<TrackPreset> Tracks
		{
			get
			{
				this.m_Tracks.RemoveAll((TrackPreset t) => t == null);
				return this.m_Tracks;
			}
		}

		public void AddAvailableTrack(TrackPreset track)
		{
			this.m_Tracks.Add(track);
			this.m_Tracks.RemoveAll((TrackPreset t) => t == null);
		}

		[SerializeField]
		private string m_MainMenuSceneName = "MainMenuScene";

		[SerializeField]
		private RegimeSettings m_DefaultRegimeSettings;

		[SerializeField]
		private RegimeSettings m_RaceRegimeSettings;

		[SerializeField]
		private DriftRegimeSettings m_DriftRegimeSettings;

		[SerializeField]
		private List<TrackPreset> m_Tracks = new List<TrackPreset>();

		[SerializeField]
		private List<string> m_BotNames = new List<string>
		{
			"Mason(b)",
			"Emma(b)",
			"Sofia(b)",
			"William(b)",
			"Natalie(b)",
			"Michael(b)",
			"Emily(b)",
			"Jacob(b)"
		};
	}
}
