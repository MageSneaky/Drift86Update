using System;
using UnityEngine;

public class GRCamRotate : MonoBehaviour
{
	private void Update()
	{
		base.transform.Rotate(Vector3.up * 0.3f);
	}
}
