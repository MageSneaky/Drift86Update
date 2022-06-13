using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace AsImpL
{
	public class LoaderObj : Loader
	{
		public override string[] ParseTexturePaths(string absolutePath)
		{
			List<string> list = new List<string>();
			string dirName = base.GetDirName(absolutePath);
			string text = this.ParseMaterialLibName(absolutePath);
			if (!string.IsNullOrEmpty(text))
			{
				string[] lines = File.ReadAllLines(dirName + text);
				List<MaterialData> list2 = new List<MaterialData>();
				this.ParseMaterialData(lines, list2);
				foreach (MaterialData materialData in list2)
				{
					if (!string.IsNullOrEmpty(materialData.diffuseTexPath))
					{
						list.Add(materialData.diffuseTexPath);
					}
					if (!string.IsNullOrEmpty(materialData.specularTexPath))
					{
						list.Add(materialData.specularTexPath);
					}
					if (!string.IsNullOrEmpty(materialData.bumpTexPath))
					{
						list.Add(materialData.bumpTexPath);
					}
					if (!string.IsNullOrEmpty(materialData.opacityTexPath))
					{
						list.Add(materialData.opacityTexPath);
					}
				}
			}
			return list.ToArray();
		}

		protected override async Task LoadModelFile(string absolutePath, string texturesFolderPath = "", string materialsFolderPath = "")
		{
			if (!absolutePath.Contains("//"))
			{
				"file:///" + absolutePath;
			}
			using (StreamReader sr = new StreamReader(absolutePath))
			{
				string text = await sr.ReadToEndAsync();
				this.loadedText = text;
			}
			StreamReader sr = null;
			if (string.IsNullOrEmpty(this.loadedText))
			{
				Loader.totalProgress.singleProgress.Remove(this.objLoadingProgress);
				throw new InvalidOperationException("Failed to load data from file. The file might be empty or non readable.");
			}
			this.ParseGeometryData(this.loadedText);
		}

		protected override async Task LoadModelFileNetworked(string objURL)
		{
			bool isWorking = true;
			byte[] downloadedBytes = null;
			Exception ex = null;
			float value = this.individualProgress.Value;
			try
			{
				base.StartCoroutine(base.DownloadFile(objURL, this.individualProgress, delegate(byte[] bytes)
				{
					isWorking = false;
					downloadedBytes = bytes;
				}, delegate(string error)
				{
					ObjectImporter.activeDownloads--;
					ex = new InvalidOperationException("Failed to download base model." + error);
					isWorking = false;
				}));
				goto IL_15A;
			}
			catch (Exception ex)
			{
				ObjectImporter.activeDownloads--;
				this.individualProgress.Value = value;
				ObjectImporter.downloadProgress.Value = this.individualProgress.Value / (float)ObjectImporter.activeDownloads * 100f;
				isWorking = false;
				Exception ex3;
				throw ex3;
			}
			IL_DD:
			ObjectImporter.downloadProgress.Value = this.individualProgress.Value / (float)ObjectImporter.activeDownloads * 100f;
			await Task.Delay(1);
			IL_15A:
			if (isWorking)
			{
				goto IL_DD;
			}
			if (ex != null)
			{
				throw ex;
			}
			ObjectImporter.downloadProgress.Value = this.individualProgress.Value / (float)ObjectImporter.activeDownloads * 100f;
			if (downloadedBytes != null && downloadedBytes.Length != 0)
			{
				using (StreamReader sr = new StreamReader(new MemoryStream(downloadedBytes)))
				{
					this.loadedText = await sr.ReadToEndAsync();
				}
				StreamReader sr = null;
			}
			if (string.IsNullOrEmpty(this.loadedText))
			{
				Loader.totalProgress.singleProgress.Remove(this.objLoadingProgress);
				throw new InvalidOperationException("Failed to load data from the downloaded obj file. The file might be empty or non readable.");
			}
			try
			{
				this.ParseGeometryData(this.loadedText);
			}
			catch (Exception ex2)
			{
				throw ex2;
			}
		}

		protected override IEnumerator LoadModelFileNetworkedWebGL(string objURL, Action<Exception> OnError)
		{
			bool isWorking = true;
			Exception ex = null;
			float value = this.individualProgress.Value;
			try
			{
				base.StartCoroutine(base.DownloadFileWebGL(objURL, this.individualProgress, delegate(string text)
				{
					isWorking = false;
					this.loadedText = text;
				}, delegate(string error)
				{
					ObjectImporter.activeDownloads--;
					ex = new InvalidOperationException("Base model download unsuccessful." + error);
					ObjectImporter.isException = true;
					OnError(ex);
					isWorking = false;
				}));
				goto IL_168;
			}
			catch (Exception obj)
			{
				ObjectImporter.activeDownloads--;
				this.individualProgress.Value = value;
				ObjectImporter.downloadProgress.Value = this.individualProgress.Value / (float)ObjectImporter.activeDownloads * 100f;
				isWorking = false;
				isWorking = false;
				OnError(obj);
				ObjectImporter.isException = true;
				goto IL_168;
			}
			IL_126:
			yield return new WaitForSeconds(0.1f);
			ObjectImporter.downloadProgress.Value = this.individualProgress.Value / (float)ObjectImporter.activeDownloads * 100f;
			IL_168:
			if (isWorking)
			{
				goto IL_126;
			}
			if (ObjectImporter.isException)
			{
				yield return null;
			}
			ObjectImporter.downloadProgress.Value = this.individualProgress.Value / (float)ObjectImporter.activeDownloads * 100f;
			if (string.IsNullOrEmpty(this.loadedText))
			{
				Loader.totalProgress.singleProgress.Remove(this.objLoadingProgress);
				throw new InvalidOperationException("Failed to load data from the downloaded obj file. The file might be empty or non readable.");
			}
			try
			{
				this.ParseGeometryData(this.loadedText);
				yield break;
			}
			catch (Exception obj2)
			{
				OnError(obj2);
				ObjectImporter.isException = true;
				yield break;
			}
			yield break;
		}

		protected override async Task LoadMaterialLibrary(string absolutePath, string materialsFolderPath = "")
		{
			string dirName = base.GetDirName(absolutePath);
			if (absolutePath.Contains("//"))
			{
				int num;
				if (absolutePath.Contains("?"))
				{
					num = absolutePath.LastIndexOf('=');
				}
				else
				{
					num = absolutePath.LastIndexOf('/');
				}
				absolutePath.Remove(num + 1) + this.mtlLib;
			}
			else if (Path.IsPathRooted(this.mtlLib))
			{
				"file:///" + this.mtlLib;
			}
			else
			{
				"file:///" + dirName + this.mtlLib;
			}
			string path = string.IsNullOrWhiteSpace(materialsFolderPath) ? (dirName + this.mtlLib) : (materialsFolderPath + this.mtlLib);
			if (File.Exists(path))
			{
				using (StreamReader sr = new StreamReader(path))
				{
					string text = await sr.ReadToEndAsync();
					this.loadedText = text;
				}
				StreamReader sr = null;
			}
			else
			{
				Debug.LogWarning("Cannot find the associated material file at the path   " + dirName + this.mtlLib);
			}
			if (!string.IsNullOrWhiteSpace(this.loadedText))
			{
				this.objLoadingProgress.message = "Parsing material library...";
				this.ParseMaterialData(this.loadedText);
			}
		}

		protected override async Task LoadMaterialLibrary(string materialURL)
		{
			bool isWorking = true;
			byte[] downloadedBytes = null;
			float value = this.individualProgress.Value;
			try
			{
				base.StartCoroutine(base.DownloadFile(materialURL, this.individualProgress, delegate(byte[] bytes)
				{
					isWorking = false;
					downloadedBytes = bytes;
				}, delegate(string error)
				{
					ObjectImporter.activeDownloads--;
					isWorking = false;
					Debug.LogWarning("Failed to load the associated material file." + error);
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
					using (StreamReader sr = new StreamReader(new MemoryStream(downloadedBytes)))
					{
						this.loadedText = await sr.ReadToEndAsync();
					}
					StreamReader sr = null;
				}
				if (!string.IsNullOrWhiteSpace(this.loadedText))
				{
					this.objLoadingProgress.message = "Parsing material library...";
					this.ParseMaterialData(this.loadedText);
				}
				return;
			}
			goto IL_D1;
		}

		protected override IEnumerator LoadMaterialLibraryWebGL(string materialURL)
		{
			bool isWorking = true;
			float value = this.individualProgress.Value;
			base.StartCoroutine(base.DownloadFileWebGL(materialURL, this.individualProgress, delegate(string text)
			{
				isWorking = false;
				this.loadedText = text;
			}, delegate(string error)
			{
				ObjectImporter.activeDownloads--;
				isWorking = false;
				Debug.LogWarning("Failed to load the associated material file." + error);
			}));
			while (isWorking)
			{
				yield return new WaitForSeconds(0.1f);
				ObjectImporter.downloadProgress.Value = this.individualProgress.Value / (float)ObjectImporter.activeDownloads * 100f;
			}
			ObjectImporter.downloadProgress.Value = this.individualProgress.Value / (float)ObjectImporter.activeDownloads * 100f;
			if (!string.IsNullOrWhiteSpace(this.loadedText))
			{
				this.objLoadingProgress.message = "Parsing material library...";
				this.ParseMaterialData(this.loadedText);
			}
			yield break;
		}

		private void GetFaceIndicesByOneFaceLine(DataSet.FaceIndices[] faces, string[] p, bool isFaceIndexPlus)
		{
			if (isFaceIndexPlus)
			{
				for (int i = 1; i < p.Length; i++)
				{
					string[] array = p[i].Trim().Split("/".ToCharArray());
					DataSet.FaceIndices faceIndices = default(DataSet.FaceIndices);
					int num = int.Parse(array[0]);
					faceIndices.vertIdx = num - 1;
					if (array.Length > 1 && array[1] != "")
					{
						int num2 = int.Parse(array[1]);
						faceIndices.uvIdx = num2 - 1;
					}
					if (array.Length > 2 && array[2] != "")
					{
						int num3 = int.Parse(array[2]);
						faceIndices.normIdx = num3 - 1;
					}
					else
					{
						faceIndices.normIdx = -1;
					}
					faces[i - 1] = faceIndices;
				}
				return;
			}
			int count = this.dataSet.vertList.Count;
			int count2 = this.dataSet.uvList.Count;
			for (int j = 1; j < p.Length; j++)
			{
				string[] array2 = p[j].Trim().Split("/".ToCharArray());
				DataSet.FaceIndices faceIndices2 = default(DataSet.FaceIndices);
				int num4 = int.Parse(array2[0]);
				faceIndices2.vertIdx = count + num4;
				if (array2.Length > 1 && array2[1] != "")
				{
					int num5 = int.Parse(array2[1]);
					faceIndices2.uvIdx = count2 + num5;
				}
				if (array2.Length > 2 && array2[2] != "")
				{
					int num6 = int.Parse(array2[2]);
					faceIndices2.normIdx = count + num6;
				}
				else
				{
					faceIndices2.normIdx = -1;
				}
				faces[j - 1] = faceIndices2;
			}
		}

		private Vector3 ConvertVec3(float x, float y, float z)
		{
			if (base.Scaling != 1f)
			{
				x *= base.Scaling;
				y *= base.Scaling;
				z *= base.Scaling;
			}
			if (base.ConvertVertAxis)
			{
				return new Vector3(x, z, y);
			}
			return new Vector3(x, y, -z);
		}

		private float ParseFloat(string floatString)
		{
			return float.Parse(floatString, CultureInfo.InvariantCulture.NumberFormat);
		}

		protected void ParseGeometryData(string objDataText)
		{
			string[] array = objDataText.Split("\n".ToCharArray());
			bool flag = true;
			bool isFaceIndexPlus = true;
			this.objLoadingProgress.message = "Parsing geometry data...";
			char[] separator = new char[]
			{
				' ',
				'\t'
			};
			for (int i = 0; i < array.Length; i++)
			{
				string text = array[i].Trim();
				if (text.Length <= 0 || text[0] != '#')
				{
					string[] array2 = text.Split(separator, StringSplitOptions.RemoveEmptyEntries);
					if (array2.Length != 0)
					{
						string text2 = null;
						if (text.Length > array2[0].Length)
						{
							text2 = text.Substring(array2[0].Length + 1).Trim();
						}
						string text3 = array2[0];
						uint num = <PrivateImplementationDetails>.ComputeStringHash(text3);
						if (num <= 1498016135U)
						{
							if (num <= 1128908517U)
							{
								if (num != 990293175U)
								{
									if (num == 1128908517U)
									{
										if (text3 == "vn")
										{
											this.dataSet.AddNormal(this.ConvertVec3(this.ParseFloat(array2[1]), this.ParseFloat(array2[2]), this.ParseFloat(array2[3])));
										}
									}
								}
								else if (text3 == "mtllib")
								{
									if (!string.IsNullOrEmpty(text2))
									{
										this.mtlLib = text2;
									}
								}
							}
							else if (num != 1328799683U)
							{
								if (num == 1498016135U)
								{
									if (text3 == "vt")
									{
										this.dataSet.AddUV(new Vector2(this.ParseFloat(array2[1]), this.ParseFloat(array2[2])));
									}
								}
							}
							else if (text3 == "usemtl")
							{
								if (!string.IsNullOrEmpty(text2))
								{
									this.dataSet.AddMaterialName(DataSet.FixMaterialName(text2));
								}
							}
						}
						else if (num <= 3809224601U)
						{
							if (num != 3792446982U)
							{
								if (num == 3809224601U)
								{
									if (text3 == "f")
									{
										int num2 = array2.Length - 1;
										DataSet.FaceIndices[] array3 = new DataSet.FaceIndices[num2];
										if (flag)
										{
											flag = false;
											isFaceIndexPlus = (int.Parse(array2[1].Trim().Split("/".ToCharArray())[0]) >= 0);
										}
										this.GetFaceIndicesByOneFaceLine(array3, array2, isFaceIndexPlus);
										if (num2 == 3)
										{
											this.dataSet.AddFaceIndices(array3[0]);
											this.dataSet.AddFaceIndices(array3[2]);
											this.dataSet.AddFaceIndices(array3[1]);
										}
										else
										{
											Triangulator.Triangulate(this.dataSet, array3);
										}
									}
								}
							}
							else if (text3 == "g")
							{
								flag = true;
								this.dataSet.AddGroup(text2);
							}
						}
						else if (num != 3926667934U)
						{
							if (num == 4077666505U)
							{
								if (text3 == "v")
								{
									this.dataSet.AddVertex(this.ConvertVec3(this.ParseFloat(array2[1]), this.ParseFloat(array2[2]), this.ParseFloat(array2[3])));
									if (array2.Length >= 7)
									{
										this.dataSet.AddColor(new Color(this.ParseFloat(array2[4]), this.ParseFloat(array2[5]), this.ParseFloat(array2[6]), 1f));
									}
								}
							}
						}
						else if (text3 == "o")
						{
							this.dataSet.AddObject(text2);
							flag = true;
						}
						if (i % 7000 == 0)
						{
							this.objLoadingProgress.percentage = Loader.LOAD_PHASE_PERC * (float)i / (float)array.Length;
						}
					}
				}
			}
			this.objLoadingProgress.percentage = Loader.LOAD_PHASE_PERC;
		}

		private string ParseMaterialLibName(string path)
		{
			string[] array = File.ReadAllLines(path);
			this.objLoadingProgress.message = "Parsing geometry data...";
			for (int i = 0; i < array.Length; i++)
			{
				string text = array[i].Trim();
				if (text.StartsWith("mtllib"))
				{
					return text.Substring("mtllib".Length).Trim();
				}
			}
			return null;
		}

		protected override bool HasMaterialLibrary
		{
			get
			{
				return this.mtlLib != null;
			}
		}

		private void ParseMaterialData(string data)
		{
			this.objLoadingProgress.message = "Parsing material data...";
			string[] lines = data.Split("\n".ToCharArray());
			this.materialData = new List<MaterialData>();
			this.ParseMaterialData(lines, this.materialData);
		}

		private void ParseMaterialData(string[] lines, List<MaterialData> mtlData)
		{
			MaterialData materialData = new MaterialData();
			char[] separator = new char[]
			{
				' ',
				'\t'
			};
			for (int i = 0; i < lines.Length; i++)
			{
				string text = lines[i].Trim();
				if (text.IndexOf("#") != -1)
				{
					text = text.Substring(0, text.IndexOf("#"));
				}
				string[] array = text.Split(separator, StringSplitOptions.RemoveEmptyEntries);
				if (array.Length != 0 && !string.IsNullOrEmpty(array[0]))
				{
					string text2 = null;
					if (text.Length > array[0].Length)
					{
						text2 = text.Substring(array[0].Length + 1).Trim();
					}
					try
					{
						string text3 = array[0];
						uint num = <PrivateImplementationDetails>.ComputeStringHash(text3);
						if (num <= 2470767803U)
						{
							if (num <= 1206838257U)
							{
								if (num <= 951936964U)
								{
									if (num != 349989973U)
									{
										if (num != 951936964U)
										{
											goto IL_55D;
										}
										if (!(text3 == "newmtl"))
										{
											goto IL_55D;
										}
										materialData = new MaterialData();
										materialData.materialName = DataSet.FixMaterialName(text2);
										mtlData.Add(materialData);
										goto IL_55D;
									}
									else if (!(text3 == "map_KD"))
									{
										goto IL_55D;
									}
								}
								else if (num != 1172738566U)
								{
									if (num != 1190501923U)
									{
										if (num != 1206838257U)
										{
											goto IL_55D;
										}
										if (!(text3 == "Ka"))
										{
											goto IL_55D;
										}
										materialData.ambientColor = this.StringsToColor(array);
										goto IL_55D;
									}
									else
									{
										if (!(text3 == "Tr"))
										{
											goto IL_55D;
										}
										materialData.overallAlpha = ((array.Length > 1 && array[1] != "") ? (1f - this.ParseFloat(array[1])) : 1f);
										goto IL_55D;
									}
								}
								else
								{
									if (!(text3 == "Ns"))
									{
										goto IL_55D;
									}
									materialData.shininess = this.ParseFloat(array[1]);
									goto IL_55D;
								}
							}
							else if (num <= 1273948733U)
							{
								if (num != 1257171114U)
								{
									if (num != 1273948733U)
									{
										goto IL_55D;
									}
									if (!(text3 == "Ke"))
									{
										goto IL_55D;
									}
									materialData.emissiveColor = this.StringsToColor(array);
									goto IL_55D;
								}
								else
								{
									if (!(text3 == "Kd"))
									{
										goto IL_55D;
									}
									materialData.diffuseColor = this.StringsToColor(array);
									goto IL_55D;
								}
							}
							else if (num != 1441724923U)
							{
								if (num != 1648103349U)
								{
									if (num != 2470767803U)
									{
										goto IL_55D;
									}
									if (!(text3 == "map_opacity"))
									{
										goto IL_55D;
									}
									goto IL_50E;
								}
								else
								{
									if (!(text3 == "bump"))
									{
										goto IL_55D;
									}
									this.ParseBumpParameters(array, materialData);
									goto IL_55D;
								}
							}
							else
							{
								if (!(text3 == "Ks"))
								{
									goto IL_55D;
								}
								materialData.specularColor = this.StringsToColor(array);
								goto IL_55D;
							}
						}
						else
						{
							if (num <= 3572267306U)
							{
								if (num <= 3087535434U)
								{
									if (num != 2551255962U)
									{
										if (num != 3087535434U)
										{
											goto IL_55D;
										}
										if (!(text3 == "map_d"))
										{
											goto IL_55D;
										}
										goto IL_50E;
									}
									else
									{
										if (!(text3 == "map_bump"))
										{
											goto IL_55D;
										}
										if (!string.IsNullOrEmpty(text2))
										{
											materialData.bumpTexPath = text2;
											goto IL_55D;
										}
										goto IL_55D;
									}
								}
								else if (num != 3180081536U)
								{
									if (num != 3482078678U)
									{
										if (num != 3572267306U)
										{
											goto IL_55D;
										}
										if (!(text3 == "refl"))
										{
											goto IL_55D;
										}
										if (!string.IsNullOrEmpty(text2))
										{
											materialData.hasReflectionTex = true;
											goto IL_55D;
										}
										goto IL_55D;
									}
									else
									{
										if (!(text3 == "map_kA"))
										{
											goto IL_55D;
										}
										goto IL_544;
									}
								}
								else if (!(text3 == "map_kS"))
								{
									goto IL_55D;
								}
							}
							else if (num <= 3775669363U)
							{
								if (num != 3702066226U)
								{
									if (num != 3722188224U)
									{
										if (num != 3775669363U)
										{
											goto IL_55D;
										}
										if (!(text3 == "d"))
										{
											goto IL_55D;
										}
										materialData.overallAlpha = ((array.Length > 1 && array[1] != "") ? this.ParseFloat(array[1]) : 1f);
										goto IL_55D;
									}
									else if (!(text3 == "map_Ks"))
									{
										goto IL_55D;
									}
								}
								else
								{
									if (!(text3 == "illum"))
									{
										goto IL_55D;
									}
									materialData.illumType = int.Parse(array[1]);
									goto IL_55D;
								}
							}
							else if (num != 4024185366U)
							{
								if (num != 4108073461U)
								{
									if (num != 4125292365U)
									{
										goto IL_55D;
									}
									if (!(text3 == "map_Ns"))
									{
										goto IL_55D;
									}
								}
								else
								{
									if (!(text3 == "map_Kd"))
									{
										goto IL_55D;
									}
									goto IL_4C4;
								}
							}
							else
							{
								if (!(text3 == "map_Ka"))
								{
									goto IL_55D;
								}
								goto IL_544;
							}
							if (!string.IsNullOrEmpty(text2))
							{
								materialData.specularTexPath = text2;
								goto IL_55D;
							}
							goto IL_55D;
							IL_544:
							if (!string.IsNullOrEmpty(text2))
							{
								Debug.Log("Map not supported:" + text);
								goto IL_55D;
							}
							goto IL_55D;
						}
						IL_4C4:
						if (!string.IsNullOrEmpty(text2))
						{
							materialData.diffuseTexPath = text2;
							goto IL_55D;
						}
						goto IL_55D;
						IL_50E:
						if (!string.IsNullOrEmpty(text2))
						{
							materialData.opacityTexPath = text2;
						}
						IL_55D:;
					}
					catch (Exception ex)
					{
						Debug.LogErrorFormat("Error at line {0} in mtl file: {1}", new object[]
						{
							i + 1,
							ex
						});
					}
				}
			}
		}

		private void ParseBumpParameters(string[] param, MaterialData mtlData)
		{
			Regex regex = new Regex("^[-+]?[0-9]*\\.?[0-9]+$");
			Dictionary<string, LoaderObj.BumpParamDef> dictionary = new Dictionary<string, LoaderObj.BumpParamDef>();
			dictionary.Add("bm", new LoaderObj.BumpParamDef("bm", "string", 1, 1));
			dictionary.Add("clamp", new LoaderObj.BumpParamDef("clamp", "string", 1, 1));
			dictionary.Add("blendu", new LoaderObj.BumpParamDef("blendu", "string", 1, 1));
			dictionary.Add("blendv", new LoaderObj.BumpParamDef("blendv", "string", 1, 1));
			dictionary.Add("imfchan", new LoaderObj.BumpParamDef("imfchan", "string", 1, 1));
			dictionary.Add("mm", new LoaderObj.BumpParamDef("mm", "string", 1, 1));
			dictionary.Add("o", new LoaderObj.BumpParamDef("o", "number", 1, 3));
			dictionary.Add("s", new LoaderObj.BumpParamDef("s", "number", 1, 3));
			dictionary.Add("t", new LoaderObj.BumpParamDef("t", "number", 1, 3));
			dictionary.Add("texres", new LoaderObj.BumpParamDef("texres", "string", 1, 1));
			int i = 1;
			string text = null;
			while (i < param.Length)
			{
				if (!param[i].StartsWith("-"))
				{
					text = param[i];
					i++;
				}
				else
				{
					string text2 = param[i].Substring(1);
					i++;
					if (dictionary.ContainsKey(text2))
					{
						LoaderObj.BumpParamDef bumpParamDef = dictionary[text2];
						ArrayList arrayList = new ArrayList();
						int j = 0;
						bool flag = false;
						while (j < bumpParamDef.valueNumMin)
						{
							if (i >= param.Length)
							{
								flag = true;
								break;
							}
							if (bumpParamDef.valueType == "number" && !regex.Match(param[i]).Success)
							{
								flag = true;
								break;
							}
							arrayList.Add(param[i]);
							j++;
							i++;
						}
						if (flag)
						{
							Debug.Log("bump variable value not enough for option:" + text2 + " of material:" + mtlData.materialName);
						}
						else
						{
							while (j < bumpParamDef.valueNumMax && i < param.Length && (!(bumpParamDef.valueType == "number") || regex.Match(param[i]).Success))
							{
								arrayList.Add(param[i]);
								j++;
								i++;
							}
							Debug.Log(string.Concat(new string[]
							{
								"found option: ",
								text2,
								" of material: ",
								mtlData.materialName,
								" args: ",
								string.Concat(arrayList.ToArray())
							}));
						}
					}
				}
			}
			if (text != null)
			{
				mtlData.bumpTexPath = text;
			}
		}

		private Color StringsToColor(string[] p)
		{
			return new Color(this.ParseFloat(p[1]), this.ParseFloat(p[2]), this.ParseFloat(p[3]));
		}

		private IEnumerator LoadOrDownloadText(string url, bool notifyErrors = true)
		{
			this.loadedText = null;
			UnityWebRequest uwr = UnityWebRequest.Get(url);
			yield return uwr.SendWebRequest();
			if (uwr.isNetworkError || uwr.isHttpError)
			{
				if (notifyErrors)
				{
					Debug.LogError(uwr.error);
				}
			}
			else
			{
				this.loadedText = uwr.downloadHandler.text;
			}
			yield break;
		}

		private string mtlLib;

		private string loadedText;

		private class BumpParamDef
		{
			public BumpParamDef(string name, string type, int numMin, int numMax)
			{
				this.optionName = name;
				this.valueType = type;
				this.valueNumMin = numMin;
				this.valueNumMax = numMax;
			}

			public string optionName;

			public string valueType;

			public int valueNumMin;

			public int valueNumMax;
		}
	}
}
