using System;
using System.Collections.Generic;
using System.Linq;
using GameBalance;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SelectTrackUI : WindowWithShopLogic
{
	public Action<TrackPreset> OnSelectTrackAction { get; set; }

	private List<TrackPreset> Tracks
	{
		get
		{
			if (!WorldLoading.IsMultiplayer)
			{
				return B.GameSettings.Tracks;
			}
			return B.MultiplayerSettings.AvailableTracksForMultiplayer;
		}
	}

	private void Start()
	{
		this.NextTrackButton.onClick.AddListener(new UnityAction(this.NextTrack));
		this.PrevTrackButton.onClick.AddListener(new UnityAction(this.PrevTrack));
	}

	private void OnEnable()
	{
		if (this.IsMultiplayer != WorldLoading.IsMultiplayer)
		{
			this.IsMultiplayer = WorldLoading.IsMultiplayer;
			this.CurrentTrackIndex = 0;
		}
		this.SelectTrack(this.Tracks[this.CurrentTrackIndex]);
	}

	public override void Open()
	{
		this.SubmitIsPressed = true;
		this.OnSelectTrackAction = null;
		base.Open();
	}

	private void Update()
	{
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
					this.NextTrack();
				}
				else
				{
					this.PrevTrack();
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
				this.SelectButton.onClick.Invoke();
			}
			this.SubmitIsPressed = true;
			return;
		}
		this.SubmitIsPressed = false;
	}

	protected override void OnSelect()
	{
		if (this.OnSelectTrackAction != null)
		{
			this.OnSelectTrackAction.SafeInvoke(this.CurrentTrackPreset);
			return;
		}
		WorldLoading.LoadingTrack = this.CurrentTrackPreset;
		Singleton<WindowsController>.Instance.OpenWindow(this.SelectCarkWindow);
	}

	private void NextTrack()
	{
		this.CurrentTrackIndex = MathExtentions.LoopClamp(this.CurrentTrackIndex + 1, 0, B.GameSettings.Tracks.Count);
		this.SelectTrack(this.Tracks[this.CurrentTrackIndex]);
	}

	private void PrevTrack()
	{
		this.CurrentTrackIndex = MathExtentions.LoopClamp(this.CurrentTrackIndex - 1, 0, B.GameSettings.Tracks.Count);
		this.SelectTrack(this.Tracks[this.CurrentTrackIndex]);
	}

	private void SelectTrack(TrackPreset track)
	{
		this.CurrentTrackPreset = track;
		base.RefreshButtonState(this.CurrentTrackPreset);
		Debug.Log("map3 : " + this.CurrentTrackPreset);
		this.TrackIcon.sprite = track.TrackIcon;
		this.TrackNameText.text = track.TrackName;
		this.LapCountText.text = string.Format("{0} {1}", track.LapsCount.ToString(), (track.LapsCount == 1) ? "Lap" : "Laps");
		this.RegimeText.text = track.RegimeSettings.RegimeCaption;
		this.RegimeImage.sprite = track.RegimeSettings.RegimeImage;
		WaypointCircuit getPathForVisual = track.GameController.PositioningSystem.GetPathForVisual;
		getPathForVisual.Awake();
		this.TrackPath.positionCount = (int)(getPathForVisual.Length / this.PathStep);
		float num = 0f;
		List<Vector3> list = new List<Vector3>();
		int num2 = 0;
		while (num2 < this.TrackPath.positionCount && num < getPathForVisual.Length)
		{
			WaypointCircuit.RoutePoint routePoint = getPathForVisual.GetRoutePoint(num);
			num += this.PathStep;
			Vector3 item;
			item..ctor(routePoint.position.x, routePoint.position.z, routePoint.position.y);
			list.Add(item);
			num2++;
		}
		float num3 = list.Max((Vector3 p) => p.x.Abs());
		float num4 = list.Max((Vector3 p) => p.y.Abs());
		float num5 = Mathf.Max(num3, num4);
		num5 = this.MaxRadius / num5;
		float num6 = list.Min((Vector3 p) => p.x);
		float num7 = list.Min((Vector3 p) => p.y);
		Vector3 vector;
		vector..ctor(num6, num7, 0f);
		for (int i = 0; i < this.TrackPath.positionCount; i++)
		{
			this.TrackPath.SetPosition(i, (list[i] - vector) * num5);
		}
	}

	[SerializeField]
	private Button NextTrackButton;

	[SerializeField]
	private Button PrevTrackButton;

	[SerializeField]
	private LineRenderer TrackPath;

	[Header("Path settings")]
	[SerializeField]
	private float MaxRadius = 1.5f;

	[SerializeField]
	private Window SelectCarkWindow;

	[SerializeField]
	private float PathStep = 10f;

	[Header("Track info")]
	[SerializeField]
	private Image TrackIcon;

	[SerializeField]
	private TextMeshProUGUI TrackNameText;

	[SerializeField]
	private TextMeshProUGUI LapCountText;

	[SerializeField]
	private TextMeshProUGUI RegimeText;

	[SerializeField]
	private Image RegimeImage;

	[Header("Animation")]
	[SerializeField]
	private Vector2 LeftPoint = new Vector2(-3840f, 0f);

	[SerializeField]
	private Vector2 RightPoint = new Vector2(3840f, 0f);

	[SerializeField]
	private float SpeedAnimation = 960f;

	private bool IsMultiplayer;

	private TrackPreset CurrentTrackPreset;

	private int CurrentTrackIndex;

	private bool SubmitIsPressed = true;

	private bool HorizontalIsPressed = true;

	private const string LapCaption = "Lap";

	private const string LapsCaption = "Laps";
}
