using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using BrainFailProductions.PolyFewRuntime;
using UnityEngine;

namespace AsImpL
{
	public class ObjectImporter : MonoBehaviour
	{
		public ObjectImporter()
		{
			ObjectImporter.isException = false;
			ObjectImporter.downloadProgress = new PolyfewRuntime.ReferencedNumeric<float>(0f);
			ObjectImporter.objDownloadProgress = 0f;
			ObjectImporter.textureDownloadProgress = 0f;
			ObjectImporter.materialDownloadProgress = 0f;
			ObjectImporter.activeDownloads = 6;
		}

		public event Action ImportingStart;

		public event Action ImportingComplete;

		public event Action<GameObject, string> CreatedModel;

		public event Action<GameObject, string> ImportedModel;

		public event Action<string> ImportError;

		public int NumImportRequests
		{
			get
			{
				return this.numTotalImports;
			}
		}

		private Loader CreateLoader(string absolutePath, bool isNetwork = false)
		{
			if (isNetwork)
			{
				LoaderObj loaderObj = base.gameObject.AddComponent<LoaderObj>();
				loaderObj.ModelCreated += this.OnModelCreated;
				loaderObj.ModelLoaded += this.OnImported;
				loaderObj.ModelError += this.OnImportError;
				return loaderObj;
			}
			string text = Path.GetExtension(absolutePath);
			if (string.IsNullOrEmpty(text))
			{
				throw new InvalidOperationException("No extension defined, unable to detect file format. Please provide a full path to the file that ends with the file name including its extension.");
			}
			text = text.ToLower();
			Loader loader;
			if (text.StartsWith(".php"))
			{
				if (!text.EndsWith(".obj"))
				{
					throw new InvalidOperationException("Unable to detect file format in " + text);
				}
				loader = base.gameObject.AddComponent<LoaderObj>();
			}
			else
			{
				if (!(text == ".obj"))
				{
					throw new InvalidOperationException("File format not supported (" + text + ")");
				}
				loader = base.gameObject.AddComponent<LoaderObj>();
			}
			loader.ModelCreated += this.OnModelCreated;
			loader.ModelLoaded += this.OnImported;
			loader.ModelError += this.OnImportError;
			return loader;
		}

		public async Task<GameObject> ImportModelAsync(string objName, string filePath, Transform parentObj, ImportOptions options, string texturesFolderPath = "", string materialsFolderPath = "")
		{
			if (this.loaderList == null)
			{
				this.loaderList = new List<Loader>();
			}
			if (this.loaderList.Count == 0)
			{
				this.numTotalImports = 0;
				Action importingStart = this.ImportingStart;
				if (importingStart != null)
				{
					importingStart();
				}
			}
			string text = filePath.Contains("//") ? filePath : Path.GetFullPath(filePath);
			text = text.Replace('\\', '/');
			Loader loader = this.CreateLoader(text, false);
			if (loader == null)
			{
				throw new SystemException("Failed to import obj.");
			}
			this.numTotalImports++;
			this.loaderList.Add(loader);
			loader.buildOptions = options;
			if (string.IsNullOrEmpty(objName))
			{
				objName = Path.GetFileNameWithoutExtension(text);
			}
			this.allLoaded = false;
			return await loader.Load(objName, text, parentObj, texturesFolderPath, materialsFolderPath);
		}

		public async Task<GameObject> ImportModelFromNetwork(string objURL, string objName, string diffuseTexURL, string bumpTexURL, string specularTexURL, string opacityTexURL, string materialURL, PolyfewRuntime.ReferencedNumeric<float> downloadProgress, ImportOptions options)
		{
			if (this.loaderList == null)
			{
				this.loaderList = new List<Loader>();
			}
			if (this.loaderList.Count == 0)
			{
				this.numTotalImports = 0;
				Action importingStart = this.ImportingStart;
				if (importingStart != null)
				{
					importingStart();
				}
			}
			Loader loader = this.CreateLoader("", true);
			if (loader == null)
			{
				throw new SystemException("Failed to import obj.");
			}
			this.numTotalImports++;
			this.loaderList.Add(loader);
			loader.buildOptions = options;
			this.allLoaded = false;
			if (string.IsNullOrWhiteSpace(objName))
			{
				objName = "";
			}
			ObjectImporter.downloadProgress = downloadProgress;
			GameObject result;
			try
			{
				result = await loader.LoadFromNetwork(objURL, diffuseTexURL, bumpTexURL, specularTexURL, opacityTexURL, materialURL, objName);
			}
			catch (Exception ex)
			{
				throw ex;
			}
			return result;
		}

		public void ImportModelFromNetworkWebGL(string objURL, string objName, string diffuseTexURL, string bumpTexURL, string specularTexURL, string opacityTexURL, string materialURL, PolyfewRuntime.ReferencedNumeric<float> downloadProgress, ImportOptions options, Action<GameObject> OnSuccess, Action<Exception> OnError)
		{
			if (this.loaderList == null)
			{
				this.loaderList = new List<Loader>();
			}
			if (this.loaderList.Count == 0)
			{
				this.numTotalImports = 0;
				Action importingStart = this.ImportingStart;
				if (importingStart != null)
				{
					importingStart();
				}
			}
			Loader loader = this.CreateLoader("", true);
			if (loader == null)
			{
				OnError(new SystemException("Loader initialization failed due to unknown reasons."));
			}
			this.numTotalImports++;
			this.loaderList.Add(loader);
			loader.buildOptions = options;
			this.allLoaded = false;
			if (string.IsNullOrWhiteSpace(objName))
			{
				objName = "";
			}
			ObjectImporter.downloadProgress = downloadProgress;
			base.StartCoroutine(loader.LoadFromNetworkWebGL(objURL, diffuseTexURL, bumpTexURL, specularTexURL, opacityTexURL, materialURL, objName, OnSuccess, OnError));
		}

		public virtual void UpdateStatus()
		{
			if (this.allLoaded)
			{
				return;
			}
			if (this.numTotalImports - Loader.totalProgress.singleProgress.Count >= this.numTotalImports)
			{
				this.allLoaded = true;
				if (this.loaderList != null)
				{
					foreach (Loader obj in this.loaderList)
					{
						UnityEngine.Object.Destroy(obj);
					}
					this.loaderList.Clear();
				}
				this.OnImportingComplete();
			}
		}

		protected virtual void Update()
		{
			this.UpdateStatus();
		}

		protected virtual void OnImportingComplete()
		{
			if (this.ImportingComplete != null)
			{
				this.ImportingComplete();
			}
		}

		protected virtual void OnModelCreated(GameObject obj, string absolutePath)
		{
			if (this.CreatedModel != null)
			{
				this.CreatedModel(obj, absolutePath);
			}
		}

		protected virtual void OnImported(GameObject obj, string absolutePath)
		{
			if (this.ImportedModel != null)
			{
				this.ImportedModel(obj, absolutePath);
			}
		}

		protected virtual void OnImportError(string absolutePath)
		{
			if (this.ImportError != null)
			{
				this.ImportError(absolutePath);
			}
		}

		public static PolyfewRuntime.ReferencedNumeric<float> downloadProgress;

		public static int activeDownloads;

		private static float objDownloadProgress;

		private static float textureDownloadProgress;

		private static float materialDownloadProgress;

		public static bool isException;

		protected int numTotalImports;

		protected bool allLoaded;

		protected ImportOptions buildOptions;

		protected List<Loader> loaderList;

		private ObjectImporter.ImportPhase importPhase;

		private enum ImportPhase
		{
			Idle,
			TextureImport,
			ObjLoad,
			AssetBuild,
			Done
		}
	}
}
