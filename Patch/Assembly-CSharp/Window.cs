using System;
using UnityEngine;

public abstract class Window : MonoBehaviour
{
	private void OnEnable()
	{
		this.OnEnableAction.SafeInvoke();
	}

	private void OnDisable()
	{
		this.OnDisableAction.SafeInvoke();
	}

	protected virtual void Awake()
	{
	}

	public abstract void Open();

	public abstract void Close();

	public Action OnEnableAction;

	public Action OnDisableAction;

	public Action CustomBackAction;
}
