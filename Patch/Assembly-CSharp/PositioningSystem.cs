using System;
using System.Collections.Generic;
using UnityEngine;

public class PositioningSystem : MonoBehaviour
{
	public static WaypointCircuit PositioningAndAiPath
	{
		get
		{
			return PositioningSystem.Instance.m_PositioningAndAiPath;
		}
	}

	public static int LapsCount { get; private set; }

	public WaypointCircuit GetPathForVisual
	{
		get
		{
			return this.m_PathForVisual;
		}
	}

	private void Awake()
	{
		PositioningSystem.Instance = this;
		PositioningSystem.LapsCount = WorldLoading.LapsCount;
	}

	private void Start()
	{
		PositioningSystem.OrderedCars = new List<PositioningCar>();
		foreach (CarController carController in GameController.AllCars)
		{
			PositioningSystem.OrderedCars.Add(carController.PositioningCar);
		}
	}

	private void Update()
	{
		this.SortCars();
	}

	private void SortCars()
	{
		PositioningSystem.OrderedCars.Sort(new PositioningSystem.CarsComparer());
	}

	public static int GetCarPos(PositioningCar car)
	{
		return PositioningSystem.OrderedCars.IndexOf(car);
	}

	public static int GetCarPos(CarController car)
	{
		return PositioningSystem.GetCarPos(car.PositioningCar);
	}

	[SerializeField]
	private WaypointCircuit m_PositioningAndAiPath;

	[SerializeField]
	private WaypointCircuit m_PathForVisual;

	public static PositioningSystem Instance;

	public static List<PositioningCar> OrderedCars;

	private class CarsComparer : IComparer<PositioningCar>
	{
		public int Compare(PositioningCar x, PositioningCar y)
		{
			if (x.ProgressDistance > y.ProgressDistance)
			{
				return -1;
			}
			return 1;
		}
	}
}
