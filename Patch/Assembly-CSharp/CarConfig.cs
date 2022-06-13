using System;
using UnityEngine;

[Serializable]
public class CarConfig
{
	[Header("Steer Settings")]
	public float MaxSteerAngle = 25f;

	[Header("Engine and power settings")]
	public DriveType DriveType = DriveType.RWD;

	public bool AutomaticGearBox = true;

	public float MaxMotorTorque = 150f;

	public AnimationCurve MotorTorqueFromRpmCurve;

	public float MaxRPM = 7000f;

	public float MinRPM = 700f;

	public float CutOffRPM = 6800f;

	public float CutOffOffsetRPM = 500f;

	public float CutOffTime = 0.1f;

	[Range(0f, 1f)]
	public float ProbabilityBackfire = 0.2f;

	public float RpmToNextGear = 6500f;

	public float RpmToPrevGear = 4500f;

	public float MaxForwardSlipToBlockChangeGear = 0.5f;

	public float RpmEngineToRpmWheelsLerpSpeed = 15f;

	public float[] GearsRatio;

	public float MainRatio;

	public float ReversGearRatio;

	[Header("Braking settings")]
	public float MaxBrakeTorque = 1000f;

	public float TargetSpeedIfBrakingGround = 20f;

	public float BrakingSpeedOneWheelTime = 2f;
}
