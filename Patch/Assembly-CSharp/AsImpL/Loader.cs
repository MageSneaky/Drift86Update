using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using BrainFailProductions.PolyFewRuntime;
using UnityEngine;
using UnityEngine.Networking;

namespace AsImpL
{
	public abstract class Loader : MonoBehaviour
	{
		public bool ConvertVertAxis
		{
			get
			{
				return this.buildOptions != null && this.buildOptions.zUp;
			}
			set
			{
				if (this.buildOptions == null)
				{
					this.buildOptions = new ImportOptions();
				}
				this.buildOptions.zUp = value;
			}
		}

		public float Scaling
		{
			get
			{
				if (this.buildOptions == null)
				{
					return 1f;
				}
				return this.buildOptions.modelScaling;
			}
			set
			{
				if (this.buildOptions == null)
				{
					this.buildOptions = new ImportOptions();
				}
				this.buildOptions.modelScaling = value;
			}
		}

		protected abstract bool HasMaterialLibrary { get; }

		public event Action<GameObject, string> ModelCreated;

		public event Action<GameObject, string> ModelLoaded;

		public event Action<string> ModelError;

		public static GameObject GetModelByPath(string absolutePath)
		{
			if (Loader.loadedModels.ContainsKey(absolutePath))
			{
				return Loader.loadedModels[absolutePath];
			}
			return null;
		}

		public async Task<GameObject> Load(string objName, string objAbsolutePath, Transform parentObj, string texturesFolderPath = "", string materialsFolderPath = "")
		{
			string fileName = Path.GetFileName(objAbsolutePath);
			string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(objAbsolutePath);
			string text = objName;
			if (text == null || text == "")
			{
				objName = fileNameWithoutExtension;
			}
			Loader.totalProgress.singleProgress.Add(this.objLoadingProgress);
			this.objLoadingProgress.fileName = fileName;
			this.objLoadingProgress.error = false;
			this.objLoadingProgress.message = "Loading " + fileName + "...";
			await Task.Yield();
			Loader.loadedModels[objAbsolutePath] = null;
			Loader.instanceCount[objAbsolutePath] = 0;
			float lastTime = Time.realtimeSinceStartup;
			float startTime = lastTime;
			await this.LoadModelFile(objAbsolutePath, texturesFolderPath, materialsFolderPath);
			this.loadStats.modelParseTime = Time.realtimeSinceStartup - lastTime;
			GameObject result;
			if (this.objLoadingProgress.error)
			{
				this.OnLoadFailed(objAbsolutePath);
				result = null;
			}
			else
			{
				lastTime = Time.realtimeSinceStartup;
				if (this.HasMaterialLibrary)
				{
					await this.LoadMaterialLibrary(objAbsolutePath, materialsFolderPath);
				}
				this.loadStats.materialsParseTime = Time.realtimeSinceStartup - lastTime;
				lastTime = Time.realtimeSinceStartup;
				await this.Build(objAbsolutePath, objName, parentObj, texturesFolderPath);
				this.loadStats.buildTime = Time.realtimeSinceStartup - lastTime;
				this.loadStats.totalTime = Time.realtimeSinceStartup - startTime;
				Loader.totalProgress.singleProgress.Remove(this.objLoadingProgress);
				this.OnLoaded(Loader.loadedModels[objAbsolutePath], objAbsolutePath);
				result = Loader.loadedModels[objAbsolutePath];
			}
			return result;
		}

		public async Task<GameObject> LoadFromNetwork(string objURL, string diffuseTexURL, string bumpTexURL, string specularTexURL, string opacityTexURL, string materialURL, string objName)
		{
			string text = objName + ".obj";
			Loader.totalProgress.singleProgress.Add(this.objLoadingProgress);
			this.objLoadingProgress.fileName = text;
			this.objLoadingProgress.error = false;
			this.objLoadingProgress.message = "Loading " + text + "...";
			await Task.Yield();
			Loader.loadedModels[objURL] = null;
			Loader.instanceCount[objURL] = 0;
			float lastTime = Time.realtimeSinceStartup;
			float startTime = lastTime;
			try
			{
				await this.LoadModelFileNetworked(objURL);
			}
			catch (Exception ex)
			{
				throw ex;
			}
			this.loadStats.modelParseTime = Time.realtimeSinceStartup - lastTime;
			GameObject result;
			if (this.objLoadingProgress.error)
			{
				this.OnLoadFailed(objURL);
				result = null;
			}
			else
			{
				lastTime = Time.realtimeSinceStartup;
				if (this.HasMaterialLibrary)
				{
					try
					{
						await this.LoadMaterialLibrary(materialURL);
						goto IL_245;
					}
					catch (Exception ex2)
					{
						throw ex2;
					}
				}
				ObjectImporter.activeDownloads--;
				IL_245:
				this.loadStats.materialsParseTime = Time.realtimeSinceStartup - lastTime;
				lastTime = Time.realtimeSinceStartup;
				try
				{
					await this.NetworkedBuild(null, objName, objURL, diffuseTexURL, bumpTexURL, specularTexURL, opacityTexURL);
				}
				catch (Exception ex3)
				{
					throw ex3;
				}
				this.loadStats.buildTime = Time.realtimeSinceStartup - lastTime;
				this.loadStats.totalTime = Time.realtimeSinceStartup - startTime;
				Loader.totalProgress.singleProgress.Remove(this.objLoadingProgress);
				this.OnLoaded(Loader.loadedModels[objURL], objURL);
				result = Loader.loadedModels[objURL];
			}
			return result;
		}

		public IEnumerator LoadFromNetworkWebGL(string objURL, string diffuseTexURL, string bumpTexURL, string specularTexURL, string opacityTexURL, string materialURL, string objName, Action<GameObject> OnSuccess, Action<Exception> OnError)
		{
			string text = objName + ".obj";
			Loader.totalProgress.singleProgress.Add(this.objLoadingProgress);
			this.objLoadingProgress.fileName = text;
			this.objLoadingProgress.error = false;
			this.objLoadingProgress.message = "Loading " + text + "...";
			Loader.loadedModels[objURL] = null;
			Loader.instanceCount[objURL] = 0;
			float lastTime = Time.realtimeSinceStartup;
			float startTime = lastTime;
			yield return base.StartCoroutine(this.LoadModelFileNetworkedWebGL(objURL, OnError));
			if (ObjectImporter.isException)
			{
				yield return null;
			}
			this.loadStats.modelParseTime = Time.realtimeSinceStartup - lastTime;
			if (this.objLoadingProgress.error)
			{
				this.OnLoadFailed(objURL);
				OnError(new Exception("Load failed due to unknown reasons."));
				yield return null;
			}
			lastTime = Time.realtimeSinceStartup;
			if (this.HasMaterialLibrary)
			{
				yield return base.StartCoroutine(this.LoadMaterialLibraryWebGL(materialURL));
			}
			else
			{
				ObjectImporter.activeDownloads--;
			}
			if (ObjectImporter.isException)
			{
				yield return null;
			}
			this.loadStats.materialsParseTime = Time.realtimeSinceStartup - lastTime;
			lastTime = Time.realtimeSinceStartup;
			yield return base.StartCoroutine(this.NetworkedBuildWebGL(null, objName, objURL, diffuseTexURL, bumpTexURL, specularTexURL, opacityTexURL));
			if (ObjectImporter.isException)
			{
				yield return null;
			}
			this.loadStats.buildTime = Time.realtimeSinceStartup - lastTime;
			this.loadStats.totalTime = Time.realtimeSinceStartup - startTime;
			Loader.totalProgress.singleProgress.Remove(this.objLoadingProgress);
			this.OnLoaded(Loader.loadedModels[objURL], objURL);
			OnSuccess(Loader.loadedModels[objURL]);
			yield break;
		}

		public abstract string[] ParseTexturePaths(string absolutePath);

		protected abstract Task LoadModelFile(string absolutePath, string texturesFolderPath = "", string materialsFolderPath = "");

		protected abstract Task LoadModelFileNetworked(string objURL);

		protected abstract IEnumerator LoadModelFileNetworkedWebGL(string objURL, Action<Exception> OnError);

		protected abstract Task LoadMaterialLibrary(string absolutePath, string materialsFolderPath = "");

		protected abstract Task LoadMaterialLibrary(string materialURL);

		protected abstract IEnumerator LoadMaterialLibraryWebGL(string materialURL);

		protected async Task Build(string absolutePath, string objName, Transform parentTransform, string texturesFolderPath = "")
		{
			float prevTime = Time.realtimeSinceStartup;
			if (this.materialData != null)
			{
				string basePath = Path.GetDirectoryName(absolutePath);
				this.objLoadingProgress.message = "Loading textures...";
				int count = 0;
				foreach (MaterialData mtl in this.materialData)
				{
					this.objLoadingProgress.percentage = Loader.LOAD_PHASE_PERC + Loader.TEXTURE_PHASE_PERC * (float)count / (float)this.materialData.Count;
					int num = count;
					count = num + 1;
					if (mtl.diffuseTexPath != null)
					{
						await this.LoadMaterialTexture(basePath, mtl.diffuseTexPath, texturesFolderPath);
						mtl.diffuseTex = this.loadedTexture;
					}
					if (mtl.bumpTexPath != null)
					{
						await this.LoadMaterialTexture(basePath, mtl.bumpTexPath, texturesFolderPath);
						mtl.bumpTex = this.loadedTexture;
					}
					if (mtl.specularTexPath != null)
					{
						await this.LoadMaterialTexture(basePath, mtl.specularTexPath, texturesFolderPath);
						mtl.specularTex = this.loadedTexture;
					}
					if (mtl.opacityTexPath != null)
					{
						await this.LoadMaterialTexture(basePath, mtl.opacityTexPath, texturesFolderPath);
						mtl.opacityTex = this.loadedTexture;
					}
					mtl = null;
				}
				List<MaterialData>.Enumerator enumerator = default(List<MaterialData>.Enumerator);
				basePath = null;
			}
			this.loadStats.buildStats.texturesTime = Time.realtimeSinceStartup - prevTime;
			prevTime = Time.realtimeSinceStartup;
			ObjectBuilder.ProgressInfo progressInfo = new ObjectBuilder.ProgressInfo();
			this.objLoadingProgress.message = "Loading materials...";
			this.objectBuilder.buildOptions = this.buildOptions;
			bool hasColors = this.dataSet.colorList.Count > 0;
			bool flag = this.materialData != null;
			this.objectBuilder.InitBuildMaterials(this.materialData, hasColors);
			float percentage = this.objLoadingProgress.percentage;
			if (flag)
			{
				while (this.objectBuilder.BuildMaterials(progressInfo))
				{
					this.objLoadingProgress.percentage = percentage + Loader.MATERIAL_PHASE_PERC * (float)this.objectBuilder.NumImportedMaterials / (float)this.materialData.Count;
				}
				this.loadStats.buildStats.materialsTime = Time.realtimeSinceStartup - prevTime;
				prevTime = Time.realtimeSinceStartup;
			}
			this.objLoadingProgress.message = "Building scene objects...";
			GameObject gameObject = new GameObject(objName);
			if (this.buildOptions.hideWhileLoading)
			{
				gameObject.SetActive(false);
			}
			if (parentTransform != null)
			{
				gameObject.transform.SetParent(parentTransform.transform, false);
			}
			this.OnCreated(gameObject, absolutePath);
			float percentage2 = this.objLoadingProgress.percentage;
			this.objectBuilder.StartBuildObjectAsync(this.dataSet, gameObject, null);
			while (this.objectBuilder.BuildObjectAsync(ref progressInfo))
			{
				this.objLoadingProgress.message = string.Concat(new object[]
				{
					"Building scene objects... ",
					progressInfo.objectsLoaded + progressInfo.groupsLoaded,
					"/",
					this.dataSet.objectList.Count + progressInfo.numGroups
				});
				this.objLoadingProgress.percentage = percentage2 + Loader.BUILD_PHASE_PERC * ((float)(progressInfo.objectsLoaded / this.dataSet.objectList.Count) + (float)progressInfo.groupsLoaded / (float)progressInfo.numGroups);
			}
			this.objLoadingProgress.percentage = 100f;
			Loader.loadedModels[absolutePath] = gameObject;
			this.loadStats.buildStats.objectsTime = Time.realtimeSinceStartup - prevTime;
		}

		protected async Task NetworkedBuild(Transform parentTransform, string objName, string objURL, string diffuseTexURL, string bumpTexURL, string specularTexURL, string opacityTexURL)
		{
			float prevTime = Time.realtimeSinceStartup;
			if (this.materialData != null)
			{
				this.objLoadingProgress.message = "Loading textures...";
				int count = 0;
				foreach (MaterialData mtl in this.materialData)
				{
					this.objLoadingProgress.percentage = Loader.LOAD_PHASE_PERC + Loader.TEXTURE_PHASE_PERC * (float)count / (float)this.materialData.Count;
					int num = count;
					count = num + 1;
					if (mtl.diffuseTexPath != null)
					{
						if (!string.IsNullOrWhiteSpace(diffuseTexURL))
						{
							try
							{
								await this.LoadMaterialTexture(diffuseTexURL);
								goto IL_166;
							}
							catch (Exception ex)
							{
								throw ex;
							}
							goto IL_15A;
						}
						goto IL_15A;
						IL_166:
						mtl.diffuseTex = this.loadedTexture;
						goto IL_185;
						IL_15A:
						ObjectImporter.activeDownloads--;
						goto IL_166;
					}
					ObjectImporter.activeDownloads--;
					IL_185:
					ObjectImporter.downloadProgress.Value = this.individualProgress.Value / (float)ObjectImporter.activeDownloads * 100f;
					if (mtl.bumpTexPath != null)
					{
						if (!string.IsNullOrWhiteSpace(bumpTexURL))
						{
							try
							{
								await this.LoadMaterialTexture(bumpTexURL);
								goto IL_23C;
							}
							catch (Exception ex2)
							{
								throw ex2;
							}
							goto IL_230;
						}
						goto IL_230;
						IL_23C:
						mtl.bumpTex = this.loadedTexture;
						goto IL_25B;
						IL_230:
						ObjectImporter.activeDownloads--;
						goto IL_23C;
					}
					ObjectImporter.activeDownloads--;
					IL_25B:
					ObjectImporter.downloadProgress.Value = this.individualProgress.Value / (float)ObjectImporter.activeDownloads * 100f;
					if (mtl.specularTexPath != null)
					{
						if (!string.IsNullOrWhiteSpace(specularTexURL))
						{
							try
							{
								await this.LoadMaterialTexture(specularTexURL);
								goto IL_312;
							}
							catch (Exception ex3)
							{
								throw ex3;
							}
							goto IL_306;
						}
						goto IL_306;
						IL_312:
						mtl.specularTex = this.loadedTexture;
						goto IL_331;
						IL_306:
						ObjectImporter.activeDownloads--;
						goto IL_312;
					}
					ObjectImporter.activeDownloads--;
					IL_331:
					ObjectImporter.downloadProgress.Value = this.individualProgress.Value / (float)ObjectImporter.activeDownloads * 100f;
					if (mtl.opacityTexPath != null)
					{
						if (!string.IsNullOrWhiteSpace(opacityTexURL))
						{
							try
							{
								await this.LoadMaterialTexture(opacityTexURL);
								goto IL_3E8;
							}
							catch (Exception ex4)
							{
								throw ex4;
							}
							goto IL_3DC;
						}
						goto IL_3DC;
						IL_3E8:
						mtl.opacityTex = this.loadedTexture;
						goto IL_407;
						IL_3DC:
						ObjectImporter.activeDownloads--;
						goto IL_3E8;
					}
					ObjectImporter.activeDownloads--;
					IL_407:
					ObjectImporter.downloadProgress.Value = this.individualProgress.Value / (float)ObjectImporter.activeDownloads * 100f;
					mtl = null;
				}
				List<MaterialData>.Enumerator enumerator = default(List<MaterialData>.Enumerator);
			}
			this.loadStats.buildStats.texturesTime = Time.realtimeSinceStartup - prevTime;
			prevTime = Time.realtimeSinceStartup;
			ObjectBuilder.ProgressInfo info = new ObjectBuilder.ProgressInfo();
			this.objLoadingProgress.message = "Loading materials...";
			this.objectBuilder.buildOptions = this.buildOptions;
			bool hasColors = this.dataSet.colorList.Count > 0;
			bool flag = this.materialData != null;
			this.objectBuilder.InitBuildMaterials(this.materialData, hasColors);
			float objInitPerc = this.objLoadingProgress.percentage;
			if (flag)
			{
				while (this.objectBuilder.BuildMaterials(info))
				{
					this.objLoadingProgress.percentage = objInitPerc + Loader.MATERIAL_PHASE_PERC * (float)this.objectBuilder.NumImportedMaterials / (float)this.materialData.Count;
					await Task.Delay(0);
				}
				this.loadStats.buildStats.materialsTime = Time.realtimeSinceStartup - prevTime;
				prevTime = Time.realtimeSinceStartup;
			}
			this.objLoadingProgress.message = "Building scene objects...";
			GameObject newObj = new GameObject(objName);
			if (this.buildOptions.hideWhileLoading)
			{
				newObj.SetActive(false);
			}
			if (parentTransform != null)
			{
				newObj.transform.SetParent(parentTransform.transform, false);
			}
			this.OnCreated(newObj, objURL);
			float initProgress = this.objLoadingProgress.percentage;
			this.objectBuilder.StartBuildObjectAsync(this.dataSet, newObj, null);
			while (this.objectBuilder.BuildObjectAsync(ref info))
			{
				this.objLoadingProgress.message = string.Concat(new object[]
				{
					"Building scene objects... ",
					info.objectsLoaded + info.groupsLoaded,
					"/",
					this.dataSet.objectList.Count + info.numGroups
				});
				this.objLoadingProgress.percentage = initProgress + Loader.BUILD_PHASE_PERC * ((float)(info.objectsLoaded / this.dataSet.objectList.Count) + (float)info.groupsLoaded / (float)info.numGroups);
				await Task.Delay(0);
			}
			this.objLoadingProgress.percentage = 100f;
			Loader.loadedModels[objURL] = newObj;
			this.loadStats.buildStats.objectsTime = Time.realtimeSinceStartup - prevTime;
		}

		protected IEnumerator NetworkedBuildWebGL(Transform parentTransform, string objName, string objURL, string diffuseTexURL, string bumpTexURL, string specularTexURL, string opacityTexURL)
		{
			float prevTime = Time.realtimeSinceStartup;
			if (this.materialData != null)
			{
				this.objLoadingProgress.message = "Loading textures...";
				int count = 0;
				foreach (MaterialData mtl in this.materialData)
				{
					this.objLoadingProgress.percentage = Loader.LOAD_PHASE_PERC + Loader.TEXTURE_PHASE_PERC * (float)count / (float)this.materialData.Count;
					int num = count;
					count = num + 1;
					if (mtl.diffuseTexPath != null)
					{
						if (!string.IsNullOrWhiteSpace(diffuseTexURL))
						{
							yield return base.StartCoroutine(this.LoadMaterialTextureWebGL(diffuseTexURL));
						}
						else
						{
							ObjectImporter.activeDownloads--;
						}
						mtl.diffuseTex = this.loadedTexture;
					}
					else
					{
						ObjectImporter.activeDownloads--;
					}
					ObjectImporter.downloadProgress.Value = this.individualProgress.Value / (float)ObjectImporter.activeDownloads * 100f;
					if (mtl.bumpTexPath != null)
					{
						if (!string.IsNullOrWhiteSpace(bumpTexURL))
						{
							yield return base.StartCoroutine(this.LoadMaterialTextureWebGL(bumpTexURL));
						}
						else
						{
							ObjectImporter.activeDownloads--;
						}
						mtl.bumpTex = this.loadedTexture;
					}
					else
					{
						ObjectImporter.activeDownloads--;
					}
					ObjectImporter.downloadProgress.Value = this.individualProgress.Value / (float)ObjectImporter.activeDownloads * 100f;
					if (mtl.specularTexPath != null)
					{
						if (!string.IsNullOrWhiteSpace(specularTexURL))
						{
							yield return base.StartCoroutine(this.LoadMaterialTextureWebGL(specularTexURL));
						}
						else
						{
							ObjectImporter.activeDownloads--;
						}
						mtl.specularTex = this.loadedTexture;
					}
					else
					{
						ObjectImporter.activeDownloads--;
					}
					ObjectImporter.downloadProgress.Value = this.individualProgress.Value / (float)ObjectImporter.activeDownloads * 100f;
					if (mtl.opacityTexPath != null)
					{
						if (!string.IsNullOrWhiteSpace(opacityTexURL))
						{
							yield return base.StartCoroutine(this.LoadMaterialTextureWebGL(opacityTexURL));
						}
						else
						{
							ObjectImporter.activeDownloads--;
						}
						mtl.opacityTex = this.loadedTexture;
					}
					else
					{
						ObjectImporter.activeDownloads--;
					}
					ObjectImporter.downloadProgress.Value = this.individualProgress.Value / (float)ObjectImporter.activeDownloads * 100f;
					mtl = null;
				}
				List<MaterialData>.Enumerator enumerator = default(List<MaterialData>.Enumerator);
			}
			this.loadStats.buildStats.texturesTime = Time.realtimeSinceStartup - prevTime;
			prevTime = Time.realtimeSinceStartup;
			ObjectBuilder.ProgressInfo progressInfo = new ObjectBuilder.ProgressInfo();
			this.objLoadingProgress.message = "Loading materials...";
			this.objectBuilder.buildOptions = this.buildOptions;
			bool hasColors = this.dataSet.colorList.Count > 0;
			bool flag = this.materialData != null;
			this.objectBuilder.InitBuildMaterials(this.materialData, hasColors);
			float percentage = this.objLoadingProgress.percentage;
			if (flag)
			{
				while (this.objectBuilder.BuildMaterials(progressInfo))
				{
					this.objLoadingProgress.percentage = percentage + Loader.MATERIAL_PHASE_PERC * (float)this.objectBuilder.NumImportedMaterials / (float)this.materialData.Count;
				}
				this.loadStats.buildStats.materialsTime = Time.realtimeSinceStartup - prevTime;
				prevTime = Time.realtimeSinceStartup;
			}
			this.objLoadingProgress.message = "Building scene objects...";
			GameObject gameObject = new GameObject(objName);
			if (this.buildOptions.hideWhileLoading)
			{
				gameObject.SetActive(false);
			}
			if (parentTransform != null)
			{
				gameObject.transform.SetParent(parentTransform.transform, false);
			}
			this.OnCreated(gameObject, objURL);
			float percentage2 = this.objLoadingProgress.percentage;
			this.objectBuilder.StartBuildObjectAsync(this.dataSet, gameObject, null);
			while (this.objectBuilder.BuildObjectAsync(ref progressInfo))
			{
				this.objLoadingProgress.message = string.Concat(new object[]
				{
					"Building scene objects... ",
					progressInfo.objectsLoaded + progressInfo.groupsLoaded,
					"/",
					this.dataSet.objectList.Count + progressInfo.numGroups
				});
				this.objLoadingProgress.percentage = percentage2 + Loader.BUILD_PHASE_PERC * ((float)(progressInfo.objectsLoaded / this.dataSet.objectList.Count) + (float)progressInfo.groupsLoaded / (float)progressInfo.numGroups);
			}
			this.objLoadingProgress.percentage = 100f;
			Loader.loadedModels[objURL] = gameObject;
			this.loadStats.buildStats.objectsTime = Time.realtimeSinceStartup - prevTime;
			yield break;
			yield break;
		}

		protected string GetDirName(string absolutePath)
		{
			string text;
			if (absolutePath.Contains("//"))
			{
				text = absolutePath.Remove(absolutePath.LastIndexOf('/') + 1);
			}
			else
			{
				string directoryName = Path.GetDirectoryName(absolutePath);
				text = (string.IsNullOrEmpty(directoryName) ? "" : directoryName);
				if (!text.EndsWith("/"))
				{
					text += "/";
				}
			}
			return text;
		}

		protected virtual void OnLoaded(GameObject obj, string absolutePath)
		{
			if (obj == null)
			{
				if (this.ModelError != null)
				{
					this.ModelError(absolutePath);
					return;
				}
			}
			else
			{
				if (this.buildOptions != null)
				{
					obj.transform.localPosition = this.buildOptions.localPosition;
					obj.transform.localRotation = Quaternion.Euler(this.buildOptions.localEulerAngles);
					obj.transform.localScale = this.buildOptions.localScale;
					if (this.buildOptions.inheritLayer)
					{
						obj.layer = obj.transform.parent.gameObject.layer;
						MeshRenderer[] componentsInChildren = obj.transform.GetComponentsInChildren<MeshRenderer>(true);
						for (int i = 0; i < componentsInChildren.Length; i++)
						{
							componentsInChildren[i].gameObject.layer = obj.transform.parent.gameObject.layer;
						}
					}
				}
				if (this.buildOptions.hideWhileLoading)
				{
					obj.SetActive(true);
				}
				if (this.ModelLoaded != null)
				{
					this.ModelLoaded(obj, absolutePath);
				}
			}
		}

		protected virtual void OnCreated(GameObject obj, string absolutePath)
		{
			if (obj == null)
			{
				if (this.ModelError != null)
				{
					this.ModelError(absolutePath);
					return;
				}
			}
			else if (this.ModelCreated != null)
			{
				this.ModelCreated(obj, absolutePath);
			}
		}

		protected virtual void OnLoadFailed(string absolutePath)
		{
			if (this.ModelError != null)
			{
				this.ModelError(absolutePath);
			}
		}

		private string GetTextureUrl(string basePath, string texturePath)
		{
			string text = texturePath.Replace("\\", "/").Replace("//", "/");
			if (!Path.IsPathRooted(text))
			{
				text = basePath + texturePath;
			}
			if (!text.Contains("//"))
			{
				text = "file:///" + text;
			}
			this.objLoadingProgress.message = "Loading textures...\n" + text;
			return text;
		}

		private async Task LoadMaterialTexture(string basePath, string path, string texturesFolderPath = "")
		{
			this.loadedTexture = null;
			string text = string.IsNullOrWhiteSpace(texturesFolderPath) ? (basePath + path) : (texturesFolderPath + "\\" + path);
			text = Path.GetFullPath(text);
			if (File.Exists(text))
			{
				byte[] result;
				using (FileStream stream = File.Open(text, FileMode.Open))
				{
					result = new byte[stream.Length];
					await stream.ReadAsync(result, 0, (int)stream.Length);
				}
				FileStream stream = null;
				if (result.Length != 0)
				{
					Texture2D tex = new Texture2D(1, 1);
					tex.LoadImage(result);
					this.loadedTexture = tex;
				}
				result = null;
			}
			else
			{
				Debug.LogWarning(string.Concat(new string[]
				{
					"Failed to load texture at path  ",
					text,
					"   BasePath  ",
					basePath,
					"  path  ",
					path
				}));
			}
		}

		private async Task LoadMaterialTexture(string textureURL)
		{
			this.loadedTexture = null;
			bool isWorking = true;
			byte[] downloadedBytes = null;
			float value = this.individualProgress.Value;
			try
			{
				base.StartCoroutine(this.DownloadFile(textureURL, this.individualProgress, delegate(byte[] bytes)
				{
					isWorking = false;
					downloadedBytes = bytes;
				}, delegate(string error)
				{
					ObjectImporter.activeDownloads--;
					isWorking = false;
					Debug.LogWarning("Failed to load the associated texture file." + error);
				}));
				goto IL_14E;
			}
			catch (Exception ex)
			{
				ObjectImporter.activeDownloads--;
				this.individualProgress.Value = value;
				ObjectImporter.downloadProgress.Value = this.individualProgress.Value / (float)ObjectImporter.activeDownloads * 100f;
				isWorking = false;
				throw ex;
			}
			IL_D1:
			ObjectImporter.downloadProgress.Value = this.individualProgress.Value / (float)ObjectImporter.activeDownloads * 100f;
			await Task.Delay(3);
			IL_14E:
			if (!isWorking)
			{
				ObjectImporter.downloadProgress.Value = this.individualProgress.Value / (float)ObjectImporter.activeDownloads * 100f;
				if (downloadedBytes != null && downloadedBytes.Length != 0)
				{
					Texture2D tex = new Texture2D(1, 1);
					tex.LoadImage(downloadedBytes);
					this.loadedTexture = tex;
				}
				else
				{
					Debug.LogWarning("Failed to load texture.");
				}
				return;
			}
			goto IL_D1;
		}

		private IEnumerator LoadMaterialTextureWebGL(string textureURL)
		{
			this.loadedTexture = null;
			bool isWorking = true;
			float value = this.individualProgress.Value;
			try
			{
				base.StartCoroutine(this.DownloadTexFileWebGL(textureURL, this.individualProgress, delegate(Texture2D texture)
				{
					isWorking = false;
					this.loadedTexture = texture;
				}, delegate(string error)
				{
					ObjectImporter.activeDownloads--;
					isWorking = false;
					Debug.LogWarning("Failed to load the associated texture file." + error);
				}));
				goto IL_125;
			}
			catch (Exception ex)
			{
				ObjectImporter.activeDownloads--;
				this.individualProgress.Value = value;
				ObjectImporter.downloadProgress.Value = this.individualProgress.Value / (float)ObjectImporter.activeDownloads * 100f;
				isWorking = false;
				throw ex;
			}
			IL_E3:
			yield return new WaitForSeconds(0.1f);
			ObjectImporter.downloadProgress.Value = this.individualProgress.Value / (float)ObjectImporter.activeDownloads * 100f;
			IL_125:
			if (!isWorking)
			{
				ObjectImporter.downloadProgress.Value = this.individualProgress.Value / (float)ObjectImporter.activeDownloads * 100f;
				if (this.loadedTexture == null)
				{
					Debug.LogWarning("Failed to load texture.");
				}
				yield break;
			}
			goto IL_E3;
		}

		private Texture2D LoadTexture(UnityWebRequest loader)
		{
			string text = Path.GetExtension(loader.url).ToLower();
			Texture2D texture2D = null;
			if (text == ".tga")
			{
				texture2D = TextureLoader.LoadTextureFromUrl(loader.url);
			}
			else if (text == ".png" || text == ".jpg" || text == ".jpeg")
			{
				texture2D = DownloadHandlerTexture.GetContent(loader);
			}
			else
			{
				Debug.LogWarning("Unsupported texture format: " + text);
			}
			if (texture2D == null)
			{
				Debug.LogErrorFormat("Failed to load texture {0}", new object[]
				{
					loader.url
				});
			}
			return texture2D;
		}

		public IEnumerator DownloadFile(string url, PolyfewRuntime.ReferencedNumeric<float> downloadProgress, Action<byte[]> DownloadComplete, Action<string> OnError)
		{
			WWW www = null;
			float oldProgress = downloadProgress.Value;
			try
			{
				www = new WWW(url);
			}
			catch (Exception ex)
			{
				downloadProgress.Value = oldProgress;
				OnError(ex.ToString());
			}
			Coroutine progress = base.StartCoroutine(this.GetProgress(www, downloadProgress));
			yield return www;
			if (!string.IsNullOrWhiteSpace(www.error))
			{
				downloadProgress.Value = oldProgress;
				OnError(www.error);
			}
			else if (www.bytes == null || www.bytes.Length == 0)
			{
				if (string.IsNullOrWhiteSpace(www.error))
				{
					downloadProgress.Value = oldProgress;
					OnError("No bytes downloaded. The file might be empty.");
				}
				else
				{
					downloadProgress.Value = oldProgress;
					OnError(www.error);
				}
			}
			else if (string.IsNullOrWhiteSpace(www.error))
			{
				downloadProgress.Value = oldProgress + 1f;
				DownloadComplete(www.bytes);
			}
			else
			{
				downloadProgress.Value = oldProgress;
				OnError(www.error);
			}
			base.StopCoroutine(progress);
			www.Dispose();
			yield break;
		}

		private IEnumerator GetProgress(WWW www, PolyfewRuntime.ReferencedNumeric<float> downloadProgress)
		{
			float oldProgress = downloadProgress.Value;
			if (www != null && downloadProgress != null)
			{
				while (!www.isDone && string.IsNullOrWhiteSpace(www.error))
				{
					yield return new WaitForSeconds(0.1f);
					downloadProgress.Value = oldProgress + www.progress;
				}
				if (www.isDone && string.IsNullOrWhiteSpace(www.error))
				{
					downloadProgress.Value = oldProgress + www.progress;
					Debug.Log("Progress  " + www.progress);
				}
			}
			yield break;
		}

		public IEnumerator DownloadFileWebGL(string url, PolyfewRuntime.ReferencedNumeric<float> downloadProgress, Action<string> DownloadComplete, Action<string> OnError)
		{
			WWW www = null;
			float oldProgress = downloadProgress.Value;
			try
			{
				www = new WWW(url);
			}
			catch (Exception ex)
			{
				downloadProgress.Value = oldProgress;
				OnError(ex.ToString());
			}
			Coroutine progress = base.StartCoroutine(this.GetProgress(www, downloadProgress));
			yield return www;
			if (!string.IsNullOrWhiteSpace(www.error))
			{
				downloadProgress.Value = oldProgress;
				OnError(www.error);
			}
			else if (www.bytes == null || www.bytes.Length == 0)
			{
				if (string.IsNullOrWhiteSpace(www.error))
				{
					downloadProgress.Value = oldProgress;
					OnError("No bytes downloaded. The file might be empty.");
				}
				else
				{
					downloadProgress.Value = oldProgress;
					OnError(www.error);
				}
			}
			else if (string.IsNullOrWhiteSpace(www.error))
			{
				downloadProgress.Value = oldProgress + 1f;
				DownloadComplete(www.text);
			}
			else
			{
				downloadProgress.Value = oldProgress;
				OnError(www.error);
			}
			try
			{
				base.StopCoroutine(progress);
			}
			catch (Exception)
			{
			}
			www.Dispose();
			yield break;
		}

		public IEnumerator DownloadTexFileWebGL(string url, PolyfewRuntime.ReferencedNumeric<float> downloadProgress, Action<Texture2D> DownloadComplete, Action<string> OnError)
		{
			WWW www = null;
			float oldProgress = downloadProgress.Value;
			try
			{
				www = new WWW(url);
			}
			catch (Exception ex)
			{
				downloadProgress.Value = oldProgress;
				OnError(ex.ToString());
			}
			Coroutine progress = base.StartCoroutine(this.GetProgress(www, downloadProgress));
			yield return www;
			if (!string.IsNullOrWhiteSpace(www.error))
			{
				downloadProgress.Value = oldProgress;
				OnError(www.error);
			}
			else if (www.bytes == null || www.bytes.Length == 0)
			{
				if (string.IsNullOrWhiteSpace(www.error))
				{
					downloadProgress.Value = oldProgress;
					OnError("No bytes downloaded. The file might be empty.");
				}
				else
				{
					downloadProgress.Value = oldProgress;
					OnError(www.error);
				}
			}
			else if (string.IsNullOrWhiteSpace(www.error))
			{
				downloadProgress.Value = oldProgress + 1f;
				DownloadComplete(www.texture);
			}
			else
			{
				downloadProgress.Value = oldProgress;
				OnError(www.error);
			}
			base.StopCoroutine(progress);
			www.Dispose();
			yield break;
		}

		public static LoadingProgress totalProgress = new LoadingProgress();

		public ImportOptions buildOptions;

		public PolyfewRuntime.ReferencedNumeric<float> individualProgress = new PolyfewRuntime.ReferencedNumeric<float>(0f);

		protected static float LOAD_PHASE_PERC = 8f;

		protected static float TEXTURE_PHASE_PERC = 1f;

		protected static float MATERIAL_PHASE_PERC = 1f;

		protected static float BUILD_PHASE_PERC = 90f;

		protected static Dictionary<string, GameObject> loadedModels = new Dictionary<string, GameObject>();

		protected static Dictionary<string, int> instanceCount = new Dictionary<string, int>();

		protected DataSet dataSet = new DataSet();

		protected ObjectBuilder objectBuilder = new ObjectBuilder();

		protected List<MaterialData> materialData;

		protected SingleLoadingProgress objLoadingProgress = new SingleLoadingProgress();

		protected Loader.Stats loadStats;

		private Texture2D loadedTexture;

		protected struct BuildStats
		{
			public float texturesTime;

			public float materialsTime;

			public float objectsTime;
		}

		protected struct Stats
		{
			public float modelParseTime;

			public float materialsParseTime;

			public float buildTime;

			public Loader.BuildStats buildStats;

			public float totalTime;
		}
	}
}
