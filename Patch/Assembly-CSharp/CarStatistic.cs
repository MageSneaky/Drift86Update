using System;
using UnityEngine;

public class CarStatistic
{
	public string PlayerName { get; private set; }

	public bool IsWrongDirection
	{
		get
		{
			return this.PositioningCar.IsWrongDirection;
		}
	}

	public int CurrentLap
	{
		get
		{
			return Mathf.Clamp(this.PositioningCar.CurrentLap, 0, int.MaxValue);
		}
	}

	public int LapsCount
	{
		get
		{
			return PositioningSystem.LapsCount;
		}
	}

	public bool IsFinished
	{
		get
		{
			return this.PositioningCar.IsFinished;
		}
	}

	public float BestLapTime { get; private set; }

	public float CurrentLapTime { get; private set; }

	public float TotalRaceTime { get; private set; }

	public CarController Car { get; private set; }

	public PositioningCar PositioningCar { get; private set; }

	public bool IsLocalCar { get; private set; }

	protected bool RaceIsStarted
	{
		get
		{
			return GameController.RaceIsStarted;
		}
	}

	public CarStatistic(CarController car, string playerName, bool isLocalCar = true)
	{
		this.PlayerName = playerName;
		this.Car = car;
		this.PositioningCar = this.Car.GetComponent<PositioningCar>();
		CarController car2 = this.Car;
		car2.CollisionAction = (Action<CarController, Collision>)Delegate.Combine(car2.CollisionAction, new Action<CarController, Collision>(this.CollisionCar));
		PositioningCar positioningCar = this.PositioningCar;
		positioningCar.OnFinishLapAction = (Action<int>)Delegate.Combine(positioningCar.OnFinishLapAction, new Action<int>(this.OnFinishLap));
		PositioningCar positioningCar2 = this.PositioningCar;
		positioningCar2.OnFinishRaceAction = (Action)Delegate.Combine(positioningCar2.OnFinishRaceAction, new Action(this.OnFinishRace));
		PositioningCar positioningCar3 = this.PositioningCar;
		positioningCar3.OnForceFinishRaceAction = (Action)Delegate.Combine(positioningCar3.OnForceFinishRaceAction, new Action(this.OnForceFinish));
		GameController instance = GameController.Instance;
		instance.FixedUpdateAction = (Action)Delegate.Combine(instance.FixedUpdateAction, new Action(this.FixedUpdate));
		GameController instance2 = GameController.Instance;
		instance2.OnStartRaceAction = (Action)Delegate.Combine(instance2.OnStartRaceAction, new Action(this.OnStartRace));
		this.IsLocalCar = isLocalCar;
	}

	protected virtual void CollisionCar(CarController car, Collision collision)
	{
	}

	public virtual void FixedUpdate()
	{
		if (!this.RaceIsStarted || this.IsFinished || this.StopUpdateTime)
		{
			return;
		}
		this.CurrentLapTime = Time.time - this.StartCurrentLapTime;
		if (this.PositioningCar.IsLocalCar)
		{
			this.TotalRaceTime = Time.time - this.StartRaceTime;
		}
	}

	public void SetRaceTime(float raceTime)
	{
		this.TotalRaceTime = raceTime;
	}

	protected virtual void OnStartRace()
	{
		this.StartRaceTime = Time.time;
		this.StartCurrentLapTime = Time.time;
	}

	private void OnFinishLap(int finishedLap)
	{
		if (finishedLap == 0)
		{
			return;
		}
		if (this.CurrentLapTime < this.BestLapTime || Mathf.Approximately(this.BestLapTime, 0f))
		{
			this.BestLapTime = this.CurrentLapTime;
		}
		this.StartCurrentLapTime = Time.time;
	}

	protected virtual void OnFinishRace()
	{
	}

	protected virtual void OnForceFinish()
	{
		float progressDistance = this.PositioningCar.ProgressDistance;
		float num = PositioningSystem.PositioningAndAiPath.Length * (float)PositioningSystem.LapsCount;
		float num2 = 1f - progressDistance / num;
		this.TotalRaceTime += this.TotalRaceTime * num2;
	}

	private void OnDestroy()
	{
		if (GameController.Instance != null)
		{
			GameController instance = GameController.Instance;
			instance.OnStartRaceAction = (Action)Delegate.Remove(instance.OnStartRaceAction, new Action(this.OnStartRace));
		}
	}

	private float StartRaceTime;

	private float StartCurrentLapTime;

	private bool StopUpdateTime;
}
