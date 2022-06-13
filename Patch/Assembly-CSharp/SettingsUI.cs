using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SettingsUI : WindowWithShowHideAnimators
{
	private void Start()
	{
		this.StartOther();
	}

	private void StartQualityDropDown()
	{
	}

	private void SetQuality(int newValue)
	{
		GameOptions.CurrentQuality = newValue;
	}

	public static event Action OnControlChanged;

	private void StartControlButtons()
	{
		this.ArrowsControlButton.onClick.AddListener(new UnityAction(this.OnArrowsControl));
		this.SteerWheelControlButton.onClick.AddListener(new UnityAction(this.OnSteerWheelControl));
		this.AccelerometrControlButton.onClick.AddListener(new UnityAction(this.OnAccelerometrControl));
		this.UpdateButtons();
	}

	private void UpdateButtons()
	{
		this.ArrowsControlButton.interactable = (GameOptions.CurrentControl > ControlType.Arrows);
		this.SteerWheelControlButton.interactable = (GameOptions.CurrentControl != ControlType.SteerWheel);
		this.AccelerometrControlButton.interactable = (GameOptions.CurrentControl != ControlType.Accelerometr);
	}

	private void OnArrowsControl()
	{
		GameOptions.CurrentControl = ControlType.Arrows;
		this.UpdateButtons();
	}

	private void OnSteerWheelControl()
	{
		GameOptions.CurrentControl = ControlType.SteerWheel;
		this.UpdateButtons();
	}

	private void OnAccelerometrControl()
	{
		GameOptions.CurrentControl = ControlType.Accelerometr;
		this.UpdateButtons();
	}

	private void StartSoundSettings()
	{
		this.MuteSoundToogle.isOn = GameOptions.SoundIsMute;
		this.MuteSoundToogle.onValueChanged.RemoveAllListeners();
		this.MuteSoundToogle.onValueChanged.AddListener(new UnityAction<bool>(this.OnChangeMute));
		this.MuteSoundToogle.onValueChanged.AddListener(delegate(bool value)
		{
			SoundControllerInUI.PlayAudioClip(this.ClickClip);
		});
	}

	private void OnChangeMute(bool value)
	{
		GameOptions.SoundIsMute = value;
	}

	private void StartOther()
	{
		if (this.EnableAiToogle)
		{
			this.EnableAiToogle.isOn = GameOptions.EnableAI;
			this.EnableAiToogle.onValueChanged.RemoveAllListeners();
			this.EnableAiToogle.onValueChanged.AddListener(new UnityAction<bool>(this.OnChangeAI));
		}
		if (this.ChangeNickNameButton != null && Singleton<ChangeNickName>.Instance != null)
		{
			this.ChangeNickNameButton.onClick.AddListener(new UnityAction(Singleton<ChangeNickName>.Instance.Show));
		}
	}

	private void OnChangeAI(bool value)
	{
		GameOptions.EnableAI = value;
	}

	[SerializeField]
	private AudioClip ClickClip;

	[SerializeField]
	private TMP_Dropdown QualityDropDown;

	[SerializeField]
	private Button ArrowsControlButton;

	[SerializeField]
	private Button SteerWheelControlButton;

	[SerializeField]
	private Button AccelerometrControlButton;

	[SerializeField]
	private Toggle MuteSoundToogle;

	[SerializeField]
	private Toggle EnableAiToogle;

	[SerializeField]
	private Button ChangeNickNameButton;

	private string nameField = "";
}
