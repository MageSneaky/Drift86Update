using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FXController : Singleton<FXController>
{
	protected override void AwakeSingleton()
	{
		this.TrailRef.gameObject.SetActive(false);
		this.AudioSourceRef.gameObject.SetActive(false);
	}

	public ParticleSystem GetAspahaltParticles
	{
		get
		{
			return this.AsphaltSmokeParticles;
		}
	}

	public ParticleSystem GetGroundParticles
	{
		get
		{
			return this.GroundSmokeParticles;
		}
	}

	public ParticleSystem GetParticles(int layer)
	{
		if (this.GroundMask.LayerInMask(layer))
		{
			return this.GroundSmokeParticles;
		}
		return this.AsphaltSmokeParticles;
	}

	public TrailRenderer GetTrail(Vector3 startPos)
	{
		TrailRenderer trailRenderer;
		if (this.FreeTrails.Count > 0)
		{
			trailRenderer = this.FreeTrails.Dequeue();
		}
		else
		{
			trailRenderer = Object.Instantiate<TrailRenderer>(this.TrailRef, this.TrailsHolder);
		}
		trailRenderer.transform.position = startPos;
		trailRenderer.gameObject.SetActive(true);
		return trailRenderer;
	}

	public void SetFreeTrail(TrailRenderer trail)
	{
		base.StartCoroutine(this.WaitVisibleTrail(trail));
	}

	private IEnumerator WaitVisibleTrail(TrailRenderer trail)
	{
		trail.transform.SetParent(this.TrailsHolder);
		yield return new WaitForSeconds(trail.time);
		trail.Clear();
		trail.gameObject.SetActive(false);
		this.FreeTrails.Enqueue(trail);
		yield break;
	}

	public void PlayCollisionSound(CarController car, Collision collision)
	{
		if (!car.CarIsVisible || collision == null)
		{
			return;
		}
		int layer = collision.gameObject.layer;
		if (this.InPlayingCollissionCars.Contains(car))
		{
			return;
		}
		AudioSource audioSource;
		if (this.FreeAudioSources.Count > 0)
		{
			audioSource = this.FreeAudioSources.Dequeue();
		}
		else
		{
			if (this.SourceCount >= this.MaxAudioSources)
			{
				Debug.LogWarning("No free SoundSources");
				return;
			}
			audioSource = Object.Instantiate<AudioSource>(this.AudioSourceRef, this.SoundsHolder);
			audioSource.gameObject.SetActive(true);
			this.SourceCount++;
		}
		audioSource.transform.position = collision.contacts[0].point;
		audioSource.clip = B.SoundSettings.GetAudioClipCollision(layer);
		float volume = Mathf.Clamp01(collision.relativeVelocity.magnitude / B.SoundSettings.CollisionSoundMultiplier);
		audioSource.volume = volume;
		base.StartCoroutine(this.PlaySoundCoroutine(audioSource));
		this.InPlayingCollissionCars.Add(car);
		base.StartCoroutine(this.RemoveLayerFromPlaying(car));
	}

	private IEnumerator RemoveLayerFromPlaying(CarController car)
	{
		yield return new WaitForSeconds(this.MinTimeBetweenSounds);
		this.InPlayingCollissionCars.Remove(car);
		yield break;
	}

	private IEnumerator PlaySoundCoroutine(AudioSource source)
	{
		if (source.clip != null)
		{
			source.Play();
			yield return new WaitForSeconds(source.clip.length);
		}
		this.FreeAudioSources.Enqueue(source);
		yield break;
	}

	[Header("Particles settings")]
	[SerializeField]
	private ParticleSystem AsphaltSmokeParticles;

	[SerializeField]
	private ParticleSystem GroundSmokeParticles;

	[SerializeField]
	private ParticleSystem SparkParticles;

	[SerializeField]
	private LayerMask TrackMask;

	[SerializeField]
	private LayerMask GroundMask;

	[Header("Trail settings")]
	[SerializeField]
	private TrailRenderer TrailRef;

	[SerializeField]
	private Transform TrailsHolder;

	[Header("Sound settings")]
	[SerializeField]
	private AudioSource AudioSourceRef;

	[SerializeField]
	private Transform SoundsHolder;

	[SerializeField]
	private int MaxAudioSources = 10;

	[SerializeField]
	private float MinTimeBetweenSounds = 1f;

	private Queue<TrailRenderer> FreeTrails = new Queue<TrailRenderer>();

	private HashSet<CarController> InPlayingCollissionCars = new HashSet<CarController>();

	private Queue<AudioSource> FreeAudioSources = new Queue<AudioSource>();

	private int SourceCount;
}
