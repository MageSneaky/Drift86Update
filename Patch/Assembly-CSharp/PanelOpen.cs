using System;
using UnityEngine;

public class PanelOpen : MonoBehaviour
{
	public void OpenPanel()
	{
		if (this.Panel != null)
		{
			this.Panel.SetActive(true);
		}
	}

	public GameObject Panel;
}
