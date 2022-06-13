using System;
using UnityEngine;

namespace PG_Physics.Wheel
{
	[Serializable]
	public struct PG_WheelColliderConfig
	{
		[SerializeField]
		private bool IsFoldout;

		public bool IsFullConfig;

		public float Mass;

		public float Radius;

		public float WheelDampingRate;

		public float SuspensionDistance;

		public float ForceAppPointDistance;

		public Vector3 Center;

		public float Spring;

		public float Damper;

		public float TargetPoint;

		public float ForwardFriction;

		public float SidewaysFriction;
	}
}
