using System;
using System.Collections.Generic;
using UnityEngine;

namespace SpielmannSpiel_Launcher
{
	public class DeactivateOnStart : MonoBehaviour
	{
		private void Start()
		{
			int count = this.toDeactivate.Count;
			for (int i = 0; i < count; i++)
			{
				if (this.toDeactivate[i] != null)
				{
					this.toDeactivate[i].SetActive(false);
				}
			}
		}

		public List<GameObject> toDeactivate;
	}
}
