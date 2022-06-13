using System;
using UnityEngine;

public class resetPos : MonoBehaviour
{
	private void OnCollisionEnter(Collision other)
	{
		if (other.transform.tag == "Respawn")
		{
			Vector3 vector = base.transform.TransformDirection(Vector3.up);
			Debug.DrawRay(base.transform.position, vector * 100f, Color.red);
			RaycastHit raycastHit;
			if (Physics.Raycast(base.transform.position, vector, out raycastHit, 100f))
			{
				base.transform.position = new Vector3(base.transform.position.x, raycastHit.transform.position.y + 2f, base.transform.position.z);
			}
		}
	}
}
