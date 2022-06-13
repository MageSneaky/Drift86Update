using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RaceEntity : BaseRaceEntity
{
	public CarStatistic PlayerStatistics { get; private set; }

	public List<CarStatistic> CarsStatistics { get; private set; }

	public RaceEntity(GameController controller) : base(controller)
	{
		this.CarsStatistics = new List<CarStatistic>();
		foreach (CarController carController in base.AllCars)
		{
			CarStatistic carStatistic;
			if (carController == base.PlayerCar)
			{
				carStatistic = new CarStatistic(carController, WorldLoading.PlayerName, true);
				this.PlayerStatistics = carStatistic;
			}
			else
			{
				carStatistic = new CarStatistic(carController, base.GetNameForBot(), true);
			}
			GameController controller2 = this.Controller;
			controller2.FixedUpdateAction = (Action)Delegate.Combine(controller2.FixedUpdateAction, new Action(carStatistic.FixedUpdate));
			this.CarsStatistics.Add(carStatistic);
		}
		GameController controller3 = this.Controller;
		controller3.FixedUpdateAction = (Action)Delegate.Combine(controller3.FixedUpdateAction, new Action(this.CheckRatingOfPlayers));
	}

	public override void AddMultiplayerCar(MultiplayerCarController multiplayerController)
	{
		base.AddMultiplayerCar(multiplayerController);
		CarStatistic carStatistic = new CarStatistic(multiplayerController.Car, multiplayerController.NickName, multiplayerController.IsMine);
		this.CarsStatistics.Add(carStatistic);
		if (multiplayerController.IsMine)
		{
			this.PlayerStatistics = carStatistic;
		}
		Debug.Log("ADDING PLAYER RACE");
		this.CheckRatingOfPlayers();
		this.MultiplayerCarAdded.SafeInvoke();
	}

	public override void CheckRatingOfPlayers()
	{
		bool flag = false;
		for (int i = 1; i < this.CarsStatistics.Count; i++)
		{
			if (this.CarsStatistics[i].PositioningCar.ProgressDistance > this.CarsStatistics[i - 1].PositioningCar.ProgressDistance)
			{
				flag = true;
				break;
			}
		}
		if (flag)
		{
			this.CarsStatistics = (from s in this.CarsStatistics
			orderby -s.PositioningCar.ProgressDistance
			select s).ToList<CarStatistic>();
			this.Controller.RatingOfPlayersChanged.SafeInvoke();
		}
	}
}
