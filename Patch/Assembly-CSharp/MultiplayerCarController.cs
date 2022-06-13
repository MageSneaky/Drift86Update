using System;
using System.Collections.Generic;
using System.Linq;
using GameBalance;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(CarController))]
public class MultiplayerCarController : MonoBehaviourPunCallbacks, IPunObservable
{
	public string NickName
	{
		get
		{
			if (this.PhotonView.Owner == null)
			{
				return string.Empty;
			}
			return this.PhotonView.Owner.NickName;
		}
	}

	public CarController Car { get; private set; }

	public PhotonView PhotonView { get; private set; }

	public bool IsMine
	{
		get
		{
			return this.PhotonView.IsMine;
		}
	}

	private Rigidbody RB
	{
		get
		{
			return this.Car.RB;
		}
	}

	private void Start()
	{
		if (!WorldLoading.HasLoadingParams || !GameController.InGameScene)
		{
			Object.Destroy(this.PhotonView);
			Object.Destroy(this);
			return;
		}
		this.PhotonView = base.GetComponent<PhotonView>();
		if (this.PhotonView == null)
		{
			Debug.LogError("GameObject without PhotonView");
			Object.Destroy(this);
			return;
		}
		if (!WorldLoading.IsMultiplayer || !WorldLoading.HasLoadingParams)
		{
			Object.Destroy(this.PhotonView);
			Object.Destroy(this);
			return;
		}
		this.Car = base.GetComponent<CarController>();
		if (this.IsMine)
		{
			if (B.MultiplayerSettings.EnableAiForDebug)
			{
				base.gameObject.AddComponent<UserControl>().enabled = false;
				this.UserControl = base.gameObject.AddComponent<DriftAIControl>();
			}
			else
			{
				this.UserControl = base.gameObject.AddComponent<UserControl>();
			}
			base.gameObject.AddComponent<AudioListener>();
		}
		if (GameController.Instance == null)
		{
			Debug.LogError("GameController not found");
			return;
		}
		if (this.PhotonView.Owner != null && this.PhotonView.Owner.CustomProperties != null && this.PhotonView.Owner.CustomProperties.ContainsKey("cn") && this.PhotonView.Owner.CustomProperties.ContainsKey("cci"))
		{
			CarColorPreset color = WorldLoading.AvailableCars.Find((CarPreset c) => c.CarCaption == (string)this.PhotonView.Owner.CustomProperties["cn"]).AvailibleColors[(int)this.PhotonView.Owner.CustomProperties["cci"]];
			this.Car.SetColor(color);
		}
		GameController.Instance.AddMultiplayerCar(this);
		this.StartDriftRegime();
		this.StartRaceRegime();
		this.SqrDistanceFastLerp = B.MultiplayerSettings.DistanceFastSync * B.MultiplayerSettings.DistanceFastSync;
		this.SqrDistanceTeleport = B.MultiplayerSettings.DistanceTeleport * B.MultiplayerSettings.DistanceTeleport;
		this.NickNameText = Object.Instantiate<TextMeshPro>(B.MultiplayerSettings.NickNameInWorld, base.transform);
		this.NickNameText.text = this.PhotonView.Owner.NickName;
		this.NickNameText.transform.SetLocalY(B.MultiplayerSettings.NickNameY);
		if (PhotonNetwork.CurrentRoom.Players.ContainsValue(this.PhotonView.Owner))
		{
			int num = PhotonNetwork.CurrentRoom.Players.FirstOrDefault((KeyValuePair<int, Player> p) => p.Value == this.PhotonView.Owner).Key - 1;
			num = MathExtentions.LoopClamp(num, 0, B.MultiplayerSettings.NickNameColors.Count);
			this.NickNameText.color = B.MultiplayerSettings.NickNameColors[num];
		}
		this.NickNameText.SetActive(false);
	}

	private void StartDriftRegime()
	{
		DriftRaceEntity driftRaceEntity = GameController.RaceEntity as DriftRaceEntity;
		if (driftRaceEntity != null)
		{
			this.StatisticsDrift = driftRaceEntity.DriftStatistics.FirstOrDefault((CarStatisticsDriftRegime s) => s.Car == this.Car);
			CarStatisticsDriftRegime statisticsDrift = this.StatisticsDrift;
			statisticsDrift.TotalScoreChanged = (Action)Delegate.Combine(statisticsDrift.TotalScoreChanged, new Action(this.UpdateTotalScore));
		}
	}

	private void UpdateTotalScore()
	{
		if (this.IsMine && this.StatisticsDrift != null)
		{
			this.PhotonView.RPC("UpdateTotalScore", 1, new object[]
			{
				this.StatisticsDrift.TotalScore
			});
		}
	}

	[PunRPC]
	private void UpdateTotalScore(float totalScore)
	{
		if (this.StatisticsDrift != null)
		{
			this.StatisticsDrift.UpdateScore(totalScore);
		}
	}

	private void StartRaceRegime()
	{
		RaceEntity raceEntity = GameController.RaceEntity as RaceEntity;
		if (raceEntity != null)
		{
			this.StatisticsRace = raceEntity.CarsStatistics.FirstOrDefault((CarStatistic s) => s.Car == this.Car);
			PositioningCar positioningCar = this.StatisticsRace.PositioningCar;
			positioningCar.OnFinishRaceAction = (Action)Delegate.Combine(positioningCar.OnFinishRaceAction, new Action(this.OnFinishRace));
		}
	}

	private void OnFinishRace()
	{
		if (this.IsMine && this.StatisticsRace != null)
		{
			this.PhotonView.RPC("RPCOnFinishRace", 1, new object[]
			{
				this.StatisticsRace.TotalRaceTime
			});
		}
	}

	[PunRPC]
	private void RPCOnFinishRace(float time)
	{
		this.StatisticsRace.SetRaceTime(time);
	}

	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (this.Car == null)
		{
			return;
		}
		if (stream.IsWriting)
		{
			stream.SendNext(this.Car.RB.position);
			stream.SendNext(this.Car.RB.rotation);
			stream.SendNext(this.Car.RB.velocity);
			stream.SendNext(this.Car.RB.angularVelocity.y);
			stream.SendNext(this.UserControl.Horizontal);
			stream.SendNext(this.UserControl.Vertical);
			stream.SendNext(this.UserControl.Brake);
			return;
		}
		Vector3 vector = (Vector3)stream.ReceiveNext();
		Quaternion quaternion = (Quaternion)stream.ReceiveNext();
		Vector3 vector2 = (Vector3)stream.ReceiveNext();
		float num = (float)stream.ReceiveNext();
		float horizontal = (float)stream.ReceiveNext();
		float vertical = (float)stream.ReceiveNext();
		bool brake = (bool)stream.ReceiveNext();
		float num2 = Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTime));
		vector += vector2 * num2;
		quaternion *= Quaternion.AngleAxis(num * num2, Vector3.up);
		this.SyncRigidbody(vector, quaternion, vector2, num);
		this.Car.UpdateControls(horizontal, vertical, brake);
	}

	public void SyncRigidbody(Vector3 pos, Quaternion rot, Vector3 velocity, float angularVelocity)
	{
		float sqrMagnitude = (pos - this.RB.position).sqrMagnitude;
		if (sqrMagnitude < this.SqrDistanceFastLerp)
		{
			this.RB.MovePosition(Vector3.Lerp(this.RB.position, pos, B.MultiplayerSettings.SlowPosSyncLerp));
			this.RB.MoveRotation(Quaternion.Lerp(this.RB.rotation, rot, B.MultiplayerSettings.SlowRotSyncLerp));
		}
		else if (sqrMagnitude < this.SqrDistanceTeleport)
		{
			this.RB.MovePosition(Vector3.Lerp(this.RB.position, pos, B.MultiplayerSettings.FastPosSyncLerp));
			this.RB.MoveRotation(Quaternion.Lerp(this.RB.rotation, rot, B.MultiplayerSettings.FastRotSyncLerp));
		}
		else
		{
			this.RB.MovePosition(pos);
			this.RB.MoveRotation(rot);
		}
		this.RB.velocity = velocity;
		this.RB.angularVelocity = new Vector3(this.RB.angularVelocity.x, angularVelocity, this.RB.angularVelocity.z);
	}

	private void Update()
	{
		if (this.NickNameText == null)
		{
			return;
		}
		if (Input.GetKeyDown(B.MultiplayerSettings.ShowNickNameCode))
		{
			this.NickNameText.SetActive(!this.NickNameText.gameObject.activeSelf);
		}
		if (this.NickNameText.gameObject.activeInHierarchy)
		{
			this.NickNameText.transform.rotation = Camera.main.transform.rotation;
		}
	}

	private TextMeshPro NickNameText;

	private ICarControl UserControl;

	private float SqrDistanceFastLerp;

	private float SqrDistanceTeleport;

	private CarStatisticsDriftRegime StatisticsDrift;

	private CarStatistic StatisticsRace;
}
