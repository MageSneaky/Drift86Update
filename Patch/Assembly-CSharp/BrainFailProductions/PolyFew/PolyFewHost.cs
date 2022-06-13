using System;
using UnityEngine;

namespace BrainFailProductions.PolyFew
{
	public class PolyFewHost : MonoBehaviour
	{
		private void Start()
		{
			if (!Application.isEditor || Application.isPlaying)
			{
				Object.DestroyImmediate(this);
			}
		}
	}
}
