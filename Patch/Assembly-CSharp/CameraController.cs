using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : Singleton<CameraController>
{
	public CameraController.CameraPreset ActivePreset { get; private set; }

	private CarController TargetCar
	{
		get
		{
			return GameController.PlayerCar;
		}
	}

	private GameController GameController
	{
		get
		{
			return GameController.Instance;
		}
	}

	private Vector3 TargetPoint
	{
		get
		{
			if (this.CurrentFrame != Time.frameCount)
			{
				if (this.GameController == null || this.TargetCar == null)
				{
					return base.transform.position;
				}
				this.m_TargetPoint = this.TargetCar.RB.velocity * this.ActivePreset.VelocityMultiplier;
				this.m_TargetPoint += this.TargetCar.transform.position;
				this.m_TargetPoint.y = 0f;
				this.CurrentFrame = Time.frameCount;
			}
			return this.m_TargetPoint;
		}
	}

	protected override void AwakeSingleton()
	{
		this.CamerasPreset.ForEach(delegate(CameraController.CameraPreset c)
		{
			c.CameraHolder.SetActive(false);
		});
		this.ActivePresetIndex = GameOptions.ActiveCameraIndex;
		this.UpdateActiveCamera();
	}

	private IEnumerator Start()
	{
		while (this.GameController == null || this.TargetCar == null)
		{
			yield return null;
		}
		base.transform.position = this.TargetPoint;
		this.ActivePreset.CameraHolder.rotation = this.TargetCar.transform.rotation;
		yield break;
	}

	private void Update()
	{
		if (this.ActivePreset.EnableRotation && (this.TargetPoint - base.transform.position).sqrMagnitude >= this.SqrMinDistance)
		{
			Quaternion b = Quaternion.LookRotation(this.TargetPoint - base.transform.position, Vector3.up);
			this.ActivePreset.CameraHolder.rotation = Quaternion.Lerp(this.ActivePreset.CameraHolder.rotation, b, Time.deltaTime * this.ActivePreset.SetRotationSpeed);
		}
		base.transform.position = Vector3.LerpUnclamped(base.transform.position, this.TargetPoint, Time.deltaTime * this.ActivePreset.SetPositionSpeed);
		if (Input.GetKeyDown(KeyCode.C) || Input.GetKeyDown(KeyCode.Joystick1Button2))
		{
			this.SetNextCamera();
		}
	}

	public void SetNextCamera()
	{
		this.ActivePresetIndex = MathExtentions.LoopClamp(this.ActivePresetIndex + 1, 0, this.CamerasPreset.Count);
		GameOptions.ActiveCameraIndex = this.ActivePresetIndex;
		this.UpdateActiveCamera();
	}

	public void UpdateActiveCamera()
	{
		if (this.ActivePreset != null)
		{
			this.ActivePreset.CameraHolder.SetActive(false);
		}
		this.ActivePreset = this.CamerasPreset[this.ActivePresetIndex];
		this.ActivePreset.CameraHolder.SetActive(true);
		this.SqrMinDistance = this.ActivePreset.MinDistanceForRotation * 2f;
		if (this.ActivePreset.EnableRotation && (this.TargetPoint - base.transform.position).sqrMagnitude >= this.SqrMinDistance)
		{
			Quaternion rotation = Quaternion.LookRotation(this.TargetPoint - base.transform.position, Vector3.up);
			this.ActivePreset.CameraHolder.rotation = rotation;
		}
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(this.TargetPoint, 1f);
		Gizmos.color = Color.white;
	}

	[SerializeField]
	private List<CameraController.CameraPreset> CamerasPreset = new List<CameraController.CameraPreset>();

	private int ActivePresetIndex = -1;

	private float SqrMinDistance;

	private int CurrentFrame;

	private Vector3 m_TargetPoint;

	[Serializable]
	public class CameraPreset
	{
		public Transform CameraHolder;

		public float SetPositionSpeed = 1f;

		public float VelocityMultiplier;

		public bool EnableRotation;

		public float MinDistanceForRotation = 0.1f;

		public float SetRotationSpeed = 1f;
	}
}
