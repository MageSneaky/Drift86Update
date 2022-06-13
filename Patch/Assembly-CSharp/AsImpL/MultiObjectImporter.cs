using System;
using System.Collections.Generic;
using UnityEngine;

namespace AsImpL
{
	public class MultiObjectImporter : ObjectImporter
	{
		public string RootPath
		{
			get
			{
				if (!(this.pathSettings != null))
				{
					return "";
				}
				return this.pathSettings.RootPath;
			}
		}

		public void ImportModelListAsync(ModelImportInfo[] modelsInfo)
		{
			if (modelsInfo == null)
			{
				return;
			}
			for (int i = 0; i < modelsInfo.Length; i++)
			{
				if (!modelsInfo[i].skip)
				{
					string name = modelsInfo[i].name;
					string text = modelsInfo[i].path;
					if (string.IsNullOrEmpty(text))
					{
						Debug.LogErrorFormat("File path missing for the model at position {0} in the list.", new object[]
						{
							i
						});
					}
					else
					{
						text = this.RootPath + text;
						ImportOptions loaderOptions = modelsInfo[i].loaderOptions;
						if (loaderOptions == null || loaderOptions.modelScaling == 0f)
						{
							loaderOptions = this.defaultImportOptions;
						}
						base.ImportModelAsync(name, text, base.transform, loaderOptions, "", "");
					}
				}
			}
		}

		protected virtual void Start()
		{
			if (this.autoLoadOnStart)
			{
				this.ImportModelListAsync(this.objectsList.ToArray());
			}
		}

		[Tooltip("Load models in the list on start")]
		public bool autoLoadOnStart;

		[Tooltip("Models to load on startup")]
		public List<ModelImportInfo> objectsList = new List<ModelImportInfo>();

		[Tooltip("Default import options")]
		public ImportOptions defaultImportOptions = new ImportOptions();

		[SerializeField]
		private PathSettings pathSettings;
	}
}
