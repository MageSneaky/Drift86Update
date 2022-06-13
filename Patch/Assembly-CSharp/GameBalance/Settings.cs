using System;
using UnityEngine;

namespace GameBalance
{
	[CreateAssetMenu(fileName = "Settings", menuName = "GameBalance/Settings/Settings")]
	public class Settings : ScriptableObject
	{
		public GameSettings GameSettings
		{
			get
			{
				return this.m_GameSettings;
			}
		}

		public GraphicsSettings GraphicsSettings
		{
			get
			{
				return this.m_GraphicsSettings;
			}
		}

		public LayerSettings LayerSettings
		{
			get
			{
				return this.m_LayerSettings;
			}
		}

		public SoundSettings SoundSettings
		{
			get
			{
				return this.m_SoundSettings;
			}
		}

		public ResourcesSettings ResourcesSettings
		{
			get
			{
				return this.m_ResourcesSettings;
			}
		}

		public MultiplayerSettings MultiplayerSettings
		{
			get
			{
				return this.m_MultiplayerSettings;
			}
		}

		[SerializeField]
		private GameSettings m_GameSettings;

		[SerializeField]
		private GraphicsSettings m_GraphicsSettings;

		[SerializeField]
		private LayerSettings m_LayerSettings;

		[SerializeField]
		private SoundSettings m_SoundSettings;

		[SerializeField]
		private ResourcesSettings m_ResourcesSettings;

		[SerializeField]
		private MultiplayerSettings m_MultiplayerSettings;
	}
}
