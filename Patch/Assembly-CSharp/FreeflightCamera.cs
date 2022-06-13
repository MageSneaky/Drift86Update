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
			float y = base.transform.localEulerAngles.y + Input.GetAxis("Mouse X") * this.mouseSensitivityX;
			this.rotY += Input.GetAxis("Mouse Y") * this.mouseSensitivityY;
			this.rotY = Mathf.Clamp(this.rotY, -89.5f, 89.5f);
			base.transform.localEulerAngles = new Vector3(-this.rotY, y, 0f);
		}
		float axis = Input.GetAxis("Vertical");
		float axis2 = Input.GetAxis("Horizontal");
		if (axis != 0f)
		{
			float num = Input.GetKey(KeyCode.LeftShift) ? this.speedFast : this.speedNormal;
			Vector3 point = new Vector3(0f, 0f, axis * num * Time.deltaTime);
			base.gameObject.transform.localPosition += base.gameObject.transform.localRotation * point;
		}
		if (axis2 != 0f)
		{
			float num2 = Input.GetKey(KeyCode.LeftShift) ? this.speedFast : this.speedNormal;
			Vector3 point2 = new Vector3(axis2 * num2 * Time.deltaTime, 0f, 0f);
			base.gameObject.transform.localPosition += base.gameObject.transform.localRotation * point2;
		}
	}

	public float speedNormal = 10f;

	public float speedFast = 50f;

	public float mouseSensitivityX = 5f;

	public float mouseSensitivityY = 5f;

	private float rotY;
}
