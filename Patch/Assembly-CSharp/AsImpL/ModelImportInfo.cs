using System;
using UnityEngine;

namespace AsImpL
{
	[Serializable]
	public class ModelImportInfo
	{
		[Tooltip("Name for the game object created\n(leave it blank to use its file name)")]
		public string name;

		[Tooltip("Path relative to the project folder")]
		public string path;

		[Tooltip("Check this to skip this model")]
		public bool skip;

		public ImportOptions loaderOptions;
	}
}
