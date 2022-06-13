using System;
using UnityEngine;

namespace GameBalance
{
	[CreateAssetMenu(fileName = "GraphicsSettings", menuName = "GameBalance/Settings/GraphicsSettings")]
	public class GraphicsSettings : ScriptableObject
	{
		public int TargetFPS
		{
			get
			{
				if (!Application.isMobilePlatform)
				{
					return this.m_TargetFPSStandalone;
				}
				return this.m_TargetFPSMobile;
			}
		}

		[SerializeField]
		private int m_TargetFPSStandalone = 60;

		[SerializeField]
		private int m_TargetFPSMobile = 30;
	}
}
