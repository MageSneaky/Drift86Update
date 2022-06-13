using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CarComponents : MonoBehaviour
{
	private void Start()
	{
		this.blink = true;
		this.frontLightsOn = true;
		this.brakeEffectsOn = true;
		this.indicatorLightOn = true;
		if (this.SpeedNeedle)
		{
			this.SpeedEulers = this.SpeedNeedle.localEulerAngles;
		}
		if (this.RpmNeedle)
		{
			this.RpmdEulers = this.RpmNeedle.localEulerAngles;
		}
		this.coroutine = this.WaitLights(2f);
		base.StartCoroutine(this.coroutine);
		this.coroutineIndicator = this.WaitIndicatorLights(0.5f);
		base.StartCoroutine(this.coroutineIndicator);
	}

	private void Update()
	{
		if (this.blink)
		{
			this.TurnOnFrontLights();
			this.TurnOnBackLights();
			this.TurnOnIndicatorLights();
		}
		if (this.SpeedNeedle)
		{
			Vector3 b = new Vector3(this.SpeedEulers.x, this.SpeedEulers.y, Mathf.Lerp(this.SpeedNeedleRotateRange.x, this.SpeedNeedleRotateRange.y, this.rotateNeedles));
			this.SpeedNeedle.localEulerAngles = Vector3.Lerp(this.SpeedNeedle.localEulerAngles, b, Time.deltaTime * this._NeedleSmoothing);
		}
		if (this.RpmNeedle)
		{
			Vector3 b2 = new Vector3(this.RpmdEulers.x, this.RpmdEulers.y, Mathf.Lerp(this.RpmNeedleRotateRange.x, this.RpmNeedleRotateRange.y, this.rotateNeedles));
			this.RpmNeedle.localEulerAngles = Vector3.Lerp(this.RpmNeedle.localEulerAngles, b2, Time.deltaTime * this._NeedleSmoothing);
		}
		if (this.steeringWheel != null)
		{
			Vector3 eulerAngles = this.steeringWheel.localRotation.eulerAngles;
			Vector3 eulerAngles2 = this.wheel_FL.localRotation.eulerAngles;
			eulerAngles.z = this.rotateNeedles * 15f;
			eulerAngles2.y = -this.rotateNeedles * 15f;
			this.steeringWheel.localRotation = Quaternion.Slerp(this.steeringWheel.localRotation, Quaternion.Euler(eulerAngles), Time.deltaTime * 2.5f);
			this.wheel_FL.localRotation = Quaternion.Slerp(this.wheel_FL.localRotation, Quaternion.Euler(eulerAngles2), Time.deltaTime * 2.5f);
			this.wheel_FR.localRotation = Quaternion.Slerp(this.wheel_FR.localRotation, Quaternion.Euler(eulerAngles2), Time.deltaTime * 2.5f);
		}
		if (this.txtSpeed && this.txtSpeed != null)
		{
			this.txtSpeed.text = ((int)(this.rotateNeedles * 100f)).ToString();
		}
		if (this.txtSpeed2 != null)
		{
			this.txtSpeed2.text = ((int)(this.rotateNeedles * 100f)).ToString();
		}
		if (this.txtRPM)
		{
			this.txtRPM.text = ((int)(this.rotateNeedles * 1000f)).ToString();
		}
		if (this.sliderRPM)
		{
			this.sliderRPM.value = this.rotateNeedles * 1000f;
		}
	}

	public void TurnOnFrontLights()
	{
		if (this.frontLightsOn)
		{
			this.frontLightEffects.SetActive(true);
			this.rotateNeedles += Time.deltaTime;
			return;
		}
		this.frontLightEffects.SetActive(false);
		this.rotateNeedles -= Time.deltaTime;
	}

	public void TurnOnBackLights()
	{
		if (this.brakeEffectsOn)
		{
			this.brakeEffects.SetActive(true);
			return;
		}
		this.brakeEffects.SetActive(false);
	}

	private IEnumerator WaitLights(float waitTime)
	{
		for (;;)
		{
			yield return new WaitForSeconds(waitTime);
			this.frontLightsOn = !this.frontLightsOn;
			this.brakeEffectsOn = !this.brakeEffectsOn;
		}
		yield break;
	}

	private IEnumerator WaitIndicatorLights(float waitTime)
	{
		for (;;)
		{
			yield return new WaitForSeconds(waitTime);
			this.indicatorLightOn = !this.indicatorLightOn;
		}
		yield break;
	}

	public void TurnOnIndicatorLights()
	{
		if (this.indicatorLightOn)
		{
			this.indicatorR.SetActive(true);
			this.indicatorL.SetActive(true);
			return;
		}
		this.indicatorR.SetActive(false);
		this.indicatorL.SetActive(false);
	}

	public bool blink;

	[Header("Lights")]
	public bool frontLightsOn;

	public bool brakeEffectsOn;

	public bool indicatorLightOn;

	[Space(5f)]
	public GameObject brakeEffects;

	public GameObject frontLightEffects;

	public GameObject reverseEffect;

	public GameObject indicatorR;

	public GameObject indicatorL;

	[Header("Needles")]
	public Transform SpeedNeedle;

	public Vector2 SpeedNeedleRotateRange = Vector3.zero;

	private Vector3 SpeedEulers = Vector3.zero;

	public Transform RpmNeedle;

	public Vector2 RpmNeedleRotateRange = Vector3.zero;

	private Vector3 RpmdEulers = Vector3.zero;

	public float _NeedleSmoothing = 1f;

	public Transform steeringWheel;

	private float rotateNeedles;

	[Header("Wheels")]
	public Transform wheel_FR;

	public Transform wheel_FL;

	[Header("Panel Texts")]
	public Text txtSpeed;

	[Header("Panel Texts")]
	public Text txtRPM;

	[Header("Panel Texts")]
	public Text txtSpeed2;

	public Slider sliderRPM;

	private IEnumerator coroutine;

	private IEnumerator coroutineIndicator;
}
