using System;

namespace SpielmannSpiel_Launcher
{
	[Serializable]
	public class FpsInfo
	{
		public override string ToString()
		{
			if (this.name != "")
			{
				return this.name;
			}
			return this.fps.ToString();
		}

		public string name = "";

		public int fps;
	}
}
