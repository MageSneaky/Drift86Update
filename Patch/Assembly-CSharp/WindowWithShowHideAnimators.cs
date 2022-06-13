using System;
using System.Collections;
using Photon.Realtime;
using UnityEngine;

public class WindowWithShowHideAnimators : Window
{
	private Animator Animator
	{
		get
		{
			if (this._Animator == null)
			{
				this._Animator = base.GetComponent<Animator>();
			}
			return this._Animator;
		}
	}

	public override void Open()
	{
		base.gameObject.SetActive(true);
		this.StopCoroutine();
		this.ShowHideCoroutine = base.StartCoroutine(this.DoShowAnimation());
	}

	private IEnumerator DoShowAnimation()
	{
		if (!this.IsInitialized)
		{
			yield return new WaitForSecondsRealtime(this.DellayFirstShow);
			this.IsInitialized = true;
		}
		if (this.Animator)
		{
			this.Animator.SetTrigger("Show");
		}
		SoundControllerInUI.PlayAudioClip(this.ShowClip);
		this.ShowHideCoroutine = null;
		yield break;
	}

	public override void Close()
	{
		this.StopCoroutine();
		this.ShowHideCoroutine = base.StartCoroutine(this.DoHideAnimation());
	}

	private IEnumerator DoHideAnimation()
	{
		SoundControllerInUI.PlayAudioClip(this.HideClip);
		if (this.Animator)
		{
			this.Animator.SetTrigger("Hide");
		}
		yield return new WaitForSecondsRealtime(this.DellayToDesableGO);
		this.ShowHideCoroutine = null;
		base.gameObject.SetActive(false);
		yield break;
	}

	private void StopCoroutine()
	{
		if (this.ShowHideCoroutine != null)
		{
			base.StopCoroutine(this.ShowHideCoroutine);
			this.ShowHideCoroutine = null;
		}
	}

	internal void OnPlayerEnteredRoom(Player newPlayer)
	{
		throw new NotImplementedException();
	}

	[SerializeField]
	private float DellayFirstShow = 0.3f;

	[SerializeField]
	private float DellayToDesableGO = 0.3f;

	[SerializeField]
	private AudioClip ShowClip;

	[SerializeField]
	private AudioClip HideClip;

	private Coroutine ShowHideCoroutine;

	private bool IsInitialized;

	private Animator _Animator;
}
