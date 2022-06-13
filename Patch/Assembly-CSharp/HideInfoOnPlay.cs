using System;
using UnityEngine;

public class HideInfoOnPlay : MonoBehaviour
{
	private void Start()
	{
		base.gameObject.SetActive(false);
	}

	private void Update()
	{
	}
}
