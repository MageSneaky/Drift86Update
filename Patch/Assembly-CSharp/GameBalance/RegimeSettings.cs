using System;
using System.Collections.Generic;
using PG_Atributes;
using PG_Physics.Wheel;
using UnityEngine;

namespace GameBalance
{
	[CreateAssetMenu(fileName = "RegimeSettings", menuName = "GameBalance/Settings/RegimeSettings")]
	public class RegimeSettings : ScriptableObject
	{
		public string RegimeSceneName
		{
			get
			{
				return this.m_RegimeSceneName;
			}
		}

		public RegimeSettings.AiConfig GetAiConfig
		{
			get
			{
				return this.m_AiConfog;
			}
		}

		public string RegimeCaption
		{
			get
			{
				return this.m_RegimeCaption;
			}
		}

		public Sprite RegimeImage
		{
			get
			{
				return this.m_RegimeImage;
			}
		}

		public List<CarPreset> AvailableCars
		{
			get
			{
				this.m_AvailableCars.RemoveAll((CarPreset c) => c == null);
				return this.m_AvailableCars;
			}
		}

		public bool EnableSteerAngleMultiplier
		{
			get
			{
				return this.m_EnableSteerAngleMultiplier;
			}
		}

		public float MinSteerAngleMultiplier
		{
			get
			{
				return this.m_MinSteerAngleMultiplier;
			}
		}

		public float MaxSteerAngleMultiplier
		{
			get
			{
				return this.m_MaxSteerAngleMultiplier;
			}
		}

		public float MaxSpeedForMinAngleMultiplier
		{
			get
			{
				return this.m_MaxSpeedForMinAngleMultiplier;
			}
		}

		public float SteerAngleChangeSpeed
		{
			get
			{
				return this.m_SteerAngleChangeSpeed;
			}
		}

		public float MinSpeedForSteerHelp
		{
			get
			{
				return this.m_MinSpeedForSteerHelp;
			}
		}

		public float HelpSteerPower
		{
			get
			{
				return this.m_HelpSteerPower;
			}
		}

		public float OppositeAngularVelocityHelpPower
		{
			get
			{
				return this.m_OppositeVelocityHelpPower;
			}
		}

		public float PositiveAngularVelocityHelpPower
		{
			get
			{
				return this.m_PositiveAngularVelocityHelpPower;
			}
		}

		public float MaxAngularVelocityHelpAngle
		{
			get
			{
				return this.m_MaxAngularVelocityHelpAngle;
			}
		}

		public float AngularVelucityInMaxAngle
		{
			get
			{
				return this.m_AngularVelucityInMaxAngle;
			}
		}

		public float AngularVelucityInMinAngle
		{
			get
			{
				return this.m_AngularVelucityInMinAngle;
			}
		}

		public float HandBrakeForwardStiffness
		{
			get
			{
				return this.m_HandBrakeForwardStiffness;
			}
		}

		public float HandBrakeSidewaysStiffness
		{
			get
			{
				return this.m_HandBrakeSidewaysStiffness;
			}
		}

		public PG_WheelColliderConfig FrontWheelsConfig
		{
			get
			{
				return this.m_FrontWheelsConfig;
			}
		}

		public PG_WheelColliderConfig RearWheelsConfig
		{
			get
			{
				return this.m_RearWheelsConfig;
			}
		}

		public void AddAvailableCar(CarPreset car)
		{
			this.m_AvailableCars.Add(car);
			this.m_AvailableCars.RemoveAll((CarPreset c) => c == null);
		}

		[SerializeField]
		private string m_RegimeSceneName;

		[SerializeField]
		private List<CarPreset> m_AvailableCars = new List<CarPreset>();

		[SerializeField]
		private RegimeSettings.AiConfig m_AiConfog;

		[Header("Info")]
		[SerializeField]
		private string m_RegimeCaption;

		[SerializeField]
		private Sprite m_RegimeImage;

		[Header("Main settings")]
		[SerializeField]
		private bool m_EnableSteerAngleMultiplier = true;

		[SerializeField]
		[ShowInInspectorIf("m_EnableSteerAngleMultiplier")]
		private float m_MinSteerAngleMultiplier = 0.05f;

		[SerializeField]
		[ShowInInspectorIf("m_EnableSteerAngleMultiplier")]
		private float m_MaxSteerAngleMultiplier = 1f;

		[SerializeField]
		[ShowInInspectorIf("m_EnableSteerAngleMultiplier")]
		private float m_MaxSpeedForMinAngleMultiplier = 250f;

		[Space(10f)]
		[SerializeField]
		private float m_SteerAngleChangeSpeed = 1f;

		[SerializeField]
		private float m_MinSpeedForSteerHelp = 20f;

		[SerializeField]
		[Range(0f, 1f)]
		private float m_HelpSteerPower = 0.8f;

		[SerializeField]
		private float m_OppositeVelocityHelpPower = 0.1f;

		[SerializeField]
		private float m_PositiveAngularVelocityHelpPower = 0.1f;

		[SerializeField]
		private float m_MaxAngularVelocityHelpAngle = 90f;

		[SerializeField]
		private float m_AngularVelucityInMaxAngle = 0.5f;

		[SerializeField]
		private float m_AngularVelucityInMinAngle = 4f;

		[SerializeField]
		private float m_HandBrakeForwardStiffness = 0.5f;

		[SerializeField]
		private float m_HandBrakeSidewaysStiffness = 0.5f;

		[SerializeField]
		private PG_WheelColliderConfig m_FrontWheelsConfig;

		[SerializeField]
		private PG_WheelColliderConfig m_RearWheelsConfig;

		[Serializable]
		public class AiConfig
		{
			public float MaxSpeed = 160f;

			public float MinSpeed = 30f;

			public float AccelSensitivity = 1f;

			public float BrakeSensitivity = 1f;

			public float ReverceWaitTime = 2f;

			public float ReverceTime = 2f;

			public float BetweenReverceTimeForReset = 6f;

			public float OffsetToFirstTargetPoint = 5f;

			public float SpeedFactorToFirstTargetPoint = -0.7f;

			public float OffsetToSecondTargetPoint = 11f;

			public float SpeedFactorToSecondTargetPoint = 0.6f;

			public float LookAngleSppedFactor = 30f;

			public float SetSteerAngleSensitivity = 5f;
		}
	}
}
