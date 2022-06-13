using System;
using UnityEngine;

namespace GameBalance
{
	[CreateAssetMenu(fileName = "LayerSettings", menuName = "GameBalance/Settings/LayerSettings")]
	public class LayerSettings : ScriptableObject
	{
		public LayerMask RoadMask
		{
			get
			{
				return this.m_RoadMask;
			}
		}

		public LayerMask BrakingGroundMask
		{
			get
			{
				return this.m_BrakingGroundMask;
			}
		}

		[SerializeField]
		private LayerMask m_RoadMask;

		[SerializeField]
		private LayerMask m_BrakingGroundMask;
	}
}
