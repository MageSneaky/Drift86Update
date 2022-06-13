using System;
using UnityEngine;

public class PanelOpener : MonoBehaviour
{
	public void OpenPanel()
	{
		if (this.Panel != null)
		{
			bool activeSelf = this.Panel.activeSelf;
			this.Panel.SetActive(!activeSelf);
		}
	}

	public GameObject Panel;
}
