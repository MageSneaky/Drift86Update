using System;
using UnityEngine;

public class SI_DDOL : MonoBehaviour
{
	private void Start()
	{
		if (GameObject.FindGameObjectsWithTag("SM").Length == 1)
		{
			UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
			return;
		}
		base.GetComponent<Transform>().gameObject.SetActive(false);
	}

	private void Update()
	{
	}
}
