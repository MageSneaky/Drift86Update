using System;
using UnityEngine;

public class FreeCamera : MonoBehaviour
{
	private void Update()
	{
		this.Velocity.z = Mathf.MoveTowards(this.Velocity.z, Input.GetAxis("Vertical") * this.MoveSpeed, this.MoveAccelerationSpeed * Time.deltaTime);
		this.Velocity.x = Mathf.MoveTowards(this.Velocity.x, Input.GetAxis("Horizontal") * this.MoveSpeed, this.MoveAccelerationSpeed * Time.deltaTime);
		this.Velocity.y = Mathf.MoveTowards(this.Velocity.y, (float)(Input.GetKey(KeyCode.E) ? 1 : (Input.GetKey(KeyCode.Q) ? -1 : 0)) * this.MoveSpeed * 0.3f, this.MoveAccelerationSpeed * Time.deltaTime);
		base.transform.position += base.transform.TransformDirection(this.Velocity);
		Input.mousePosition - this.MousePos;
		this.MousePos = Input.mousePosition;
		base.transform.rotation *= Quaternion.AngleAxis(Input.GetAxis("Mouse Y") * this.RotateSpeed, Vector3.left);
		base.transform.rotation *= Quaternion.AngleAxis(Input.GetAxis("Mouse X") * this.RotateSpeed, Vector3.up);
		Vector3 eulerAngles = base.transform.eulerAngles;
		eulerAngles.z = 0f;
		base.transform.eulerAngles = eulerAngles;
		if (Input.GetKey(KeyCode.LeftAlt))
		{
			Cursor.visible = true;
			Cursor.lockState = CursorLockMode.None;
		}
		else
		{
			Cursor.visible = false;
			Cursor.lockState = CursorLockMode.Locked;
		}
		Camera.main.fieldOfView = Mathf.Clamp(Camera.main.fieldOfView + Input.mouseScrollDelta.y, 5f, 150f);
	}

	[SerializeField]
	private float MoveSpeed;

	[SerializeField]
	private float MoveAccelerationSpeed;

	[SerializeField]
	private float RotateSpeed;

	private Vector3 Velocity;

	private Vector3 MousePos;
}
