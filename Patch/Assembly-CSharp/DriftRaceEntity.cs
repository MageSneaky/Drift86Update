using System;
using System.Collections.Generic;
using System.Linq;

public class DriftRaceEntity : BaseRaceEntity
{
	public CarStatisticsDriftRegime PlayerDriftStatistics { get; private set; }

	public List<CarStatisticsDriftRegime> DriftStatistics { get; private set; }

	public DriftRaceEntity(GameController controller) : base(controller)
	{
		this.DriftStatistics = new List<CarStatisticsDriftRegime>();
		foreach (CarController carController in base.AllCars)
		{
			CarStatisticsDriftRegime carStatisticsDriftRegime;
			if (carController == base.PlayerCar)
			{
				carStatisticsDriftRegime = new CarStatisticsDriftRegime(carController, WorldLoading.PlayerName, true);
				this.PlayerDriftStatistics = carStatisticsDriftRegime;
			}
			else
			{
				carStatisticsDriftRegime = new CarStatisticsDriftRegime(carController, base.GetNameForBot(), true);
			}
			CarStatisticsDriftRegime carStatisticsDriftRegime2 = carStatisticsDriftRegime;
			carStatisticsDriftRegime2.TotalScoreChanged = (Action)Delegate.Combine(carStatisticsDriftRegime2.TotalScoreChanged, new Action(this.CheckRatingOfPlayers));
			this.DriftStatistics.Remove(carStatisticsDriftRegime);
			this.DriftStatistics.Add(carStatisticsDriftRegime);
		}
	}

	public override void AddMultiplayerCar(MultiplayerCarController multiplayerController)
	{
		base.AddMultiplayerCar(multiplayerController);
		CarStatisticsDriftRegime carStatisticsDriftRegime = new CarStatisticsDriftRegime(multiplayerController.Car, multiplayerController.NickName, multiplayerController.IsMine);
		this.DriftStatistics.Remove(carStatisticsDriftRegime);
		this.DriftStatistics.Add(carStatisticsDriftRegime);
		if (multiplayerController.IsMine)
		{
			this.PlayerDriftStatistics = carStatisticsDriftRegime;
		}
		GameController instance = GameController.Instance;
		instance.FixedUpdateAction = (Action)Delegate.Combine(instance.FixedUpdateAction, new Action(carStatisticsDriftRegime.FixedUpdate));
		CarStatisticsDriftRegime carStatisticsDriftRegime2 = carStatisticsDriftRegime;
		carStatisticsDriftRegime2.TotalScoreChanged = (Action)Delegate.Combine(carStatisticsDriftRegime2.TotalScoreChanged, new Action(this.CheckRatingOfPlayers));
		this.MultiplayerCarAdded.SafeInvoke();
	}

	public override void CheckRatingOfPlayers()
	{
		bool flag = false;
		for (int i = 1; i < this.DriftStatistics.Count; i++)
		{
			if (this.DriftStatistics[i].TotalScore > this.DriftStatistics[i - 1].TotalScore)
			{
				flag = true;
				break;
			}
		}
		if (flag)
		{
			this.DriftStatistics = (from s in this.DriftStatistics
			orderby -s.TotalScore
			select s).ToList<CarStatisticsDriftRegime>();
		}
		this.Controller.RatingOfPlayersChanged.SafeInvoke();
	}
}
