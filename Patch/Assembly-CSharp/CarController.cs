using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GameBalance;
using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CarController : MonoBehaviourPunCallbacks, ISetColor
{
	private float MaxSteerAngle
	{
		get
		{
			return this.CarConfig.MaxSteerAngle;
		}
	}

	private DriveType DriveType
	{
		get
		{
			return this.CarConfig.DriveType;
		}
	}

	private bool AutomaticGearBox
	{
		get
		{
			return this.CarConfig.AutomaticGearBox;
		}
	}

	private AnimationCurve MotorTorqueFromRpmCurve
	{
		get
		{
			return this.CarConfig.MotorTorqueFromRpmCurve;
		}
	}

	private float MaxRPM
	{
		get
		{
			return this.CarConfig.MaxRPM;
		}
	}

	private float MinRPM
	{
		get
		{
			return this.CarConfig.MinRPM;
		}
	}

	private float CutOffRPM
	{
		get
		{
			return this.CarConfig.CutOffRPM;
		}
	}

	private float CutOffOffsetRPM
	{
		get
		{
			return this.CarConfig.CutOffOffsetRPM;
		}
	}

	private float RpmToNextGear
	{
		get
		{
			return this.CarConfig.RpmToNextGear;
		}
	}

	private float RpmToPrevGear
	{
		get
		{
			return this.CarConfig.RpmToPrevGear;
		}
	}

	private float MaxForwardSlipToBlockChangeGear
	{
		get
		{
			return this.CarConfig.MaxForwardSlipToBlockChangeGear;
		}
	}

	private float RpmEngineToRpmWheelsLerpSpeed
	{
		get
		{
			return this.CarConfig.RpmEngineToRpmWheelsLerpSpeed;
		}
	}

	private float[] GearsRatio
	{
		get
		{
			return this.CarConfig.GearsRatio;
		}
	}

	private float MainRatio
	{
		get
		{
			return this.CarConfig.MainRatio;
		}
	}

	private float ReversGearRatio
	{
		get
		{
			return this.CarConfig.ReversGearRatio;
		}
	}

	private float MaxBrakeTorque
	{
		get
		{
			return this.CarConfig.MaxBrakeTorque;
		}
	}

	private float TargetSpeedIfBrakingGround
	{
		get
		{
			return this.CarConfig.TargetSpeedIfBrakingGround;
		}
	}

	private float BrakingSpeedOneWheelTime
	{
		get
		{
			return this.CarConfig.BrakingSpeedOneWheelTime;
		}
	}

	public bool EnableSteerAngleMultiplier
	{
		get
		{
			return this.RegimeSettings.EnableSteerAngleMultiplier;
		}
	}

	private float MinSteerAngleMultiplier
	{
		get
		{
			return this.RegimeSettings.MinSteerAngleMultiplier;
		}
	}

	private float MaxSteerAngleMultiplier
	{
		get
		{
			return this.RegimeSettings.MaxSteerAngleMultiplier;
		}
	}

	private float MaxSpeedForMinAngleMultiplier
	{
		get
		{
			return this.RegimeSettings.MaxSpeedForMinAngleMultiplier;
		}
	}

	private float SteerAngleChangeSpeed
	{
		get
		{
			return this.RegimeSettings.SteerAngleChangeSpeed;
		}
	}

	private float MinSpeedForSteerHelp
	{
		get
		{
			return this.RegimeSettings.MinSpeedForSteerHelp;
		}
	}

	private float HelpSteerPower
	{
		get
		{
			return this.RegimeSettings.HelpSteerPower;
		}
	}

	private float OppositeAngularVelocityHelpPower
	{
		get
		{
			return this.RegimeSettings.OppositeAngularVelocityHelpPower;
		}
	}

	private float PositiveAngularVelocityHelpPower
	{
		get
		{
			return this.RegimeSettings.PositiveAngularVelocityHelpPower;
		}
	}

	private float MaxAngularVelocityHelpAngle
	{
		get
		{
			return this.RegimeSettings.MaxAngularVelocityHelpAngle;
		}
	}

	private float AngularVelucityInMaxAngle
	{
		get
		{
			return this.RegimeSettings.AngularVelucityInMaxAngle;
		}
	}

	private float AngularVelucityInMinAngle
	{
		get
		{
			return this.RegimeSettings.AngularVelucityInMinAngle;
		}
	}

	public PositioningCar PositioningCar
	{
		get
		{
			if (this.m_PositioningCar == null)
			{
				this.m_PositioningCar = base.GetComponent<PositioningCar>();
				if (this.m_PositioningCar == null)
				{
					this.m_PositioningCar = base.gameObject.AddComponent<PositioningCar>();
				}
			}
			return this.m_PositioningCar;
		}
	}

	public CarConfig GetCarConfig
	{
		get
		{
			return this.CarConfig;
		}
	}

	public Wheel[] Wheels { get; private set; }

	public Rigidbody RB
	{
		get
		{
			if (!this._RB)
			{
				this._RB = base.GetComponent<Rigidbody>();
			}
			return this._RB;
		}
	}

	private List<SetColorForMaskMaterial> SetColorMeshes
	{
		get
		{
			if (this.m_SetColorMeshes == null)
			{
				this.m_SetColorMeshes = base.gameObject.GetComponentsInChildren<SetColorForMaskMaterial>(true).ToList<SetColorForMaskMaterial>();
			}
			return this.m_SetColorMeshes;
		}
	}

	public bool CarIsVisible
	{
		get
		{
			return this.BaseView.isVisible;
		}
	}

	public float CurrentMaxSlip { get; private set; }

	public int CurrentMaxSlipWheelIndex { get; private set; }

	public float CurrentSpeed { get; private set; }

	public float SpeedInHour
	{
		get
		{
			return this.CurrentSpeed * 3.6f;
		}
	}

	public int CarDirection
	{
		get
		{
			if (this.CurrentSpeed < 1f)
			{
				return 0;
			}
			if (this.VelocityAngle >= 90f || this.VelocityAngle <= -90f)
			{
				return -1;
			}
			return 1;
		}
	}

	private void Awake()
	{
		if (!GameController.InGameScene)
		{
			base.GetComponent<BodyTilt>().enabled = false;
			base.enabled = false;
			return;
		}
		if (this.BaseView == null || !this.BaseView.gameObject.activeInHierarchy)
		{
			this.BaseView = base.gameObject.GetComponentInChildren<Renderer>();
		}
		this.RB.centerOfMass = this.COM.localPosition;
		this.Wheels = new Wheel[]
		{
			this.FrontLeftWheel,
			this.FrontRightWheel,
			this.RearLeftWheel,
			this.RearRightWheel
		};
		Wheel[] wheels = this.Wheels;
		for (int i = 0; i < wheels.Length; i++)
		{
			wheels[i].WheelCollider.ConfigureVehicleSubsteps(40f, 13, 8);
		}
		switch (this.DriveType)
		{
		case DriveType.AWD:
			this.FirstDriveWheel = 0;
			this.LastDriveWheel = 3;
			break;
		case DriveType.FWD:
			this.FirstDriveWheel = 0;
			this.LastDriveWheel = 1;
			break;
		case DriveType.RWD:
			this.FirstDriveWheel = 2;
			this.LastDriveWheel = 3;
			break;
		}
		this.MaxMotorTorque = this.CarConfig.MaxMotorTorque / (float)(this.LastDriveWheel - this.FirstDriveWheel + 1);
		this.AllGearsRatio = new float[this.GearsRatio.Length + 2];
		this.AllGearsRatio[0] = this.ReversGearRatio * this.MainRatio;
		this.AllGearsRatio[1] = 0f;
		for (int j = 0; j < this.GearsRatio.Length; j++)
		{
			this.AllGearsRatio[j + 2] = this.GearsRatio[j] * this.MainRatio;
		}
		this.RegimeSettings = WorldLoading.RegimeSettings;
		this.FrontLeftWheel.UpdateFrictionConfig(this.RegimeSettings.FrontWheelsConfig);
		this.FrontRightWheel.UpdateFrictionConfig(this.RegimeSettings.FrontWheelsConfig);
		this.RearLeftWheel.UpdateFrictionConfig(this.RegimeSettings.RearWheelsConfig);
		this.RearRightWheel.UpdateFrictionConfig(this.RegimeSettings.RearWheelsConfig);
		using (List<ParticleSystem>.Enumerator enumerator = this.BackFireParticles.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				ParticleSystem particles = enumerator.Current;
				this.BackFireAction = (Action)Delegate.Combine(this.BackFireAction, new Action(delegate()
				{
					particles.Emit(2);
				}));
			}
		}
	}

	private void Start()
	{
		this.CollisionAction = (Action<CarController, Collision>)Delegate.Combine(this.CollisionAction, new Action<CarController, Collision>(Singleton<FXController>.Instance.PlayCollisionSound));
	}

	public void UpdateControls(float horizontal, float vertical, bool brake)
	{
		float num = horizontal * this.MaxSteerAngle;
		if (this.EnableSteerAngleMultiplier)
		{
			num *= Mathf.Clamp(1f - this.SpeedInHour / this.MaxSpeedForMinAngleMultiplier, this.MinSteerAngleMultiplier, this.MaxSteerAngleMultiplier);
		}
		this.CurrentSteerAngle = Mathf.MoveTowards(this.CurrentSteerAngle, num, Time.deltaTime * this.SteerAngleChangeSpeed);
		this.CurrentAcceleration = vertical;
		if (this.InHandBrake != brake)
		{
			float forward = brake ? this.RegimeSettings.HandBrakeForwardStiffness : 1f;
			float sideways = brake ? this.RegimeSettings.HandBrakeSidewaysStiffness : 1f;
			this.RearLeftWheel.PG_WheelCollider.UpdateStiffness(forward, sideways);
			this.RearRightWheel.PG_WheelCollider.UpdateStiffness(forward, sideways);
		}
		this.InHandBrake = brake;
	}

	private void Update()
	{
		for (int i = 0; i < this.Wheels.Length; i++)
		{
			this.Wheels[i].UpdateVisual(this.CarIsVisible);
		}
	}

	private void FixedUpdate()
	{
		this.CurrentSpeed = this.RB.velocity.magnitude;
		this.UpdateSteerAngleLogic();
		this.UpdateRpmAndTorqueLogic();
		this.CurrentMaxSlip = this.Wheels[0].CurrentMaxSlip;
		this.CurrentMaxSlipWheelIndex = 0;
		int num = 0;
		if (this.InHandBrake)
		{
			this.RearLeftWheel.WheelCollider.brakeTorque = this.MaxBrakeTorque;
			this.RearRightWheel.WheelCollider.brakeTorque = this.MaxBrakeTorque;
			this.FrontLeftWheel.WheelCollider.brakeTorque = 0f;
			this.FrontRightWheel.WheelCollider.brakeTorque = 0f;
		}
		for (int i = 0; i < this.Wheels.Length; i++)
		{
			if (!this.InHandBrake)
			{
				this.Wheels[i].WheelCollider.brakeTorque = this.CurrentBrake;
			}
			this.Wheels[i].FixedUpdate();
			if (this.CurrentMaxSlip < this.Wheels[i].CurrentMaxSlip)
			{
				this.CurrentMaxSlip = this.Wheels[i].CurrentMaxSlip;
				this.CurrentMaxSlipWheelIndex = i;
			}
			if (this.Wheels[i].WheelCollider.isGrounded && B.LayerSettings.BrakingGroundMask.LayerInMask(this.Wheels[i].GetHit.collider.gameObject.layer))
			{
				num++;
			}
		}
		if (num > 0 && this.SpeedInHour > this.TargetSpeedIfBrakingGround)
		{
			this.RB.velocity = Vector3.MoveTowards(this.RB.velocity, Vector3.zero, (float)num * this.BrakingSpeedOneWheelTime * Time.deltaTime);
		}
	}

	private void OnCollisionEnter(Collision collision)
	{
		this.CollisionAction.SafeInvoke(this, collision);
	}

	private void OnTriggerEnter(Collider trigger)
	{
		if (trigger.gameObject.tag == "ResetCarTrigger")
		{
			this.ResetPosition();
			this.CollisionAction.SafeInvoke(this, null);
		}
	}

	public float VelocityAngle { get; private set; }

	private void UpdateSteerAngleLogic()
	{
		object obj = this.SpeedInHour > this.MinSpeedForSteerHelp && this.CarDirection > 0;
		float num = 0f;
		this.VelocityAngle = -Vector3.SignedAngle(this.RB.velocity, base.transform.TransformDirection(Vector3.forward), Vector3.up);
		object obj2 = obj;
		if (obj2 != null)
		{
			num = Mathf.Clamp(this.VelocityAngle * this.HelpSteerPower, -this.MaxSteerAngle, this.MaxSteerAngle);
		}
		num = Mathf.Clamp(num + this.CurrentSteerAngle, -(this.MaxSteerAngle + 10f), this.MaxSteerAngle + 10f);
		this.Wheels[0].WheelCollider.steerAngle = num;
		this.Wheels[1].WheelCollider.steerAngle = num;
		if (obj2 != null)
		{
			float num2 = Mathf.Abs(this.VelocityAngle) / this.MaxAngularVelocityHelpAngle;
			Vector3 angularVelocity = this.RB.angularVelocity;
			if (this.VelocityAngle * this.CurrentSteerAngle > 0f)
			{
				float num3 = this.OppositeAngularVelocityHelpPower * this.CurrentSteerAngle * Time.fixedDeltaTime;
				angularVelocity.y += num3 * num2;
			}
			else if (!Mathf.Approximately(this.CurrentSteerAngle, 0f))
			{
				float num4 = this.PositiveAngularVelocityHelpPower * this.CurrentSteerAngle * Time.fixedDeltaTime;
				angularVelocity.y += num4 * (1f - num2);
			}
			float num5 = (this.AngularVelucityInMaxAngle - this.AngularVelucityInMinAngle) * num2 + this.AngularVelucityInMinAngle;
			angularVelocity.y = Mathf.Clamp(angularVelocity.y, -num5, num5);
			this.RB.angularVelocity = angularVelocity;
		}
	}

	public int CurrentGear { get; private set; }

	public int CurrentGearIndex
	{
		get
		{
			return this.CurrentGear + 1;
		}
	}

	public float EngineRPM { get; private set; }

	public float GetMaxRPM
	{
		get
		{
			return this.MaxRPM;
		}
	}

	public float GetMinRPM
	{
		get
		{
			return this.MinRPM;
		}
	}

	public float GetInCutOffRPM
	{
		get
		{
			return this.CutOffRPM - this.CutOffOffsetRPM;
		}
	}

	private void UpdateRpmAndTorqueLogic()
	{
		if (this.InCutOff)
		{
			if (this.CutOffTimer > 0f)
			{
				this.CutOffTimer -= Time.fixedDeltaTime;
				this.EngineRPM = Mathf.Lerp(this.EngineRPM, this.GetInCutOffRPM, this.RpmEngineToRpmWheelsLerpSpeed * Time.fixedDeltaTime);
			}
			else
			{
				this.InCutOff = false;
			}
		}
		if (!GameController.RaceIsStarted)
		{
			if (this.InCutOff)
			{
				return;
			}
			float num = (this.CurrentAcceleration > 0f) ? this.MaxRPM : this.MinRPM;
			float num2 = (this.CurrentAcceleration > 0f) ? this.RpmEngineToRpmWheelsLerpSpeed : (this.RpmEngineToRpmWheelsLerpSpeed * 0.2f);
			this.EngineRPM = Mathf.Lerp(this.EngineRPM, num, num2 * Time.fixedDeltaTime);
			if (this.EngineRPM >= this.CutOffRPM)
			{
				this.PlayBackfireWithProbability();
				this.InCutOff = true;
				this.CutOffTimer = this.CarConfig.CutOffTime;
			}
			return;
		}
		else
		{
			float num3 = 0f;
			for (int i = this.FirstDriveWheel + 1; i <= this.LastDriveWheel; i++)
			{
				num3 += this.Wheels[i].WheelCollider.rpm;
			}
			num3 /= (float)(this.LastDriveWheel - this.FirstDriveWheel + 1);
			if (!this.InCutOff)
			{
				float num4 = ((num3 + 20f) * this.AllGearsRatio[this.CurrentGearIndex]).Abs();
				num4 = Mathf.Clamp(num4, this.MinRPM, this.MaxRPM);
				this.EngineRPM = Mathf.Lerp(this.EngineRPM, num4, this.RpmEngineToRpmWheelsLerpSpeed * Time.fixedDeltaTime);
			}
			if (this.EngineRPM >= this.CutOffRPM)
			{
				this.PlayBackfireWithProbability();
				this.InCutOff = true;
				this.CutOffTimer = this.CarConfig.CutOffTime;
				return;
			}
			if (!Mathf.Approximately(this.CurrentAcceleration, 0f))
			{
				if ((float)this.CarDirection * this.CurrentAcceleration >= 0f)
				{
					this.CurrentBrake = 0f;
					float num5 = this.MotorTorqueFromRpmCurve.Evaluate(this.EngineRPM * 0.001f);
					float motorTorque = this.CurrentAcceleration * (num5 * (this.MaxMotorTorque * this.AllGearsRatio[this.CurrentGearIndex]));
					if (Mathf.Abs(num3) * this.AllGearsRatio[this.CurrentGearIndex] > this.MaxRPM)
					{
						motorTorque = 0f;
					}
					float num6 = this.AllGearsRatio[this.CurrentGearIndex] * this.EngineRPM;
					for (int j = this.FirstDriveWheel; j <= this.LastDriveWheel; j++)
					{
						if (this.Wheels[j].WheelCollider.rpm <= num6)
						{
							this.Wheels[j].WheelCollider.motorTorque = motorTorque;
						}
						else
						{
							this.Wheels[j].WheelCollider.motorTorque = 0f;
						}
					}
				}
				else
				{
					this.CurrentBrake = this.MaxBrakeTorque;
					for (int k = this.FirstDriveWheel; k <= this.LastDriveWheel; k++)
					{
						this.Wheels[k].WheelCollider.motorTorque = 0f;
					}
				}
			}
			else
			{
				for (int l = this.FirstDriveWheel; l <= this.LastDriveWheel; l++)
				{
					this.Wheels[l].WheelCollider.motorTorque = 0f;
				}
			}
			if (this.AutomaticGearBox)
			{
				bool flag = false;
				for (int m = this.FirstDriveWheel; m <= this.LastDriveWheel; m++)
				{
					if (this.Wheels[m].CurrentForwardSleep > this.MaxForwardSlipToBlockChangeGear)
					{
						flag = true;
						break;
					}
				}
				float num7 = 0f;
				float num8 = 0f;
				if (!flag && this.EngineRPM > this.RpmToNextGear && this.CurrentGear >= 0 && this.CurrentGear < this.AllGearsRatio.Length - 2)
				{
					num7 = this.AllGearsRatio[this.CurrentGearIndex];
					int currentGear = this.CurrentGear;
					this.CurrentGear = currentGear + 1;
					num8 = this.AllGearsRatio[this.CurrentGearIndex];
				}
				else if (this.EngineRPM < this.RpmToPrevGear && this.CurrentGear > 0 && (this.EngineRPM <= this.MinRPM || this.CurrentGear != 1))
				{
					num7 = this.AllGearsRatio[this.CurrentGearIndex];
					int currentGear = this.CurrentGear;
					this.CurrentGear = currentGear - 1;
					num8 = this.AllGearsRatio[this.CurrentGearIndex];
				}
				if (!Mathf.Approximately(num7, 0f) && !Mathf.Approximately(num8, 0f))
				{
					this.EngineRPM = Mathf.Lerp(this.EngineRPM, this.EngineRPM * (num8 / num7), this.RpmEngineToRpmWheelsLerpSpeed * Time.fixedDeltaTime);
				}
				if (this.CarDirection <= 0 && this.CurrentAcceleration < 0f)
				{
					this.CurrentGear = -1;
					return;
				}
				if (this.CurrentGear <= 0 && this.CarDirection >= 0 && this.CurrentAcceleration > 0f)
				{
					this.CurrentGear = 1;
					return;
				}
				if (this.CarDirection == 0 && this.CurrentAcceleration == 0f)
				{
					this.CurrentGear = 0;
				}
			}
			return;
		}
	}

	private void PlayBackfireWithProbability()
	{
		this.PlayBackfireWithProbability(this.GetCarConfig.ProbabilityBackfire);
	}

	private void PlayBackfireWithProbability(float probability)
	{
		if (Random.Range(0f, 1f) <= probability)
		{
			this.BackFireAction.SafeInvoke();
		}
	}

	public void SetColor(CarColorPreset color)
	{
		foreach (SetColorForMaskMaterial setColorForMaskMaterial in this.SetColorMeshes)
		{
			setColorForMaskMaterial.SetColor(color);
		}
	}

	public void ResetPosition()
	{
		if (this.ResetPositionCoroutine != null)
		{
			base.StopCoroutine(this.ResetPositionCoroutine);
		}
		this.ResetPositionCoroutine = base.StartCoroutine(this.DoResetPosition());
	}

	private IEnumerator DoResetPosition()
	{
		if (this.PositioningCar.LastCorrectProgressDistance == 0f || Time.time - this.LastResetTime < 5f)
		{
			yield break;
		}
		this.LastResetTime = Time.time;
		for (int i = 0; i < this.Wheels.Length; i++)
		{
			this.Wheels[i].StopEmitFX = true;
			this.Wheels[i].UpdateVisual(this.CarIsVisible);
		}
		this.CurrentGear = 0;
		this.EngineRPM = this.MinRPM;
		this.RB.velocity = Vector3.zero;
		this.RB.angularVelocity = Vector3.zero;
		base.transform.position = this.PositioningCar.LastCorrectPosition.position;
		base.transform.rotation = Quaternion.LookRotation(this.PositioningCar.LastCorrectPosition.direction, Vector3.up);
		this.ResetCarAction.SafeInvoke();
		yield return new WaitForFixedUpdate();
		for (int j = 0; j < this.Wheels.Length; j++)
		{
			this.Wheels[j].StopEmitFX = false;
		}
		this.ResetPositionCoroutine = null;
		yield break;
	}

	private void OnDrawGizmosSelected()
	{
		Vector3 position = base.transform.position;
		Vector3 vector = base.transform.position + Vector3.ClampMagnitude(this.RB.velocity, 4f);
		Vector3 vector2 = base.transform.TransformPoint(Vector3.forward * 4f);
		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere(position, 0.2f);
		Gizmos.DrawLine(position, vector);
		Gizmos.DrawLine(position, vector2);
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(vector2, 0.2f);
		Gizmos.color = Color.blue;
		Gizmos.DrawWireSphere(vector, 0.2f);
		Gizmos.color = Color.white;
	}

	[SerializeField]
	private Wheel FrontLeftWheel;

	[SerializeField]
	private Wheel FrontRightWheel;

	[SerializeField]
	private Wheel RearLeftWheel;

	[SerializeField]
	private Wheel RearRightWheel;

	[SerializeField]
	private Transform COM;

	[SerializeField]
	private List<ParticleSystem> BackFireParticles = new List<ParticleSystem>();

	[SerializeField]
	private Renderer BaseView;

	[SerializeField]
	private CarConfig CarConfig;

	private float MaxMotorTorque;

	private RegimeSettings RegimeSettings;

	private PositioningCar m_PositioningCar;

	public Action<CarController, Collision> CollisionAction;

	public Action ResetCarAction;

	public Action BackFireAction;

	private float[] AllGearsRatio;

	private Rigidbody _RB;

	private List<SetColorForMaskMaterial> m_SetColorMeshes;

	private float CurrentSteerAngle;

	private float CurrentAcceleration;

	private float CurrentBrake;

	private bool InHandBrake;

	private int FirstDriveWheel;

	private int LastDriveWheel;

	private float CutOffTimer;

	private bool InCutOff;

	private float LastResetTime;

	private Coroutine ResetPositionCoroutine;
}
