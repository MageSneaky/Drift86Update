using System;
using UnityEngine;

public class FreeflightCamera : MonoBehaviour
{
	private void Start()
	{
		if (base.GetComponent<Rigidbody>())
		{
			base.GetComponent<Rigidbody>().freezeRotation = true;
		}
	}

	private void Update()
	{
		if (Input.GetMouseButton(1))
		{
			float num = base.transform.localEulerAngles.y + Input.GetAxis("Mouse X") * this.mouseSensitivityX;
			this.rotY += Input.GetAxis("Mouse Y") * this.mouseSensitivityY;
			this.rotY = Mathf.Clamp(this.rotY, -89.5f, 89.5f);
			base.transform.localEulerAngles = new Vector3(-this.rotY, num, 0f);
		}
		float axis = Input.GetAxis("Vertical");
		float axis2 = Input.GetAxis("Horizontal");
		if (axis != 0f)
		{
			float num2 = Input.GetKey(304) ? this.speedFast : this.speedNormal;
			Vector3 vector;
			vector..ctor(0f, 0f, axis * num2 * Time.deltaTime);
			base.gameObject.transform.localPosition += base.gameObject.transform.localRotation * vector;
		}
		if (axis2 != 0f)
		{
			float num3 = Input.GetKey(304) ? this.speedFast : this.speedNormal;
			Vector3 vector2;
			vector2..ctor(axis2 * num3 * Time.deltaTime, 0f, 0f);
			base.gameObject.transform.localPosition += base.gameObject.transform.localRotation * vector2;
		}
	}

	private void OnGUI()
	{
		GUI.Label(new Rect(10f, 10f, 100f, 30f), "Working");
	}

	public float speedNormal = 10f;

	public float speedFast = 50f;

	public float mouseSensitivityX = 5f;

	public float mouseSensitivityY = 5f;

	private float rotY;
}
