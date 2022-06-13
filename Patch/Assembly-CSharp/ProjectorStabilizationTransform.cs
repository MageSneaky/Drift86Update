using System;
using UnityEngine;

public class ProjectorStabilizationTransform : MonoBehaviour
{
	private void Start()
	{
		this.Car = base.GetComponentInParent<CarController>();
		if (this.Car == null)
		{
			Debug.LogError("CarController not found in parent");
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	private void Update()
	{
		Vector3 forward = this.Car.transform.TransformDirection(Vector3.forward);
		forward.y = 0f;
		base.transform.rotation = Quaternion.LookRotation(forward, Vector3.up);
	}

	private CarController Car;
}
