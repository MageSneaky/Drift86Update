using System;
using GameBalance;
using UnityEngine;

public class CarStatisticsDriftRegime : CarStatistic
{
	public float TotalScore { get; private set; }

	public float BestScore { get; private set; }

	public float CurrentScore { get; private set; }

	public int CurrentMultiplier { get; private set; }

	public bool InDrift { get; private set; }

	public float DriftTimeProcent { get; private set; }

	public float MultiplierProcent
	{
		get
		{
			return this.CurrentMultiplierScore / this.Settings.MinScoreForIncMultiplier;
		}
	}

	public bool NeedCalcelateDriftPoint
	{
		get
		{
			return !base.IsWrongDirection && base.PositioningCar.LastPointIsCorrect;
		}
	}

	private DriftRegimeSettings Settings
	{
		get
		{
			return B.GameSettings.DriftRegimeSettings;
		}
	}

	private float AbsCarVelocityAngle
	{
		get
		{
			return Mathf.Abs(base.Car.VelocityAngle);
		}
	}

	public CarStatisticsDriftRegime(CarController car, string playerName, bool isLocalCar = true) : base(car, playerName, isLocalCar)
	{
		car.ResetCarAction = (Action)Delegate.Combine(car.ResetCarAction, new Action(this.ResetCurrentStatistics));
		this.CurrentMultiplier = 1;
		this.InDrift = false;
	}

	public override void FixedUpdate()
	{
		if (!base.RaceIsStarted || base.IsFinished || GameController.RaceIsEnded)
		{
			return;
		}
		base.FixedUpdate();
		if (!base.IsLocalCar || base.PositioningCar.LastCorrectProgressDistance == 0f)
		{
			return;
		}
		if (this.InDrift)
		{
			this.InDriftUpdate();
			return;
		}
		if (!this.NeedCalcelateDriftPoint || this.AbsCarVelocityAngle <= this.Settings.MinAngle || base.Car.SpeedInHour <= this.Settings.MinSpeed)
		{
			this.Timer = 0f;
			this.DriftTimeProcent = 0f;
			this.CurrentMultiplierScore = 0f;
			return;
		}
		if (this.Timer > this.Settings.WaitDriftTime)
		{
			this.Timer = 0f;
			this.InDrift = true;
			this.DriftDirection = ((base.Car.VelocityAngle < 0f) ? -1 : 1);
			return;
		}
		this.Timer += Time.fixedDeltaTime;
		this.DriftTimeProcent = this.Timer / this.Settings.WaitDriftTime;
	}

	private void InDriftUpdate()
	{
		if (!this.NeedCalcelateDriftPoint || this.AbsCarVelocityAngle < this.Settings.MinAngle || this.AbsCarVelocityAngle > this.Settings.MaxAngle || base.Car.SpeedInHour < this.Settings.MinSpeed)
		{
			if (this.Timer > this.Settings.WaitEndDriftTime)
			{
				this.TotalScore += this.CurrentScore;
				this.TotalScoreChanged.SafeInvoke();
				if (this.CurrentScore > this.BestScore)
				{
					this.BestScore = this.CurrentScore;
				}
				this.ResetCurrentStatistics();
			}
			else
			{
				this.Timer += Time.fixedDeltaTime;
				this.DriftTimeProcent = 1f - this.Timer / this.Settings.WaitEndDriftTime;
			}
			this.CurrentMultiplierScore = 0f;
			return;
		}
		this.Timer = 0f;
		this.DriftTimeProcent = 1f;
		float num = this.AbsCarVelocityAngle / this.Settings.MaxAngle * (base.Car.CurrentSpeed * Time.fixedDeltaTime) * this.Settings.ScorePerMeter;
		this.CurrentScore += num * (float)this.CurrentMultiplier;
		this.CurrentMultiplierScore += num;
		if (this.CurrentMultiplierScore > this.Settings.MinScoreForIncMultiplier)
		{
			this.CurrentMultiplier = Mathf.Clamp(this.CurrentMultiplier + 1, 1, this.Settings.MaxMultiplier);
			this.CurrentMultiplierScore = 0f;
		}
		int num2 = (base.Car.VelocityAngle < 0f) ? -1 : 1;
		if (this.DriftDirection != num2)
		{
			this.DriftDirection = num2;
			this.CurrentMultiplierScore = 0f;
		}
	}

	public void UpdateScore(float totalScore)
	{
		if (totalScore != this.TotalScore)
		{
			this.TotalScore = totalScore;
			this.TotalScoreChanged.SafeInvoke();
		}
	}

	protected override void OnFinishRace()
	{
		if (this.InDrift)
		{
			this.TotalScore += this.CurrentScore;
			this.TotalScoreChanged.SafeInvoke();
			if (this.CurrentScore > this.BestScore)
			{
				this.BestScore = this.CurrentScore;
			}
			this.ResetCurrentStatistics();
		}
	}

	protected override void OnForceFinish()
	{
		float progressDistance = base.PositioningCar.ProgressDistance;
		float num = PositioningSystem.PositioningAndAiPath.Length * (float)PositioningSystem.LapsCount;
		float num2 = 1f - progressDistance / num;
		this.TotalScore += this.TotalScore * num2;
		this.TotalScoreChanged.SafeInvoke();
	}

	protected override void CollisionCar(CarController car, Collision collision)
	{
		if (collision == null || collision.gameObject.tag == "ResetDriftCollision")
		{
			this.ResetCurrentStatistics();
		}
	}

	private void ResetCurrentStatistics()
	{
		this.Timer = 0f;
		this.InDrift = false;
		this.CurrentMultiplier = 1;
		this.CurrentScore = 0f;
		this.CurrentMultiplierScore = 0f;
		this.DriftTimeProcent = 0f;
	}

	public Action TotalScoreChanged;

	private int DriftDirection;

	private float CurrentMultiplierScore;

	private float Timer;
}
