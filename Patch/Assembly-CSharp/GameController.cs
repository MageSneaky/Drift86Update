using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ExitGames.Client.Photon;
using GameBalance;
using HeathenEngineering.SteamAPI;
using Photon.Pun;
using Photon.Realtime;
using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviourPunCallbacks, IOnEventCallback
{
	public static bool RaceIsStarted
	{
		get
		{
			return GameController.Instance.m_RaceIsStarted;
		}
	}

	public static CarController PlayerCar
	{
		get
		{
			return GameController.Instance.m_PlayerCar;
		}
	}

	public static List<CarController> AllCars
	{
		get
		{
			return GameController.Instance.m_AllCars;
		}
	}

	public static bool InGameScene
	{
		get
		{
			return GameController.Instance != null;
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
			return GameController.Instance.m_GameIsEnded;
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
		if (!WorldLoading.HasLoadingParams && !SceneManager.GetActiveScene().name.Contains("Persistant"))
		{
			Debug.Log("LOADING WSH");
			WorldLoading.RegimeForDebug = this.RegimeForDebug;
			LoadingScreenUI.LoadScene(this.RegimeForDebug.RegimeSceneName, LoadSceneMode.Additive);
			Debug.Log("LOADING WSH");
		}
		GameController.Instance = this;
		this.OnEndGameAction = (Action)Delegate.Combine(this.OnEndGameAction, new Action(delegate()
		{
			this.m_GameIsEnded = true;
		}));
		base.StartCoroutine(this.StartRaceCoroutine());
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
		if (WorldLoading.IsMultiplayer)
		{
			this.m_AllCars.ForEach(delegate(CarController c)
			{
				UnityEngine.Object.Destroy(c.gameObject);
			});
			this.m_AllCars.Clear();
			this.InitRaceEntity();
			if (!SceneManager.GetActiveScene().name.Contains("Persistant"))
			{
				base.StartCoroutine(this.InstantiateMultiplayerCar());
			}
			return;
		}
		if (!WorldLoading.HasLoadingParams)
		{
			if (GameController.AllCars.All((CarController c) => c.GetComponent<UserControl>() == null))
			{
				CarController carController3 = GameController.AllCars.First<CarController>();
				UserControl userControl = carController3.gameObject.AddComponent<UserControl>();
				this.m_PlayerCar = carController3;
				if (carController3.GetComponent<DriftAIControl>() != null)
				{
					userControl.enabled = false;
				}
			}
			if (this.m_PlayerCar != null)
			{
				this.m_PlayerCar.gameObject.AddComponent<AudioListener>();
			}
			else
			{
				Debug.LogErrorFormat("[Debug Scene] PlayerCar not found ", Array.Empty<object>());
			}
			this.InitRaceEntity();
			return;
		}
		this.m_AllCars.ForEach(delegate(CarController c)
		{
			UnityEngine.Object.Destroy(c.gameObject);
		});
		this.m_AllCars.Clear();
		this.m_PlayerCar = UnityEngine.Object.Instantiate<CarController>(WorldLoading.PlayerCar.CarPrefab);
		if (this.m_PlayerCar.GetComponent<UserControl>() == null)
		{
			this.m_PlayerCar.gameObject.AddComponent<UserControl>();
		}
		this.m_PlayerCar.SetColor(WorldLoading.SelectedColor);
		this.m_AllCars.Add(this.m_PlayerCar);
		for (int j = 0; j < WorldLoading.AIsCount; j++)
		{
			CarPreset carPreset = WorldLoading.AvailableCars.RandomChoice<CarPreset>();
			CarController carController4 = UnityEngine.Object.Instantiate<CarController>(carPreset.CarPrefab);
			carController4.gameObject.AddComponent<DriftAIControl>();
			UserControl component2 = carController4.GetComponent<UserControl>();
			if (component2 != null)
			{
				UnityEngine.Object.Destroy(component2);
			}
			carController4.SetColor(carPreset.GetRandomColor());
			this.m_AllCars.Add(carController4);
		}
		for (int k = 0; k < this.m_AllCars.Count; k++)
		{
			int index = UnityEngine.Random.Range(0, this.m_AllCars.Count);
			CarController value = this.m_AllCars[k];
			this.m_AllCars[k] = this.m_AllCars[index];
			this.m_AllCars[index] = value;
		}
		this.m_PlayerCar.gameObject.AddComponent<AudioListener>();
		if (this.m_AllCars.Count > this.CarPositions.Count)
		{
			Debug.LogErrorFormat("CarPositions less loaded cars count: CarPositions: {0}, Loaded cars: {1}", new object[]
			{
				this.CarPositions.Count,
				this.m_AllCars.Count
			});
			return;
		}
		for (int l = 0; l < this.m_AllCars.Count; l++)
		{
			this.m_AllCars[l].transform.position = this.CarPositions[l].position;
			this.m_AllCars[l].transform.rotation = this.CarPositions[l].rotation;
		}
		this.InitRaceEntity();
	}

	private void InitRaceEntity()
	{
		if (WorldLoading.RegimeSettings is DriftRegimeSettings)
		{
			GameController.RaceEntity = new DriftRaceEntity(this);
			return;
		}
		GameController.RaceEntity = new RaceEntity(this);
	}

	private IEnumerator StartRaceCoroutine()
	{
		if (WorldLoading.IsMultiplayer)
		{
			PhotonNetwork.LocalPlayer.SetCustomProperties(new object[]
			{
				"il",
				true,
				"ir",
				false
			});
		}
		yield return new WaitForSeconds(0f);
		while (!LoadingScreenUI.IsLoaded)
		{
			yield return null;
		}
		UnityEngine.Object.Instantiate<GameObject>(this.CountdownObject).SetActive(true);
		yield return new WaitForSeconds(this.CountdownTime);
		this.OnStartRaceAction.SafeInvoke();
		this.m_RaceIsStarted = true;
		yield return new WaitForSeconds(this.DellayCountdownShowHide);
		this.CountdownObject.SetActive(false);
		this.SteamSuccess();
		yield break;
	}

	public void SteamSuccess()
	{
		GameObject[] array = GameObject.FindGameObjectsWithTag("SM");
		if (array.Length >= 1)
		{
			SteamUserStats.SetAchievement("Play_Multi");
			SteamSettings.Client.StoreStatsAndAchievements();
		}
		if (UnityEngine.Object.FindObjectOfType<UserControl>() && array.Length >= 1)
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
		Debug.Log("1111111111");
		yield return new WaitForSeconds(0f);
		while (!LoadingScreenUI.IsLoaded)
		{
			Debug.Log("22222222222");
			yield return null;
		}
		Debug.Log("333333333");
		Vector3 position = this.CarPositions[0].position;
		Quaternion rotation = this.CarPositions[0].rotation;
		int num = 0;
		Debug.Log("4444444444444");
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
		Debug.Log("55555555555555555555");
		if (!SceneManager.GetActiveScene().name.Contains("Persistant"))
		{
			PhotonNetwork.Instantiate(WorldLoading.PlayerCar.CarPrefab.name, position, rotation, 0, null);
		}
		Debug.Log("66666666666666");
		yield break;
	}

	public virtual void AddMultiplayerCar(MultiplayerCarController multiplayerController)
	{
		GameController.AllCars.Add(multiplayerController.Car);
		this.MultiplayerCars.Add(multiplayerController);
		if (multiplayerController.IsMine)
		{
			this.m_PlayerCar = multiplayerController.Car;
		}
		GameController.RaceEntity.AddMultiplayerCar(multiplayerController);
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
			if (!GameController.AllCars.Any((CarController c) => c != null && !c.PositioningCar.IsFinished))
			{
				break;
			}
			endGameTimerText.text = string.Format(this.EndGameTextPrefix, timer);
			int num = timer;
			timer = num - 1;
			yield return new WaitForSeconds(1f);
		}
		if (!GameController.PlayerCar.PositioningCar.IsFinished)
		{
			GameController.PlayerCar.PositioningCar.OnFinishRaceAction.SafeInvoke();
			GameController.SendFinishEvent();
		}
		for (;;)
		{
			if (!GameController.AllCars.Any((CarController c) => c != null && !c.PositioningCar.IsFinished))
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

	public void FixedUpdate()
	{
		this.FixedUpdateAction.SafeInvoke();
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

	public static GameController Instance;

	public static BaseRaceEntity RaceEntity;

	private bool m_GameIsEnded;

	private bool m_RaceIsStarted;

	public Action RatingOfPlayersChanged;

	[SerializeField]
	public Action OnEndGameAction;

	[SerializeField]
	public Action OnStartRaceAction;

	public Action FixedUpdateAction;

	private List<CarController> m_AllCars = new List<CarController>();

	private CarController m_PlayerCar;

	private Coroutine FinishTimerCoroutine;

	private List<MultiplayerCarController> MultiplayerCars = new List<MultiplayerCarController>();
}
