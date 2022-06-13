using System;
using PG_Physics.Wheel;
using UnityEngine;

[Serializable]
public struct Wheel
{
	public float CurrentMaxSlip
	{
		get
		{
			return Mathf.Max(this.CurrentForwardSleep, this.CurrentSidewaysSleep);
		}
	}

	public float CurrentForwardSleep { get; private set; }

	public float CurrentSidewaysSleep { get; private set; }

	public WheelHit GetHit
	{
		get
		{
			return this.Hit;
		}
	}

	public bool StopEmitFX { get; set; }

	public PG_WheelCollider PG_WheelCollider
	{
		get
		{
			if (this.m_PGWC == null)
			{
				this.m_PGWC = this.WheelCollider.GetComponent<PG_WheelCollider>();
			}
			if (this.m_PGWC == null)
			{
				this.m_PGWC = this.WheelCollider.gameObject.AddComponent<PG_WheelCollider>();
				this.m_PGWC.CheckFirstEnable();
			}
			return this.m_PGWC;
		}
	}

	private FXController FXController
	{
		get
		{
			return Singleton<FXController>.Instance;
		}
	}

	public void FixedUpdate()
	{
		if (this.WheelCollider.GetGroundHit(ref this.Hit))
		{
			float currentForwardSleep = this.CurrentForwardSleep;
			float currentSidewaysSleep = this.CurrentSidewaysSleep;
			this.CurrentForwardSleep = (currentForwardSleep + Mathf.Abs(this.Hit.forwardSlip)) / 2f;
			this.CurrentSidewaysSleep = (currentSidewaysSleep + Mathf.Abs(this.Hit.sidewaysSlip)) / 2f;
			return;
		}
		this.CurrentForwardSleep = 0f;
		this.CurrentSidewaysSleep = 0f;
	}

	public void UpdateVisual(bool carIsVisible)
	{
		if (carIsVisible)
		{
			this.UpdateTransform();
		}
		if (!this.StopEmitFX && this.WheelCollider.isGrounded && this.CurrentMaxSlip > this.SlipForGenerateParticle)
		{
			if (carIsVisible)
			{
				ParticleSystem particles = this.FXController.GetParticles(this.Hit.collider.gameObject.layer);
				Vector3 position = this.WheelCollider.transform.position;
				position.y = this.Hit.point.y;
				particles.transform.position = position;
				particles.Emit(1);
			}
			if (this.Trail == null)
			{
				this.HitPoint = this.WheelCollider.transform.position;
				this.HitPoint.y = this.Hit.point.y;
				this.Trail = this.FXController.GetTrail(this.HitPoint);
				this.Trail.transform.SetParent(this.WheelCollider.transform);
				this.Trail.transform.localPosition += this.TrailOffset;
				return;
			}
		}
		else if (this.Trail != null)
		{
			this.FXController.SetFreeTrail(this.Trail);
			this.Trail = null;
		}
	}

	public void UpdateTransform()
	{
		Vector3 position;
		Quaternion rotation;
		this.WheelCollider.GetWorldPose(ref position, ref rotation);
		this.WheelView.position = position;
		this.WheelView.rotation = rotation;
	}

	public void UpdateFrictionConfig(PG_WheelColliderConfig config)
	{
		this.PG_WheelCollider.UpdateConfig(config);
	}

	public WheelCollider WheelCollider;

	public Transform WheelView;

	public float SlipForGenerateParticle;

	public Vector3 TrailOffset;

	private WheelHit Hit;

	private TrailRenderer Trail;

	private PG_WheelCollider m_PGWC;

	private Vector3 HitPoint;

	private const int SmoothValuesCount = 3;
}
