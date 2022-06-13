using System;
using UnityEngine;

namespace GameBalance
{
	[CreateAssetMenu(fileName = "ResourcesSettings", menuName = "GameBalance/Settings/ResourcesSettings")]
	public class ResourcesSettings : ScriptableObject
	{
		public SoundControllerInUI SoundControllerInUI
		{
			get
			{
				return this.m_SoundControllerInUI;
			}
		}

		public LoadingScreenUI LoadingScreenUI
		{
			get
			{
				return this.m_LoadingScreenUI;
			}
		}

		public MessageBox MessageBox
		{
			get
			{
				return this.m_MessageBox;
			}
		}

		[SerializeField]
		private SoundControllerInUI m_SoundControllerInUI;

		[SerializeField]
		private LoadingScreenUI m_LoadingScreenUI;

		[SerializeField]
		private MessageBox m_MessageBox;
	}
}
