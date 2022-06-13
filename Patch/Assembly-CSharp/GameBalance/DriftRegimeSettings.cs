using System;
using UnityEngine;

namespace GameBalance
{
	[CreateAssetMenu(fileName = "DriftRegimeSettings", menuName = "GameBalance/Settings/DriftRegimeSettings")]
	public class DriftRegimeSettings : RegimeSettings
	{
		public float WaitDriftTime
		{
			get
			{
				return this.m_WaitDriftTime;
			}
		}

		public float WaitEndDriftTime
		{
			get
			{
				return this.m_WaitEndDriftTime;
			}
		}

		public float MinAngle
		{
			get
			{
				return this.m_MinAngle;
			}
		}

		public float MaxAngle
		{
			get
			{
				return this.m_MaxAngle;
			}
		}

		public float MinSpeed
		{
			get
			{
				return this.m_MinSpeed;
			}
		}

		public float ScorePerMeter
		{
			get
			{
				return this.m_ScorePerMeter;
			}
		}

		public int MaxMultiplier
		{
			get
			{
				return this.m_MaxMultiplier;
			}
		}

		public float MinScoreForIncMultiplier
		{
			get
			{
				return this.m_MinScoreForIncMultiplier;
			}
		}

		public float MoneyForDriftMultiplier
		{
			get
			{
				return this.m_MoneyForDriftMultiplier;
			}
		}

		[Header("Drift settings")]
		[SerializeField]
		private float m_WaitDriftTime = 1f;

		[SerializeField]
		private float m_WaitEndDriftTime = 1f;

		[SerializeField]
		private float m_MinAngle = 10f;

		[SerializeField]
		private float m_MaxAngle = 90f;

		[SerializeField]
		private float m_MinSpeed = 20f;

		[SerializeField]
		private float m_ScorePerMeter = 100f;

		[SerializeField]
		private int m_MaxMultiplier = 5;

		[SerializeField]
		private float m_MinScoreForIncMultiplier = 2000f;

		[SerializeField]
		private float m_MoneyForDriftMultiplier = 0.004f;
	}
}
