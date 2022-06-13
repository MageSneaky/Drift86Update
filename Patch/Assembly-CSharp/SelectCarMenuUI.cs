using System;
using System.Collections;
using System.Collections.Generic;
using CodeStage.AntiCheat.Storage;
using GameBalance;
using HeathenEngineering.SteamAPI;
using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SelectCarMenuUI : WindowWithShopLogic
{
	private List<CarPreset> Cars
	{
		get
		{
			if (!WorldLoading.IsMultiplayer)
			{
				return WorldLoading.AvailableCars;
			}
			return B.MultiplayerSettings.AvailableCarsForMultiplayer;
		}
	}

	public Action<CarPreset> OnSelectCarAction { get; set; }

	protected override void Awake()
	{
		this.SetNextCarButton.onClick.AddListener(new UnityAction(this.NextCar));
		this.SetPrevCarButton.onClick.AddListener(new UnityAction(this.PrevCar));
		this.SelectParamsPanelButton.onClick.AddListener(new UnityAction(this.OnSelectParamsPanel));
		this.SelectColorPanelButton.onClick.AddListener(new UnityAction(this.OnSelectColorPanel));
	}

	private void OnEnable()
	{
		if (this.IsMultiplayer != WorldLoading.IsMultiplayer)
		{
			this.IsMultiplayer = WorldLoading.IsMultiplayer;
			this.CurrentCarIndex = 0;
		}
		this.SelectCar(this.Cars[this.CurrentCarIndex]);
	}

	private IEnumerator Start()
	{
		yield return null;
		this.OnSelectParamsPanel();
		yield break;
	}

	private void Update()
	{
		if (!ObscuredPrefs.GetBool("DLC - CarsPack", false))
		{
			foreach (string b in this.CarsInDLC)
			{
				if (this.CarCaptionText.text == b)
				{
					this.SelectButton.interactable = false;
				}
			}
			if (!this.SelectButton.interactable)
			{
				this.DLCButton.SetActive(true);
			}
			else
			{
				this.DLCButton.SetActive(false);
			}
		}
		else if (ObscuredPrefs.GetBool("DLC - CarsPack", false) && !this.SelectButton.interactable)
		{
			this.SelectButton.interactable = true;
			this.DLCButton.SetActive(false);
			Debug.Log("[OK] DLC, INTERABLE TRUE DES BTN");
		}
		if (Singleton<WindowsController>.Instance.CurrentWindow != this)
		{
			return;
		}
		float axis = Input.GetAxis("Horizontal");
		if (!Mathf.Approximately(axis, 0f))
		{
			if (!this.HorizontalIsPressed)
			{
				if (axis > 0f)
				{
					this.NextCar();
				}
				else
				{
					this.PrevCar();
				}
			}
			this.HorizontalIsPressed = true;
		}
		else
		{
			this.HorizontalIsPressed = false;
		}
		if (!Mathf.Approximately(Input.GetAxis("Submit"), 0f))
		{
			if (!this.SubmitIsPressed && this.SelectButton.interactable)
			{
				Debug.Log("LA SA CLIQUE");
				this.SelectButton.onClick.Invoke();
			}
			this.SubmitIsPressed = true;
			return;
		}
		this.SubmitIsPressed = false;
	}

	private void SelectCar(CarPreset newCar)
	{
		if (this.CarInScene)
		{
			Object.Destroy(this.CarInScene.gameObject);
		}
		this.CarCaptionText.text = newCar.CarCaption;
		this.SelectedCar = newCar;
		base.RefreshButtonState(newCar);
		GameObject gameObject = newCar.CarPrefabForSelectMenu;
		if (gameObject == null)
		{
			gameObject = newCar.CarPrefab.gameObject;
		}
		this.CarInScene = Object.Instantiate<GameObject>(gameObject);
		this.CarInScene.transform.position = this.CarPosition.position;
		this.CarInScene.transform.rotation = this.CarPosition.rotation;
		this.CarParammsPanel.SelectCar(newCar);
		this.CarSetColorPanel.SelectCar(newCar, this.CarInScene.GetComponent<ISetColor>());
	}

	protected override void OnSelect()
	{
		int num = 0;
		if (!ObscuredPrefs.GetBool("DLC - CarsPack", false))
		{
			foreach (string b in this.CarsInDLC)
			{
				if (this.CarCaptionText.text == b)
				{
					this.SelectButton.interactable = false;
					Debug.Log("TENTATIVE DE GLITCH");
					num = 1;
				}
			}
			if (num == 0)
			{
				this.StartGame();
				return;
			}
		}
		else if (ObscuredPrefs.GetBool("DLC - CarsPack", false))
		{
			this.StartGame();
		}
	}

	public void CHeckDlcpage()
	{
		this.CarsPack.OpenStore(0);
	}

	public void StartGame()
	{
		WorldLoading.PlayerCar = this.SelectedCar;
		this.mycar = this.SelectedCar;
		if (this.OnSelectCarAction != null)
		{
			this.OnSelectCarAction.SafeInvoke(this.SelectedCar);
			return;
		}
		WorldLoading.IsMultiplayer = false;
		LoadingScreenUI.LoadScene(WorldLoading.LoadingTrack.SceneName, WorldLoading.RegimeSettings.RegimeSceneName);
		Debug.Log("DEMARRAGE DU MODE SINGLEPLAYER");
		SteamUserStats.SetAchievement("Play_SP");
		SteamSettings.Client.StoreStatsAndAchievements();
	}

	private void NextCar()
	{
		this.CurrentCarIndex = MathExtentions.LoopClamp(this.CurrentCarIndex + 1, 0, this.Cars.Count);
		this.SelectCar(this.Cars[this.CurrentCarIndex]);
	}

	private void PrevCar()
	{
		this.CurrentCarIndex = MathExtentions.LoopClamp(this.CurrentCarIndex - 1, 0, this.Cars.Count);
		this.SelectCar(this.Cars[this.CurrentCarIndex]);
	}

	public override void Open()
	{
		this.SubmitIsPressed = true;
		this.OnSelectCarAction = null;
		Singleton<CameraInMainMenu>.Instance.SetCarSelectMenu(true);
		base.Open();
	}

	public override void Close()
	{
		Singleton<CameraInMainMenu>.Instance.SetCarSelectMenu(false);
		base.Close();
	}

	public void OnSelectParamsPanel()
	{
		this.SelectParamsPanelButton.interactable = false;
		this.SelectColorPanelButton.interactable = true;
		if (this.MovePanelsCoroutine != null)
		{
			base.StopCoroutine(this.MovePanelsCoroutine);
			this.MovePanelsCoroutine = null;
		}
		this.MovePanelsCoroutine = base.StartCoroutine(this.DoShowPanel(this.CarParammsPanel.transform, this.CarSetColorPanel.transform, this.SelectParamsPanelButton.transform.localPosition));
	}

	public void OnSelectColorPanel()
	{
		this.SelectParamsPanelButton.interactable = true;
		this.SelectColorPanelButton.interactable = false;
		if (this.MovePanelsCoroutine != null)
		{
			base.StopCoroutine(this.MovePanelsCoroutine);
			this.MovePanelsCoroutine = null;
		}
		this.MovePanelsCoroutine = base.StartCoroutine(this.DoShowPanel(this.CarSetColorPanel.transform, this.CarParammsPanel.transform, this.SelectColorPanelButton.transform.localPosition));
	}

	private IEnumerator DoShowPanel(Transform showPanel, Transform hidePanel, Vector3 newSelectedBackgroundPos)
	{
		while (!Mathf.Approximately(showPanel.position.y, this.ShownPanelPos.position.y) || !Mathf.Approximately(hidePanel.position.y, this.HiddenPanelPos.position.y) || !Mathf.Approximately(this.SelectedButtonBackground.transform.position.x, newSelectedBackgroundPos.x))
		{
			yield return null;
			showPanel.position = Vector3.Lerp(showPanel.position, this.ShownPanelPos.position, Time.deltaTime * this.MovePanelsSpeed);
			hidePanel.position = Vector3.Lerp(hidePanel.position, this.HiddenPanelPos.position, Time.deltaTime * this.MovePanelsSpeed);
			this.SelectedButtonBackground.localPosition = Vector3.Lerp(this.SelectedButtonBackground.localPosition, newSelectedBackgroundPos, Time.deltaTime * this.MovePanelsSpeed);
		}
		yield break;
	}

	private void OnDisable()
	{
		base.StopAllCoroutines();
	}

	[SerializeField]
	private Button SetNextCarButton;

	[SerializeField]
	private Button SetPrevCarButton;

	[SerializeField]
	private TextMeshProUGUI CarCaptionText;

	[SerializeField]
	private Transform CarPosition;

	[SerializeField]
	private CarParamsUI CarParammsPanel;

	[SerializeField]
	private CarSetColorUI CarSetColorPanel;

	[SerializeField]
	private Button SelectParamsPanelButton;

	[SerializeField]
	private Button SelectColorPanelButton;

	[SerializeField]
	private Transform SelectedButtonBackground;

	[SerializeField]
	private RectTransform ShownPanelPos;

	[SerializeField]
	private RectTransform HiddenPanelPos;

	[SerializeField]
	private float MovePanelsSpeed;

	[SerializeField]
	public CarPreset mycar;

	[Space]
	public DownloadableContentObject CarsPack;

	public string[] CarsInDLC;

	public GameObject DLCButton;

	private bool IsMultiplayer;

	private int CurrentCarIndex;

	private CarPreset SelectedCar;

	private GameObject CarInScene;

	private Coroutine MovePanelsCoroutine;

	private bool SubmitIsPressed = true;

	private bool HorizontalIsPressed = true;
}
