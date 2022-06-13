using System;
using System.Collections.Generic;
using GameBalance;
using UnityEngine;

namespace CreateHelper
{
	[CreateAssetMenu(fileName = "CreateHelperSettings", menuName = "Utils/CreateHelperSettings")]
	public class CreateHelperSettings : ScriptableObject
	{
		public List<CarColorPreset> ColorPresets
		{
			get
			{
				return this.m_ColorPresets;
			}
		}

		public CarPreset CarPresetRef
		{
			get
			{
				return this.m_CarPresetRef;
			}
		}

		public string CarPrefabSavePath
		{
			get
			{
				return this.m_CarPrefabSavePath;
			}
		}

		public string CarAssetSavePath
		{
			get
			{
				return this.m_CarAssetSavePath;
			}
		}

		public RegimeSettings DefaultRegime
		{
			get
			{
				return this.m_DefaultRegime;
			}
		}

		public FXController FXControllerRef
		{
			get
			{
				return this.m_FXControllerRef;
			}
		}

		public GameController GameControllerRef
		{
			get
			{
				return this.m_GameControllerRef;
			}
		}

		public string TrackSceneSavePath
		{
			get
			{
				return this.m_TrackSceneSavePath;
			}
		}

		public string TrackAssetSavePath
		{
			get
			{
				return this.m_TrackAssetSavePath;
			}
		}

		public string GameControllerSavePath
		{
			get
			{
				return this.m_GameControllerSavePath;
			}
		}

		[Header("Car settings")]
		[SerializeField]
		private CarPreset m_CarPresetRef;

		[SerializeField]
		private List<CarColorPreset> m_ColorPresets = new List<CarColorPreset>();

		[SerializeField]
		private string m_CarPrefabSavePath = "Assets/ACC/Prefabs/Cars/";

		[SerializeField]
		private string m_CarAssetSavePath = "Assets/ACC/Balance/Cars/";

		[Space(10f)]
		[Header("TrackSettings")]
		[SerializeField]
		private RegimeSettings m_DefaultRegime;

		[SerializeField]
		private FXController m_FXControllerRef;

		[SerializeField]
		private GameController m_GameControllerRef;

		[SerializeField]
		private string m_TrackSceneSavePath = "Assets/ACC/Scenes/";

		[SerializeField]
		private string m_TrackAssetSavePath = "Assets/ACC/Balance/Tracks/";

		[SerializeField]
		private string m_GameControllerSavePath = "Assets/ACC/Prefabs/GameControllers/";
	}
}
