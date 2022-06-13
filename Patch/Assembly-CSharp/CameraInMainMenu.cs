using System;
using System.Collections;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

public class CameraInMainMenu : Singleton<CameraInMainMenu>
{
	protected override void AwakeSingleton()
	{
		this.DefaultPosition = base.transform.position;
		this.DefaultRotation = base.transform.eulerAngles;
		if (Application.isMobilePlatform)
		{
			this.CameraBlur.enabled = false;
			return;
		}
		this.DefaultBlurIterations = this.CameraBlur.iterations;
		this.DefaultBlurSpread = this.CameraBlur.blurSpread;
	}

	public void SetCarSelectMenu(bool value)
	{
		this.InCarSelectMenu = value;
		base.StopAllCoroutines();
		if (this.InCarSelectMenu)
		{
			this.ChangePositionCoroutine = base.StartCoroutine(this.ChangePosition(this.PositionInSelectCarMenu.position, this.PositionInSelectCarMenu.eulerAngles));
		}
		else
		{
			this.ChangePositionCoroutine = base.StartCoroutine(this.ChangePosition(this.DefaultPosition, this.DefaultRotation));
		}
		if (!Application.isMobilePlatform)
		{
			base.StartCoroutine(this.SetActiveBlur(!this.InCarSelectMenu));
		}
	}

	private IEnumerator ChangePosition(Vector3 newPos, Vector3 newRot)
	{
		while (base.transform.position != newPos)
		{
			base.transform.position = Vector3.MoveTowards(base.transform.position, newPos, Time.deltaTime * this.ChangePositionSpeed);
			base.transform.eulerAngles = Vector3.Lerp(base.transform.eulerAngles, newRot, Time.deltaTime * this.ChangeRotationLerpSpeed);
			yield return null;
		}
		this.ChangePositionCoroutine = null;
		yield break;
	}

	private IEnumerator SetActiveBlur(bool active)
	{
		if (active)
		{
			this.CameraBlur.enabled = true;
		}
		float normolizeTime = (float)(active ? 0 : 1);
		float targetNormalize = 1f - normolizeTime;
		while (!Mathf.Approximately(normolizeTime, targetNormalize))
		{
			normolizeTime = Mathf.MoveTowards(normolizeTime, targetNormalize, Time.deltaTime * this.ChangeBlurSpeed);
			this.CameraBlur.iterations = Mathf.RoundToInt((float)this.DefaultBlurIterations * normolizeTime);
			this.CameraBlur.blurSpread = this.DefaultBlurSpread * normolizeTime;
			yield return null;
		}
		if (!active)
		{
			this.CameraBlur.enabled = false;
		}
		yield break;
	}

	private void Update()
	{
		if (this.InCarSelectMenu && this.ChangePositionCoroutine == null)
		{
			this.UpdateRotate();
			base.transform.rotation *= Quaternion.AngleAxis(this.CurrentSpeedRotate * Time.deltaTime, Vector3.up);
		}
	}

	private void UpdateRotate()
	{
		if ((!Application.isMobilePlatform || Input.touchCount == 1) && Input.GetMouseButtonDown(0))
		{
			this.ClickInEmptyPlace = true;
			this.PrevMousePos = Input.mousePosition;
		}
		else if (Input.touchCount == 0 && !Input.GetMouseButton(0))
		{
			this.ClickInEmptyPlace = false;
		}
		Vector3 vector = Input.mousePosition - this.PrevMousePos;
		this.PrevMousePos = Input.mousePosition;
		this.CurrentSpeedRotate = Mathf.MoveTowards(this.CurrentSpeedRotate, this.ClickInEmptyPlace ? (vector.x * 5f) : 0f, Time.deltaTime * this.RotateSensitivityInMenu * (float)(this.ClickInEmptyPlace ? 5 : 1));
	}

	private void OnDisable()
	{
		base.StopAllCoroutines();
		this.ChangePositionCoroutine = null;
	}

	[SerializeField]
	private Blur CameraBlur;

	[SerializeField]
	private Transform PositionInSelectCarMenu;

	[SerializeField]
	private float ChangePositionSpeed = 10f;

	[SerializeField]
	private float ChangeRotationLerpSpeed = 5f;

	[SerializeField]
	private float ChangeBlurSpeed = 0.5f;

	[SerializeField]
	private float RotateSensitivityInMenu = 5f;

	private Coroutine ChangePositionCoroutine;

	private Vector3 DefaultPosition;

	private Vector3 DefaultRotation;

	private bool InCarSelectMenu;

	private float CurrentSpeedRotate;

	private int DefaultBlurIterations;

	private float DefaultBlurSpread;

	private bool ClickInEmptyPlace;

	private Vector3 PrevMousePos;
}
