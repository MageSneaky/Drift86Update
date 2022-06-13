using System;
using UnityEngine;

namespace PG_Physics.Wheel
{
	[RequireComponent(typeof(WheelCollider))]
	public class PG_WheelCollider : MonoBehaviour
	{
		public WheelCollider WheelCollider
		{
			get
			{
				if (this.m_WheelCollider == null)
				{
					this.m_WheelCollider = base.GetComponent<WheelCollider>();
				}
				return this.m_WheelCollider;
			}
		}

		public Rigidbody RB
		{
			get
			{
				if (this.m_RB == null)
				{
					this.m_RB = this.WheelCollider.attachedRigidbody;
				}
				return this.m_RB;
			}
		}

		public void UpdateStiffness(float forward, float sideways)
		{
			WheelFrictionCurve forwardFriction = this.WheelCollider.forwardFriction;
			WheelFrictionCurve sidewaysFriction = this.WheelCollider.sidewaysFriction;
			forwardFriction.stiffness = forward;
			sidewaysFriction.stiffness = sideways;
			this.WheelCollider.forwardFriction = forwardFriction;
			this.WheelCollider.sidewaysFriction = sidewaysFriction;
		}

		public void UpdateConfig()
		{
			this.UpdateConfig(this.WheelConfig);
		}

		public void UpdateConfig(PG_WheelColliderConfig newConfig)
		{
			if (this.RB == null)
			{
				Debug.LogError("WheelCollider without attached RigidBody");
				return;
			}
			this.WheelConfig.ForwardFriction = newConfig.ForwardFriction;
			this.WheelConfig.SidewaysFriction = newConfig.SidewaysFriction;
			if (newConfig.IsFullConfig)
			{
				float spring = Mathf.Lerp(0f, 60000f, newConfig.Spring);
				float damper = Mathf.Lerp(0f, 10000f, newConfig.Damper);
				JointSpring suspensionSpring = default(JointSpring);
				suspensionSpring.spring = spring;
				suspensionSpring.damper = damper;
				suspensionSpring.targetPosition = newConfig.TargetPoint;
				this.WheelCollider.mass = newConfig.Mass;
				this.WheelCollider.radius = newConfig.Radius;
				this.WheelCollider.wheelDampingRate = newConfig.WheelDampingRate;
				this.WheelCollider.suspensionDistance = newConfig.SuspensionDistance;
				this.WheelCollider.forceAppPointDistance = newConfig.ForceAppPointDistance;
				this.WheelCollider.center = newConfig.Center;
				this.WheelCollider.suspensionSpring = suspensionSpring;
			}
			WheelFrictionCurve forwardFriction = default(WheelFrictionCurve);
			forwardFriction.extremumSlip = Mathf.Lerp(0.4f, 0.4f, newConfig.ForwardFriction);
			forwardFriction.extremumValue = Mathf.Lerp(0.7f, 4.5f, newConfig.ForwardFriction);
			forwardFriction.asymptoteSlip = Mathf.Lerp(0.6f, 0.6f, newConfig.ForwardFriction);
			forwardFriction.asymptoteValue = Mathf.Lerp(0.65f, 4f, newConfig.ForwardFriction);
			forwardFriction.stiffness = 1f;
			WheelFrictionCurve sidewaysFriction = default(WheelFrictionCurve);
			sidewaysFriction.extremumSlip = Mathf.Lerp(0.4f, 0.4f, newConfig.SidewaysFriction);
			sidewaysFriction.extremumValue = Mathf.Lerp(0.7f, 4.5f, newConfig.SidewaysFriction);
			sidewaysFriction.asymptoteSlip = Mathf.Lerp(0.6f, 0.6f, newConfig.SidewaysFriction);
			sidewaysFriction.asymptoteValue = Mathf.Lerp(0.65f, 4f, newConfig.SidewaysFriction);
			sidewaysFriction.stiffness = 1f;
			this.WheelCollider.forwardFriction = forwardFriction;
			this.WheelCollider.sidewaysFriction = sidewaysFriction;
		}

		public bool CheckFirstEnable()
		{
			if (this.m_WheelCollider != null)
			{
				return false;
			}
			float spring = (this.WheelCollider.suspensionSpring.spring - 0f) / 60000f;
			float damper = (this.WheelCollider.suspensionSpring.damper - 0f) / 10000f;
			float forwardFriction = (this.WheelCollider.forwardFriction.extremumValue - 0.7f) / 3.8f;
			float sidewaysFriction = (this.WheelCollider.sidewaysFriction.extremumValue - 0.7f) / 3.8f;
			this.WheelConfig = default(PG_WheelColliderConfig);
			this.WheelConfig.Mass = this.WheelCollider.mass;
			this.WheelConfig.Radius = this.WheelCollider.radius;
			this.WheelConfig.WheelDampingRate = this.WheelCollider.wheelDampingRate;
			this.WheelConfig.SuspensionDistance = this.WheelCollider.suspensionDistance;
			this.WheelConfig.ForceAppPointDistance = this.WheelCollider.forceAppPointDistance;
			this.WheelConfig.Center = this.WheelCollider.center;
			this.WheelConfig.TargetPoint = this.WheelCollider.suspensionSpring.targetPosition;
			this.WheelConfig.Spring = spring;
			this.WheelConfig.Damper = damper;
			this.WheelConfig.ForwardFriction = forwardFriction;
			this.WheelConfig.SidewaysFriction = sidewaysFriction;
			return true;
		}

		[SerializeField]
		[FullField]
		private PG_WheelColliderConfig WheelConfig;

		[SerializeField]
		[HideInInspector]
		private WheelCollider m_WheelCollider;

		[SerializeField]
		[HideInInspector]
		private Rigidbody m_RB;

		private const float minSpring = 0f;

		private const float maxSpring = 60000f;

		private const float minDamper = 0f;

		private const float maxDamper = 10000f;

		private const float minExtremumSlip = 0.4f;

		private const float minExtremumValue = 0.7f;

		private const float minAsymptoteSlip = 0.6f;

		private const float minAsymptoteValue = 0.65f;

		private const float maxExtremumSlip = 0.4f;

		private const float maxExtremumValue = 4.5f;

		private const float maxAsymptoteSlip = 0.6f;

		private const float maxAsymptoteValue = 4f;
	}
}
