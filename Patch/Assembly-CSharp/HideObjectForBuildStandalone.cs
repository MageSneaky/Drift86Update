using System;
using UnityEngine;

public class HideObjectForBuildStandalone : MonoBehaviour
{
	private void Awake()
	{
		if (!Application.isMobilePlatform)
		{
			base.gameObject.SetActive(false);
		}
	}
}
