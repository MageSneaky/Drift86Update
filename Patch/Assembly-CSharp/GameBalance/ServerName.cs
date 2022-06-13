using System;
using UnityEngine;

namespace GameBalance
{
	[Serializable]
	public struct ServerName
	{
		public string ServerCaption
		{
			get
			{
				return this.m_ServerCaption;
			}
		}

		public string ServerToken
		{
			get
			{
				return this.m_ServerToken;
			}
		}

		[SerializeField]
		private string m_ServerCaption;

		[SerializeField]
		private string m_ServerToken;
	}
}
