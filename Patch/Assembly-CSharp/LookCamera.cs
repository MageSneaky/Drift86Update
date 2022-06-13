using System;
using UnityEngine;

public class LookCamera : MonoBehaviour
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
		if (Input.GetKey(117))
		{
			base.gameObject.transform.localPosition = new Vector3(0f, 3500f, 0f);
		}
	}

	public float speedNormal = 10f;

	public float speedFast = 50f;

	public float mouseSensitivityX = 5f;

	public float mouseSensitivityY = 5f;

	private float rotY;
}
