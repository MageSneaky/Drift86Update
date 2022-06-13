using System;
using UnityEngine;

public class FlyCamera : MonoBehaviour
{
	private void Start()
	{
		Vector3 eulerAngles = base.transform.eulerAngles;
		this.x = eulerAngles.y;
		this.y = eulerAngles.x;
		this.rigidbody = base.GetComponent<Rigidbody>();
		if (this.rigidbody != null)
		{
			this.rigidbody.freezeRotation = true;
		}
	}

	private void Update()
	{
		if (FlyCamera.deactivated)
		{
			return;
		}
		if (this.target)
		{
			float num = Input.GetAxis("Mouse X");
			float num2 = Input.GetAxis("Mouse Y");
			if (!Input.GetMouseButton(0))
			{
				num = 0f;
				num2 = 0f;
			}
			this.x += num * this.xSpeed * this.distance * 0.02f;
			this.y -= num2 * this.ySpeed * 0.02f;
			this.y = FlyCamera.ClampAngle(this.y, this.yMinLimit, this.yMaxLimit);
			Quaternion rotation = Quaternion.Euler(this.y, this.x, 0f);
			this.distance = Mathf.Clamp(this.distance - Input.GetAxis("Mouse ScrollWheel") * 5f, this.distanceMin, this.distanceMax);
			RaycastHit raycastHit;
			if (Physics.Linecast(this.target.position, base.transform.position, out raycastHit))
			{
				this.distance -= raycastHit.distance;
			}
			Vector3 point = new Vector3(0f, 0f, -this.distance);
			Vector3 position = rotation * point + this.target.position;
			base.transform.position = position;
			base.transform.rotation = rotation;
		}
	}

	public static float ClampAngle(float angle, float min, float max)
	{
		if (angle < -360f)
		{
			angle += 360f;
		}
		if (angle > 360f)
		{
			angle -= 360f;
		}
		return Mathf.Clamp(angle, min, max);
	}

	public Transform target;

	public float distance = 5f;

	public float xSpeed = 120f;

	public float ySpeed = 120f;

	public float panSpeed = 0.05f;

	public float yMinLimit = -20f;

	public float yMaxLimit = 80f;

	public float distanceMin = 0.5f;

	public float distanceMax = 15f;

	private Rigidbody rigidbody;

	private float x;

	private float y;

	public static bool deactivated;
}
