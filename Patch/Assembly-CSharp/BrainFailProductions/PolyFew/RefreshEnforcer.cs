﻿using System;
using UnityEngine;

namespace BrainFailProductions.PolyFew
{
	[ExecuteInEditMode]
	public class RefreshEnforcer : MonoBehaviour
	{
		private void Start()
		{
			Object.DestroyImmediate(this);
		}

		private void Update()
		{
		}
	}
}
