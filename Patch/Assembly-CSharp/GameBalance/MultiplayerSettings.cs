using System;
using System.Collections.Generic;
using Photon.Realtime;
using TMPro;
using UnityEngine;

namespace GameBalance
{
	[CreateAssetMenu(fileName = "MultiplayerSettings", menuName = "GameBalance/Settings/MultiplayerSettings")]
	public class MultiplayerSettings : ScriptableObject
	{
		public List<ServerName> Servers
		{
			get
			{
				return this.m_Servers;
			}
		}

		public float PingUpdateSettings
		{
			get
			{
				return this.m_PingUpdateSettings;
			}
		}

		public Sprite BadPingSprite
		{
			get
			{
				return this.m_BadPingSprite;
			}
		}

		public Sprite MediumPingSprite
		{
			get
			{
				return this.m_MediumPingSprite;
			}
		}

		public Sprite GoodPingSprite
		{
			get
			{
				return this.m_GoodPingSprite;
			}
		}

		public Sprite VeryGoodPingSprite
		{
			get
			{
				return this.m_VeryGoodPingSprite;
			}
		}

		public int MediumPing
		{
			get
			{
				return this.m_MediumPing;
			}
		}

		public int GoodPing
		{
			get
			{
				return this.m_GoodPing;
			}
		}

		public int VeryGoodPing
		{
			get
			{
				return this.m_VeryGoodPing;
			}
		}

		public List<TrackPreset> AvailableTracksForMultiplayer
		{
			get
			{
				this.m_AvailableTracksForMultiplayer.RemoveAll((TrackPreset t) => t == null);
				return this.m_AvailableTracksForMultiplayer;
			}
		}

		public List<CarPreset> AvailableCarsForMultiplayer
		{
			get
			{
				this.m_AvailableCarsForMultiplayer.RemoveAll((CarPreset c) => c == null);
				return this.m_AvailableCarsForMultiplayer;
			}
		}

		public byte MaxPlayersInRoom
		{
			get
			{
				return this.m_MaxPlayersInRoom;
			}
		}

		public float WaitOtherPlayersTime
		{
			get
			{
				return this.m_WaitOtherPlayersTime;
			}
		}

		public int SecondsToEndGame
		{
			get
			{
				return this.m_SecondsToEndGame;
			}
		}

		public float SlowPosSyncLerp
		{
			get
			{
				return this.m_SlowPosSyncLerp;
			}
		}

		public float SlowRotSyncLerp
		{
			get
			{
				return this.m_SlowRotSyncLerp;
			}
		}

		public float FastPosSyncLerp
		{
			get
			{
				return this.m_FastPosSyncLerp;
			}
		}

		public float FastRotSyncLerp
		{
			get
			{
				return this.m_FastRotSyncLerp;
			}
		}

		public float DistanceFastSync
		{
			get
			{
				return this.m_DistanceFastSync;
			}
		}

		public float DistanceTeleport
		{
			get
			{
				return this.m_DistanceTeleport;
			}
		}

		public bool EnableAiForDebug
		{
			get
			{
				return this.m_EnableAiForDebug;
			}
		}

		public KeyCode ShowNickNameCode
		{
			get
			{
				return this.m_ShowNickNameCode;
			}
		}

		public TextMeshPro NickNameInWorld
		{
			get
			{
				return this.m_NickNameInWorld;
			}
		}

		public float NickNameY
		{
			get
			{
				return this.m_NickNameY;
			}
		}

		public List<Color> NickNameColors
		{
			get
			{
				return this.m_NickNameColors;
			}
		}

		public void ShowDisconnectCause(DisconnectCause cause, Action onCloseAction = null)
		{
			if (cause == DisconnectCause.DisconnectByClientLogic)
			{
				onCloseAction.SafeInvoke();
				return;
			}
			string message = string.Empty;
			if (!this.DisconnectCauseStrings.TryGetValue(cause, out message))
			{
				message = cause.ToString();
			}
			MessageBox.Show(message, null, onCloseAction, "", "Close");
		}

		public void AddAvailableTrack(TrackPreset track)
		{
			this.m_AvailableTracksForMultiplayer.RemoveAll((TrackPreset t) => t == null);
			this.m_AvailableTracksForMultiplayer.Add(track);
		}

		public void AddAvailableCar(CarPreset car)
		{
			this.m_AvailableCarsForMultiplayer.RemoveAll((CarPreset c) => c == null);
			this.m_AvailableCarsForMultiplayer.Add(car);
		}

		[SerializeField]
		private List<ServerName> m_Servers = new List<ServerName>();

		[Header("Ping info settings")]
		[SerializeField]
		private float m_PingUpdateSettings = 1f;

		[SerializeField]
		private Sprite m_BadPingSprite;

		[SerializeField]
		private Sprite m_MediumPingSprite;

		[SerializeField]
		private Sprite m_GoodPingSprite;

		[SerializeField]
		private Sprite m_VeryGoodPingSprite;

		[SerializeField]
		private int m_MediumPing = 150;

		[SerializeField]
		private int m_GoodPing = 100;

		[SerializeField]
		private int m_VeryGoodPing = 50;

		[Space(10f)]
		[Header("Available content for multiplayer")]
		[SerializeField]
		private List<TrackPreset> m_AvailableTracksForMultiplayer = new List<TrackPreset>();

		[SerializeField]
		private List<CarPreset> m_AvailableCarsForMultiplayer = new List<CarPreset>();

		[Space(10f)]
		[SerializeField]
		private byte m_MaxPlayersInRoom = 4;

		[SerializeField]
		private float m_WaitOtherPlayersTime = 3f;

		[SerializeField]
		private int m_SecondsToEndGame = 20;

		[Space(10f)]
		[SerializeField]
		private float m_SlowPosSyncLerp = 0.1f;

		[SerializeField]
		private float m_SlowRotSyncLerp = 0.1f;

		[Space(10f)]
		[SerializeField]
		private float m_FastPosSyncLerp = 0.3f;

		[SerializeField]
		private float m_FastRotSyncLerp = 0.3f;

		[Space(10f)]
		[SerializeField]
		private float m_DistanceFastSync = 2f;

		[SerializeField]
		private float m_DistanceTeleport = 5f;

		[Space(10f)]
		[SerializeField]
		private KeyCode m_ShowNickNameCode = KeyCode.Tab;

		[SerializeField]
		private TextMeshPro m_NickNameInWorld;

		[SerializeField]
		private float m_NickNameY = 2.3f;

		[SerializeField]
		private List<Color> m_NickNameColors;

		[Space(10f)]
		[Header("Debug settings")]
		[SerializeField]
		private bool m_EnableAiForDebug;

		public Dictionary<DisconnectCause, string> DisconnectCauseStrings = new Dictionary<DisconnectCause, string>
		{
			{
				DisconnectCause.ServerTimeout,
				"Server timeout"
			},
			{
				DisconnectCause.MaxCcuReached,
				"Max CCU has reached, try connecting later."
			},
			{
				DisconnectCause.ClientTimeout,
				"Client timeout"
			}
		};
	}
}
