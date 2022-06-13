using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CustomButton : Button
{
	public event Action<PointerEventData> onPointerDown;

	public event Action<PointerEventData> onPointerUp;

	public bool ButtonIsPressed
	{
		get
		{
			return base.IsPressed();
		}
	}

	protected override void Awake()
	{
		if (Application.isMobilePlatform)
		{
			Navigation navigation = base.navigation;
			navigation.mode = 0;
			base.navigation = navigation;
		}
		base.Awake();
	}

	protected override void OnEnable()
	{
		if (this.SelectedOnStart && base.navigation.mode != null)
		{
			base.StartCoroutine(this.DoSelect());
		}
		base.OnEnable();
	}

	private IEnumerator DoSelect()
	{
		yield return null;
		this.Select();
		yield break;
	}

	public override void OnPointerDown(PointerEventData eventData)
	{
		base.OnPointerDown(eventData);
		this.onPointerDown.SafeInvoke(eventData);
		if (this.EnableSounds)
		{
			SoundControllerInUI.PlayAudioClip(this.OnDownSound);
		}
	}

	public override void OnPointerUp(PointerEventData eventData)
	{
		base.OnPointerUp(eventData);
		this.onPointerUp.SafeInvoke(eventData);
		if (this.EnableSounds)
		{
			SoundControllerInUI.PlayAudioClip(this.OnUpSound);
		}
	}

	public override void OnPointerClick(PointerEventData eventData)
	{
		base.OnPointerClick(eventData);
		if (this.EnableSounds)
		{
			SoundControllerInUI.PlayAudioClip(this.OnClickSound);
		}
	}

	[SerializeField]
	private bool SelectedOnStart;

	[SerializeField]
	private bool EnableSounds;

	[SerializeField]
	private AudioClip OnClickSound;

	[SerializeField]
	private AudioClip OnDownSound;

	[SerializeField]
	private AudioClip OnUpSound;
}
