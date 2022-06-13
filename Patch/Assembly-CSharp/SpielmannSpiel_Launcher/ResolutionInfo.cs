using System;
using UnityEngine;

namespace SpielmannSpiel_Launcher
{
	public class ResolutionInfo
	{
		public ResolutionInfo(Resolution res)
		{
			this.resolution = res;
			this.size = new Vector2((float)res.width, (float)res.height);
			this.label = res.width + " x " + res.height;
		}

		public override string ToString()
		{
			return this.label;
		}

		public string label = "";

		public Vector2 size;

		public Resolution resolution;
	}
}
