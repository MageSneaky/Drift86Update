using System;
using UnityEngine;

public class PanelOpen2 : MonoBehaviour
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
