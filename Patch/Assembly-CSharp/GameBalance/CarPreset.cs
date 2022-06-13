using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameBalance
{
	[CreateAssetMenu(fileName = "Car", menuName = "GameBalance/Game/CarPreset")]
	public class CarPreset : LockedContent
	{
		public float GetPower
		{
			get
			{
				return this.m_CarPrefab.GetCarConfig.MaxMotorTorque;
			}
		}

		public float GetControl
		{
			get
			{
				return this.m_CarPrefab.GetCarConfig.MaxSteerAngle;
			}
		}

		public float GetMass
		{
			get
			{
				return this.m_CarPrefab.RB.mass;
			}
		}

		public string CarCaption
		{
			get
			{
				return this.m_CarCaption;
			}
		}

		public CarController CarPrefab
		{
			get
			{
				return this.m_CarPrefab;
			}
		}

		public GameObject CarPrefabForSelectMenu
		{
			get
			{
				return this.m_CarPrefabForSelectMenu;
			}
		}

		public List<CarColorPreset> AvailibleColors
		{
			get
			{
				return this.m_AvailibleColors;
			}
		}

		public string Description
		{
			get
			{
				return this.m_Description;
			}
		}

		public CarColorPreset GetRandomColor()
		{
			return this.AvailibleColors[Random.Range(0, this.AvailibleColors.Count)];
		}

		public CarPreset(string carCaption, GameObject carPrefabForSelectMenu, CarController carPrefab, List<CarColorPreset> colors, string description, LockedContent.UnlockType unlockType, int price, TrackPreset completeTrackForUnlock)
		{
			this.m_CarCaption = carCaption;
			this.m_CarPrefabForSelectMenu = carPrefabForSelectMenu;
			this.m_CarPrefab = carPrefab;
			this.m_AvailibleColors = colors;
			this.m_Description = description;
			this.Unlock = unlockType;
			this.Price = price;
			this.CompleteTrackForUnlock = completeTrackForUnlock;
		}

		[SerializeField]
		private string m_CarCaption;

		[SerializeField]
		private GameObject m_CarPrefabForSelectMenu;

		[SerializeField]
		private CarController m_CarPrefab;

		[SerializeField]
		private List<CarColorPreset> m_AvailibleColors = new List<CarColorPreset>();

		[SerializeField]
		[TextArea(2, 5)]
		private string m_Description;
	}
}
