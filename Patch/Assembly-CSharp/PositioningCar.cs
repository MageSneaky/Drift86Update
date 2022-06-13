using System;
using UnityEngine;

public class PositioningCar : MonoBehaviour
{
	public bool IsFinished { get; private set; }

	public int CurrentLap { get; private set; }

	public bool IsWrongDirection { get; private set; }

	public WaypointCircuit Circuit
	{
		get
		{
			return PositioningSystem.PositioningAndAiPath;
		}
	}

	public bool LastPointIsCorrect
	{
		get
		{
			return this.ProgressDistance >= this.LastCorrectProgressDistance;
		}
	}

	public WaypointCircuit.RoutePoint LastCorrectPosition
	{
		get
		{
			return this.Circuit.GetRoutePoint(this.LastCorrectProgressDistance);
		}
	}

	public WaypointCircuit.RoutePoint ProgressPoint { get; private set; }

	public float ProgressDistance { get; private set; }

	public float LastCorrectProgressDistance { get; private set; }

	public float LapLength
	{
		get
		{
			return this.Circuit.Length;
		}
	}

	public bool IsLocalCar
	{
		get
		{
			return !WorldLoading.IsMultiplayer || this.MultiplayerCar == null || this.MultiplayerCar.IsMine;
		}
	}

	private void Awake()
	{
		if (!GameController.InGameScene)
		{
			base.enabled = false;
			return;
		}
		this.IsWrongDirection = false;
		this.CurrentLap = 1;
		this.CarController = base.GetComponent<CarController>();
	}

	private void Start()
	{
		this.ProgressPoint = this.Circuit.GetRoutePoint(0f);
		this.MultiplayerCar = base.GetComponent<MultiplayerCarController>();
		GameController instance = GameController.Instance;
		instance.OnEndGameAction = (Action)Delegate.Combine(instance.OnEndGameAction, new Action(this.ForceFinish));
	}

	private void FixedUpdate()
	{
		if (!GameController.InPause)
		{
			this.UpdateProgress();
		}
	}

	private void UpdateProgress()
	{
		Vector3 lhs = this.ProgressPoint.position - base.transform.position;
		float num = Vector3.Dot(lhs, this.ProgressPoint.direction);
		if (num < 0f)
		{
			while (num < 0f)
			{
				this.ProgressDistance += Mathf.Max(0.5f, this.CarController.CurrentSpeed * Time.fixedDeltaTime);
				this.ProgressPoint = this.Circuit.GetRoutePoint(this.ProgressDistance);
				lhs = this.ProgressPoint.position - base.transform.position;
				num = Vector3.Dot(lhs, this.ProgressPoint.direction);
			}
			this.DistanceToProgressPoint = (this.ProgressPoint.position - base.transform.position).magnitude;
			if (this.ProgressDistance > this.LastCorrectProgressDistance)
			{
				this.LastCorrectProgressDistance = this.ProgressDistance;
			}
			this.IsWrongDirection = false;
			if (this.ProgressDistance > this.LapLength * (float)this.CurrentLap)
			{
				this.CrossedFinishLine();
				return;
			}
		}
		else if (this.ProgressDistance > 0f && (this.DistanceToProgressPoint + 10f) * (this.DistanceToProgressPoint + 10f) < lhs.sqrMagnitude)
		{
			num = Vector3.Dot(lhs, -this.ProgressPoint.direction);
			if (num < 0f)
			{
				this.ProgressDistance -= lhs.magnitude * 0.5f;
				this.ProgressPoint = this.Circuit.GetRoutePoint(this.ProgressDistance);
				this.IsWrongDirection = true;
				this.DistanceToProgressPoint = (this.ProgressPoint.position - base.transform.position).magnitude;
			}
		}
	}

	private void CrossedFinishLine()
	{
		if (this.IsFinished)
		{
			return;
		}
		this.OnFinishLapAction.SafeInvoke(this.CurrentLap);
		if (this.CurrentLap + 1 > PositioningSystem.LapsCount)
		{
			this.FinishRace();
		}
		else
		{
			int currentLap = this.CurrentLap;
			this.CurrentLap = currentLap + 1;
		}
		this.CurrentLap = Mathf.Clamp(this.CurrentLap, 1, PositioningSystem.LapsCount);
	}

	public void ForceFinish()
	{
		if (!this.IsFinished && this.IsLocalCar)
		{
			this.OnForceFinishRaceAction.SafeInvoke();
			this.FinishRace();
			return;
		}
		this.IsFinished = true;
	}

	private void FinishRace()
	{
		if (!this.IsLocalCar || this.IsFinished)
		{
			return;
		}
		this.IsFinished = true;
		this.OnFinishRaceAction.SafeInvoke();
		if (this.CarController == GameController.PlayerCar)
		{
			if (WorldLoading.IsMultiplayer)
			{
				GameController.SendFinishEvent();
				return;
			}
			GameController.Instance.OnEndGameAction.SafeInvoke();
		}
	}

	private void OnDrawGizmosSelected()
	{
		if (Application.isPlaying)
		{
			Gizmos.color = Color.green;
			Gizmos.DrawWireSphere(this.Circuit.GetRoutePosition(this.ProgressDistance), 1f);
		}
	}

	public Action OnStartAction;

	public Action<int> OnFinishLapAction;

	public Action OnFinishRaceAction;

	public Action OnForceFinishRaceAction;

	private MultiplayerCarController MultiplayerCar;

	private CarController CarController;

	private float DistanceToProgressPoint;
}
