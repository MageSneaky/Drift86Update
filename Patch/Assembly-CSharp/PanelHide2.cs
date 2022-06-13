using System;
using UnityEngine;

public class PanelHide2 : MonoBehaviour
{
	public void OpenPanel()
	{
		if (this.Panel != null)
		{
			this.Panel.SetActive(false);
		}
	}

	public GameObject Panel;
}
