using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CodeStage.AntiCheat.Storage;
using ExitGames.Client.Photon;
using GameBalance;
using HeathenEngineering.SteamAPI;
using Photon.Pun;
using Photon.Realtime;
using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameControllerPersistant : MonoBehaviourPunCallbacks, IOnEventCallback
{
	public static bool RaceIsStarted
	{
		get
		{
			return GameControllerPersistant.Instance.m_RaceIsStarted;
		}
	}

	public static CarController PlayerCar
	{
		get
		{
			return GameControllerPersistant.Instance.m_PlayerCar;
		}
	}

	public static List<CarController> AllCars
	{
		get
		{
			return GameControllerPersistant.Instance.m_AllCars;
		}
	}

	public static bool InGameScene
	{
		get
		{
			return GameControllerPersistant.Instance != null;
		}
	}

	public static bool InPause
	{
		get
		{
			return Mathf.Approximately(Time.timeScale, 0f);
		}
	}

	public static bool RaceIsEnded
	{
		get
		{
			return GameControllerPersistant.Instance.m_GameIsEnded;
		}
	}

	public PositioningSystem PositioningSystem
	{
		get
		{
			return this.m_PositioningSystem;
		}
	}

	private void Awake()
	{
		if (!WorldLoading.HasLoadingParams)
		{
			WorldLoading.RegimeForDebug = this.RegimeForDebug;
		}
		PhotonNetwork.AutomaticallySyncScene = true;
		GameControllerPersistant.Instance = this;
		this.OnEndGameAction = (Action)Delegate.Combine(this.OnEndGameAction, new Action(delegate()
		{
			this.m_GameIsEnded = true;
		}));
		foreach (CarController carController in UnityEngine.Object.FindObjectsOfType<CarController>())
		{
			if (carController.GetComponent<UserControl>() != null)
			{
				if (this.m_PlayerCar != null)
				{
					Debug.LogErrorFormat("CarControllers with UserControl script count > 1", Array.Empty<object>());
				}
				else
				{
					this.m_PlayerCar = carController;
				}
			}
			this.m_AllCars.Add(carController);
		}
		this.CarPositions.ForEach(delegate(Transform p)
		{
			p.SetActive(false);
		});
		foreach (CarController carController2 in this.m_AllCars)
		{
			AudioListener component = carController2.GetComponent<AudioListener>();
			if (component != null)
			{
				UnityEngine.Object.Destroy(component);
			}
		}
		this.m_AllCars.ForEach(delegate(CarController c)
		{
			UnityEngine.Object.Destroy(c.gameObject);
		});
		this.m_AllCars.Clear();
		this.InitRaceEntity();
		base.StartCoroutine(this.InstantiateMultiplayerCar());
	}

	private void InitRaceEntity()
	{
		if (WorldLoading.RegimeSettings is DriftRegimeSettings)
		{
			GameControllerPersistant.RaceEntity = new DriftRaceEntity(base.GetComponent<GameController>());
			return;
		}
		GameControllerPersistant.RaceEntity = new RaceEntity(base.GetComponent<GameController>());
	}

	private void OnGUI()
	{
	}

	private IEnumerator StartRaceCoroutine()
	{
		Debug.Log("2222222222 START RACE COROUTINE");
		if (WorldLoading.IsMultiplayer)
		{
			Debug.Log("BBBBBBBBBBBBBBBB");
			PhotonNetwork.LocalPlayer.SetCustomProperties(new object[]
			{
				"il",
				true,
				"ir",
				false
			});
		}
		Debug.Log("2222222222 START RACE COROUTINE");
		while (!LoadingScreenUI.IsLoaded)
		{
			yield return null;
		}
		Debug.Log("2222222222 START RACE COROUTINE");
		UnityEngine.Object.Instantiate<GameObject>(this.CountdownObject).SetActive(true);
		Debug.Log("2222222222 START RACE COROUTINE");
		yield return new WaitForSeconds(this.CountdownTime);
		this.OnStartRaceAction.SafeInvoke();
		this.m_RaceIsStarted = true;
		Debug.Log("2222222222 START RACE COROUTINE");
		yield return new WaitForSeconds(this.DellayCountdownShowHide);
		Debug.Log("000000000 START RACE COROUTINE");
		this.CountdownObject.SetActive(false);
		Debug.Log("333333333333333 START RACE COROUTINE");
		Debug.Log("44444444444 START RACE COROUTINE");
		yield break;
	}

	public void SetAchivement()
	{
		if (UnityEngine.Object.FindObjectOfType<UserControl>())
		{
			SteamUserStats.SetAchievement(UnityEngine.Object.FindObjectOfType<UserControl>().gameObject.transform.name.Split(new char[]
			{
				'('
			})[0]);
			SteamSettings.Client.StoreStatsAndAchievements();
		}
	}

	private IEnumerator InstantiateMultiplayerCar()
	{
		yield return new WaitForSeconds(1f);
		RoomOptions roomOptions = new RoomOptions();
		roomOptions.MaxPlayers = 12;
		PhotonNetwork.JoinOrCreateRoom(ObscuredPrefs.GetString("MYROOM", ""), roomOptions, null, null);
		yield return new WaitForSeconds(1.5f);
		PhotonNetwork.AutomaticallySyncScene = true;
		while (!LoadingScreenUI.IsLoaded)
		{
			yield return null;
		}
		Vector3 position = this.CarPositions[0].position;
		Quaternion rotation = this.CarPositions[0].rotation;
		int num = 0;
		foreach (KeyValuePair<int, Player> keyValuePair in from p in PhotonNetwork.CurrentRoom.Players
		orderby p.Key
		select p)
		{
			if (keyValuePair.Value == PhotonNetwork.LocalPlayer)
			{
				position = this.CarPositions[num].position;
				rotation = this.CarPositions[num].rotation;
				break;
			}
			num++;
		}
		PhotonNetwork.Instantiate(WorldLoading.PlayerCar.CarPrefab.name, position, rotation, 0, null);
		yield break;
	}

	public virtual void AddMultiplayerCar(MultiplayerCarController multiplayerController)
	{
		GameControllerPersistant.AllCars.Add(multiplayerController.Car);
		this.MultiplayerCars.Add(multiplayerController);
		if (multiplayerController.IsMine)
		{
			this.m_PlayerCar = multiplayerController.Car;
		}
		GameControllerPersistant.RaceEntity.AddMultiplayerCar(multiplayerController);
		Debug.Log("ADDING VIA GAME CONTROLLER");
	}

	public static void SendFinishEvent()
	{
		PhotonNetwork.RaiseEvent(1, null, new RaiseEventOptions
		{
			Receivers = ReceiverGroup.All
		}, SendOptions.SendReliable);
	}

	public void OnEvent(EventData photonEvent)
	{
		if (photonEvent.Code == 1)
		{
			if (this.FinishTimerCoroutine == null)
			{
				this.FinishTimerCoroutine = base.StartCoroutine(this.StartFinishRaceTimer());
			}
			Dictionary<int, Player> players = PhotonNetwork.CurrentRoom.Players;
			MultiplayerCarController multiplayerCarController = this.MultiplayerCars.FirstOrDefault((MultiplayerCarController c) => players.ContainsKey(photonEvent.Sender) && c.PhotonView.Owner == players[photonEvent.Sender]);
			if (multiplayerCarController != null)
			{
				multiplayerCarController.Car.PositioningCar.ForceFinish();
			}
		}
	}

	private IEnumerator StartFinishRaceTimer()
	{
		int timer = B.MultiplayerSettings.SecondsToEndGame;
		GameObject endGameTimerHolder = UnityEngine.Object.Instantiate<GameObject>(this.EndGameTimerHolder);
		endGameTimerHolder.SetActive(true);
		TextMeshProUGUI endGameTimerText = endGameTimerHolder.GetComponentInChildren<TextMeshProUGUI>();
		while (timer >= 0)
		{
			if (!GameControllerPersistant.AllCars.Any((CarController c) => c != null && !c.PositioningCar.IsFinished))
			{
				break;
			}
			endGameTimerText.text = string.Format(this.EndGameTextPrefix, timer);
			int num = timer;
			timer = num - 1;
			yield return new WaitForSeconds(1f);
		}
		if (!GameControllerPersistant.PlayerCar.PositioningCar.IsFinished)
		{
			GameControllerPersistant.PlayerCar.PositioningCar.OnFinishRaceAction.SafeInvoke();
			GameControllerPersistant.SendFinishEvent();
		}
		for (;;)
		{
			if (!GameControllerPersistant.AllCars.Any((CarController c) => c != null && !c.PositioningCar.IsFinished))
			{
				break;
			}
			yield return null;
		}
		endGameTimerHolder.SetActive(false);
		this.OnEndGameAction.SafeInvoke();
		yield break;
	}

	public static void LeaveRoom()
	{
		if (WorldLoading.IsMultiplayer)
		{
			PhotonNetwork.LeaveRoom(true);
		}
	}

	public override void OnDisconnected(DisconnectCause cause)
	{
		Action action = delegate()
		{
			LoadingScreenUI.LoadScene(B.GameSettings.MainMenuSceneName, LoadSceneMode.Single);
		};
		B.MultiplayerSettings.ShowDisconnectCause(cause, null);
	}

	private void Update()
	{
	}

	private void FixedUpdate()
	{
		base.GetComponent<GameController>().FixedUpdate();
	}

	[SerializeField]
	private GameObject CountdownObject;

	[SerializeField]
	private float CountdownTime = 3f;

	[SerializeField]
	private float DellayCountdownShowHide = 1f;

	[SerializeField]
	private List<Transform> CarPositions = new List<Transform>();

	[SerializeField]
	private PositioningSystem m_PositioningSystem;

	[SerializeField]
	private GameObject EndGameTimerHolder;

	[SerializeField]
	[TextArea(1, 2)]
	private string EndGameTextPrefix = "The first player finished.\nThe game will end in {0} seconds";

	[Space(10f)]
	[SerializeField]
	private RegimeSettings RegimeForDebug;

	public static GameControllerPersistant Instance;

	public static BaseRaceEntity RaceEntity;

	private bool m_GameIsEnded;

	private bool m_RaceIsStarted;

	public Action RatingOfPlayersChanged;

	public Action OnEndGameAction;

	public Action OnStartRaceAction;

	public Action FixedUpdateAction;

	private List<CarController> m_AllCars = new List<CarController>();

	private CarController m_PlayerCar;

	private Coroutine FinishTimerCoroutine;

	private List<MultiplayerCarController> MultiplayerCars = new List<MultiplayerCarController>();
}
