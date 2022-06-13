using System;
using UnityEngine;

public class BucketWheelRotation : MonoBehaviour
{
	private void Start()
	{
		this.angle = 0f;
	}

	private void Update()
	{
		this.angle -= Time.deltaTime * this.speed * 25f;
		base.transform.localEulerAngles = new Vector3(this.angle, 0f, 0f);
	}

	public float speed = 1f;

	private float angle;
}
