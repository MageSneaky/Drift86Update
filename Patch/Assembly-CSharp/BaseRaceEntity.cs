using System;
using System.Collections.Generic;

public class BaseRaceEntity
{
	protected CarController PlayerCar
	{
		get
		{
			return GameController.PlayerCar;
		}
	}

	protected List<CarController> AllCars
	{
		get
		{
			return GameController.AllCars;
		}
	}

	public BaseRaceEntity(GameController controller)
	{
		this.Controller = controller;
	}

	public virtual void AddMultiplayerCar(MultiplayerCarController multiplayerController)
	{
	}

	public virtual void CheckRatingOfPlayers()
	{
	}

	protected string GetNameForBot()
	{
		if (this.BotNames.Count == 0)
		{
			this.BotNames.AddRange(B.GameSettings.BotNames);
		}
		string text = this.BotNames.RandomChoice<string>();
		this.BotNames.Remove(text);
		return text;
	}

	protected GameController Controller;

	public Action MultiplayerCarAdded;

	private List<string> BotNames = new List<string>();
}
