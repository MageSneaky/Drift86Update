using System;
using System.IO;
using UnityEngine;

namespace AsImpL
{
	public class PathSettings : MonoBehaviour
	{
		public string RootPath
		{
			get
			{
				switch (this.defaultRootPath)
				{
				case RootPathEnum.DataPath:
					return Application.dataPath + "/";
				case RootPathEnum.DataPathParent:
					return Application.dataPath + "/../";
				case RootPathEnum.PersistentDataPath:
					return Application.persistentDataPath + "/";
				default:
					return "";
				}
			}
		}

		public static PathSettings FindPathComponent(GameObject obj)
		{
			PathSettings pathSettings = obj.GetComponent<PathSettings>();
			if (pathSettings == null)
			{
				pathSettings = Object.FindObjectOfType<PathSettings>();
			}
			if (pathSettings == null)
			{
				pathSettings = obj.AddComponent<PathSettings>();
			}
			return pathSettings;
		}

		public string FullPath(string path)
		{
			string result = path;
			if (!Path.IsPathRooted(path))
			{
				result = this.RootPath + path;
			}
			return result;
		}

		[Tooltip("Default root path for models")]
		public RootPathEnum defaultRootPath;

		[Tooltip("Root path for models on mobile devices")]
		public RootPathEnum mobileRootPath;
	}
}
