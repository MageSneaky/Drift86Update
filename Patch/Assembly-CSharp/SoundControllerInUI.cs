using System;
using System.Collections.Generic;
using UnityEngine;

public class SoundControllerInUI : MonoBehaviour
{
	private void Awake()
	{
		if (SoundControllerInUI.Instance == null)
		{
			SoundControllerInUI.Instance = this;
			UnityEngine.Object.DontDestroyOnLoad(this);
			return;
		}
		UnityEngine.Object.Destroy(base.gameObject);
	}

	private void Play(AudioClip clip)
	{
		AudioSource audioSource = null;
		foreach (AudioSource audioSource2 in this.SourcePool)
		{
			if (audioSource2.isPlaying)
			{
				audioSource = audioSource2;
				break;
			}
		}
		if (audioSource == null)
		{
			audioSource = UnityEngine.Object.Instantiate<AudioSource>(this.SourceRef, base.transform);
			audioSource.priority = 8 * this.SourcePool.Count;
			this.SourcePool.Add(audioSource);
		}
		audioSource.clip = clip;
		audioSource.Play();
	}

	public static SoundControllerInUI Instance { get; private set; }

	public static void PlayAudioClip(AudioClip clip)
	{
		if (clip == null)
		{
			return;
		}
		SoundControllerInUI.CheckController();
		SoundControllerInUI.Instance.Play(clip);
	}

	private static void CheckController()
	{
		if (SoundControllerInUI.Instance == null)
		{
			UnityEngine.Object.Instantiate<SoundControllerInUI>(B.ResourcesSettings.SoundControllerInUI);
		}
	}

	[SerializeField]
	private AudioSource SourceRef;

	private HashSet<AudioSource> SourcePool = new HashSet<AudioSource>();
}
