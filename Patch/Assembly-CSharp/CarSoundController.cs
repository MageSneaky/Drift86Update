using System;
using UnityEngine;

[RequireComponent(typeof(CarController))]
public class CarSoundController : MonoBehaviour
{
	private float MaxRPM
	{
		get
		{
			return this.CarController.GetMaxRPM;
		}
	}

	private float EngineRPM
	{
		get
		{
			return this.CarController.EngineRPM;
		}
	}

	private void Awake()
	{
		if (!GameController.InGameScene)
		{
			this.EngineSource.Stop();
			this.SlipSource.Stop();
			base.enabled = false;
			return;
		}
		this.CarController = base.GetComponent<CarController>();
		CarController carController = this.CarController;
		carController.BackFireAction = (Action)Delegate.Combine(carController.BackFireAction, new Action(this.PlayBackfire));
	}

	private void Update()
	{
		this.EngineSource.pitch = this.EngineRPM / this.MaxRPM + this.PitchOffset;
		if (this.CarController.CurrentMaxSlip > this.MinSlipSound)
		{
			if (B.LayerSettings.BrakingGroundMask.LayerInMask(this.CarController.Wheels[this.CarController.CurrentMaxSlipWheelIndex].GetHit.collider.gameObject.layer))
			{
				this.SlipSource.clip = B.SoundSettings.GroundSlip;
			}
			else
			{
				this.SlipSource.clip = B.SoundSettings.AsphaltSlip;
			}
			if (!this.SlipSource.isPlaying)
			{
				this.SlipSource.Play();
			}
			float num = this.CarController.CurrentMaxSlip / this.MaxSlipForSound;
			this.SlipSource.volume = num * 0.5f;
			this.SlipSource.pitch = Mathf.Clamp(num, 0.75f, 1f);
			return;
		}
		this.SlipSource.Stop();
	}

	private void PlayBackfire()
	{
		this.EngineSource.PlayOneShot(this.EngineBackFireClip);
	}

	[Header("Engine sounds")]
	[SerializeField]
	private AudioClip EngineIdleClip;

	[SerializeField]
	private AudioClip EngineBackFireClip;

	[SerializeField]
	private float PitchOffset = 0.5f;

	[SerializeField]
	private AudioSource EngineSource;

	[Header("Slip sounds")]
	[SerializeField]
	private AudioSource SlipSource;

	[SerializeField]
	private float MinSlipSound = 0.15f;

	[SerializeField]
	private float MaxSlipForSound = 1f;

	private CarController CarController;
}
