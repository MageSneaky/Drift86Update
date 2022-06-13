using System;
using UnityEngine;

namespace Photon.Chat.UtilityScripts
{
	public class OnStartDelete : MonoBehaviour
	{
		private void Start()
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}
}
