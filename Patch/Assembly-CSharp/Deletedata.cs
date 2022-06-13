using System;
using UnityEngine;

public class Deletedata : MonoBehaviour
{
	private void Start()
	{
	}

	public void DeleteAll()
	{
		PlayerPrefs.DeleteAll();
	}

	private void Update()
	{
	}
}
