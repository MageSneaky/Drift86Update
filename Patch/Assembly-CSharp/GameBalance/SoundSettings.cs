using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace GameBalance
{
	[CreateAssetMenu(fileName = "SoundSettings", menuName = "GameBalance/Settings/SoundSettings")]
	public class SoundSettings : ScriptableObject
	{
		public AudioMixerSnapshot StandartSnapshot
		{
			get
			{
				return this.m_StandartSnapshot;
			}
		}

		public AudioMixerSnapshot PauseSnapshot
		{
			get
			{
				return this.m_PauseSnapshot;
			}
		}

		public AudioMixerSnapshot MuteSnapshot
		{
			get
			{
				return this.m_MuteSnapshot;
			}
		}

		public AudioClip AsphaltSlip
		{
			get
			{
				return this.m_AsphaltSlip;
			}
		}

		public AudioClip GroundSlip
		{
			get
			{
				return this.m_GroundSlip;
			}
		}

		public float CollisionSoundMultiplier
		{
			get
			{
				return this.m_CollisionSoundMultiplier;
			}
		}

		public AudioClip GetAudioClipCollision(int layer)
		{
			AudioClip result;
			if (SoundSettings.CollisionsSounds == null)
			{
				SoundSettings.CollisionsSounds = new Dictionary<int, AudioClip>();
				for (int i = 0; i < this.m_AudioClips.Count; i++)
				{
					if (SoundSettings.CollisionsSounds.TryGetValue(this.m_Layers[i], out result))
					{
						Debug.LogErrorFormat("Doble layer: {0}", new object[]
						{
							this.m_Layers[i]
						});
					}
					else
					{
						SoundSettings.CollisionsSounds.Add(this.m_Layers[i], this.m_AudioClips[i]);
					}
				}
			}
			if (SoundSettings.CollisionsSounds.TryGetValue(layer, out result))
			{
				return result;
			}
			return null;
		}

		[Header("Global settings")]
		[SerializeField]
		private AudioMixerSnapshot m_StandartSnapshot;

		[SerializeField]
		private AudioMixerSnapshot m_PauseSnapshot;

		[SerializeField]
		private AudioMixerSnapshot m_MuteSnapshot;

		[SerializeField]
		private AudioClip m_AsphaltSlip;

		[SerializeField]
		private AudioClip m_GroundSlip;

		[Header("Collisions")]
		[SerializeField]
		private float m_CollisionSoundMultiplier = 40f;

		[SerializeField]
		[HideInInspector]
		private List<Layer> m_Layers = new List<Layer>();

		[SerializeField]
		[HideInInspector]
		private List<AudioClip> m_AudioClips = new List<AudioClip>();

		private static Dictionary<int, AudioClip> CollisionsSounds;
	}
}
