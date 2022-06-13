using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SpielmannSpiel_Launcher
{
	public class ScreenHelper
	{
		public static List<ResolutionInfo> getResolutionInfos()
		{
			Resolution[] resolutions = Screen.resolutions;
			Dictionary<Vector2, ResolutionInfo> dictionary = new Dictionary<Vector2, ResolutionInfo>();
			int num = resolutions.Length;
			for (int i = 0; i < num; i++)
			{
				ResolutionInfo resolutionInfo = new ResolutionInfo(resolutions[i]);
				if (dictionary.ContainsKey(resolutionInfo.size))
				{
					dictionary[resolutionInfo.size] = resolutionInfo;
				}
				else
				{
					dictionary.Add(resolutionInfo.size, resolutionInfo);
				}
			}
			return dictionary.Values.Reverse<ResolutionInfo>().ToList<ResolutionInfo>();
		}

		public static InitialLauncherScreenSettings initialLauncherScreenSettings;
	}
}
