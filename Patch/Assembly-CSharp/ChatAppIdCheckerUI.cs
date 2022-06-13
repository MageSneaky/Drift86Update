using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class ChatAppIdCheckerUI : MonoBehaviour
{
	public void Update()
	{
		if (string.IsNullOrEmpty(PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat))
		{
			if (this.Description != null)
			{
				this.Description.text = "<Color=Red>WARNING:</Color>\nPlease setup a Chat AppId in the PhotonServerSettings file.";
				return;
			}
		}
		else if (this.Description != null)
		{
			this.Description.text = string.Empty;
		}
	}

	public Text Description;
}
