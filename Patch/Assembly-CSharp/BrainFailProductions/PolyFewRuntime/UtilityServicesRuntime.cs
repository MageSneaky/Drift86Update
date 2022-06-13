using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using AsImpL;
using UnityEngine;

namespace BrainFailProductions.PolyFewRuntime
{
	public class UtilityServicesRuntime : MonoBehaviour
	{
		public static Texture2D DuplicateTexture(Texture2D source)
		{
			RenderTexture temporary = RenderTexture.GetTemporary(source.width, source.height, 0, 7, 1);
			Graphics.Blit(source, temporary);
			RenderTexture active = RenderTexture.active;
			RenderTexture.active = temporary;
			Texture2D texture2D = new Texture2D(source.width, source.height);
			texture2D.ReadPixels(new Rect(0f, 0f, (float)temporary.width, (float)temporary.height), 0, 0);
			texture2D.Apply();
			RenderTexture.active = active;
			RenderTexture.ReleaseTemporary(temporary);
			return texture2D;
		}

		public static Renderer[] GetChildRenderersForCombining(GameObject forObject, bool skipInactiveChildObjects)
		{
			List<Renderer> list = new List<Renderer>();
			if (skipInactiveChildObjects && !forObject.gameObject.activeSelf)
			{
				Debug.LogWarning("No Renderers under the GameObject \"" + forObject.name + "\" combined because the object was inactive and was skipped entirely.");
				return null;
			}
			if (forObject.GetComponent<LODGroup>() != null)
			{
				Debug.LogWarning("No Renderers under the GameObject \"" + forObject.name + "\" combined because the object had LOD groups and was skipped entirely.");
				return null;
			}
			UtilityServicesRuntime.CollectChildRenderersForCombining(forObject.transform, list, skipInactiveChildObjects);
			return list.ToArray();
		}

		public static MeshRenderer CreateStaticLevelRenderer(string name, Transform parentTransform, Transform originalTransform, Mesh mesh, Material[] materials)
		{
			GameObject gameObject = new GameObject(name, new Type[]
			{
				typeof(MeshFilter),
				typeof(MeshRenderer)
			});
			Transform transform = gameObject.transform;
			if (originalTransform != null)
			{
				UtilityServicesRuntime.ParentAndOffsetTransform(transform, parentTransform, originalTransform);
			}
			else
			{
				UtilityServicesRuntime.ParentAndResetTransform(transform, parentTransform);
			}
			gameObject.GetComponent<MeshFilter>().sharedMesh = mesh;
			MeshRenderer component = gameObject.GetComponent<MeshRenderer>();
			component.sharedMaterials = materials;
			return component;
		}

		public static SkinnedMeshRenderer CreateSkinnedLevelRenderer(string name, Transform parentTransform, Transform originalTransform, Mesh mesh, Material[] materials, Transform rootBone, Transform[] bones)
		{
			GameObject gameObject = new GameObject(name, new Type[]
			{
				typeof(SkinnedMeshRenderer)
			});
			Transform transform = gameObject.transform;
			if (originalTransform != null)
			{
				UtilityServicesRuntime.ParentAndOffsetTransform(transform, parentTransform, originalTransform);
			}
			else
			{
				UtilityServicesRuntime.ParentAndResetTransform(transform, parentTransform);
			}
			SkinnedMeshRenderer component = gameObject.GetComponent<SkinnedMeshRenderer>();
			component.sharedMesh = mesh;
			component.sharedMaterials = materials;
			component.rootBone = rootBone;
			component.bones = bones;
			return component;
		}

		private static void CollectChildRenderersForCombining(Transform transform, List<Renderer> resultRenderers, bool skipInactiveChildObjects)
		{
			Renderer[] components = transform.GetComponents<Renderer>();
			resultRenderers.AddRange(components);
			int childCount = transform.childCount;
			for (int i = 0; i < childCount; i++)
			{
				Transform child = transform.GetChild(i);
				if (skipInactiveChildObjects && !child.gameObject.activeSelf)
				{
					Debug.LogWarning("No Renderers under the GameObject \"" + transform.name + "\" combined because the object was inactive and was skipped entirely.");
				}
				else if (child.GetComponent<LODGroup>() != null)
				{
					Debug.LogWarning("No Renderers under the GameObject \"" + transform.name + "\" combined because the object had LOD groups and was skipped entirely.");
				}
				else
				{
					UtilityServicesRuntime.CollectChildRenderersForCombining(child, resultRenderers, skipInactiveChildObjects);
				}
			}
		}

		private static void ParentAndResetTransform(Transform transform, Transform parentTransform)
		{
			transform.SetParent(parentTransform);
			transform.localPosition = Vector3.zero;
			transform.localRotation = Quaternion.identity;
			transform.localScale = Vector3.one;
		}

		public static void ParentAndOffsetTransform(Transform transform, Transform parentTransform, Transform originalTransform)
		{
			transform.position = originalTransform.position;
			transform.rotation = originalTransform.rotation;
			transform.localScale = originalTransform.lossyScale;
			transform.SetParent(parentTransform, true);
		}

		public class OBJExporterImporter
		{
			private void InitializeExporter(GameObject toExport, string exportPath, PolyfewRuntime.OBJExportOptions exportOptions)
			{
				this.exportPath = exportPath;
				if (string.IsNullOrWhiteSpace(exportPath))
				{
					throw new DirectoryNotFoundException("The path provided is non-existant.");
				}
				exportPath = Path.GetFullPath(exportPath);
				if (exportPath[exportPath.Length - 1] == '\\')
				{
					exportPath = exportPath.Remove(exportPath.Length - 1);
				}
				else if (exportPath[exportPath.Length - 1] == '/')
				{
					exportPath = exportPath.Remove(exportPath.Length - 1);
				}
				if (!Directory.Exists(exportPath))
				{
					throw new DirectoryNotFoundException("The path provided is non-existant.");
				}
				if (toExport == null)
				{
					throw new ArgumentNullException("toExport", "Please provide a GameObject to export as OBJ file.");
				}
				this.meshRenderer = toExport.GetComponent<MeshRenderer>();
				this.meshFilter = toExport.GetComponent<MeshFilter>();
				if (!(this.meshRenderer == null) && this.meshRenderer.isPartOfStaticBatch)
				{
					throw new InvalidOperationException("The provided object is static batched. Static batched object cannot be exported. Please disable it before trying to export the object.");
				}
				if (this.meshFilter == null)
				{
					throw new InvalidOperationException("There is no MeshFilter attached to the provided GameObject.");
				}
				this.meshToExport = this.meshFilter.sharedMesh;
				if (this.meshToExport == null || this.meshToExport.triangles == null || this.meshToExport.triangles.Length == 0)
				{
					throw new InvalidOperationException("The MeshFilter on the provided GameObject has invalid or no mesh at all.");
				}
				if (exportOptions != null)
				{
					this.applyPosition = exportOptions.applyPosition;
					this.applyRotation = exportOptions.applyRotation;
					this.applyScale = exportOptions.applyScale;
					this.generateMaterials = exportOptions.generateMaterials;
					this.exportTextures = exportOptions.exportTextures;
				}
			}

			private void InitializeExporter(Mesh toExport, string exportPath)
			{
				this.exportPath = exportPath;
				if (string.IsNullOrWhiteSpace(exportPath))
				{
					throw new DirectoryNotFoundException("The path provided is non-existant.");
				}
				if (!Directory.Exists(exportPath))
				{
					throw new DirectoryNotFoundException("The path provided is non-existant.");
				}
				if (toExport == null)
				{
					throw new ArgumentNullException("toExport", "Please provide a Mesh to export as OBJ file.");
				}
				this.meshToExport = toExport;
				if (this.meshToExport == null || this.meshToExport.triangles == null || this.meshToExport.triangles.Length == 0)
				{
					throw new InvalidOperationException("The MeshFilter on the provided GameObject has invalid or no mesh at all.");
				}
			}

			private Vector3 RotateAroundPoint(Vector3 point, Vector3 pivot, Quaternion angle)
			{
				return angle * (point - pivot) + pivot;
			}

			private Vector3 MultiplyVec3s(Vector3 v1, Vector3 v2)
			{
				return new Vector3(v1.x * v2.x, v1.y * v2.y, v1.z * v2.z);
			}

			public void ExportGameObjectToOBJ(GameObject toExport, string exportPath, PolyfewRuntime.OBJExportOptions exportOptions = null, Action OnSuccess = null)
			{
				if (Application.platform == 17)
				{
					Debug.LogWarning("The function cannot run on WebGL player. As web apps cannot read from or write to local file system.");
					return;
				}
				Dictionary<string, bool> dictionary = new Dictionary<string, bool>();
				this.InitializeExporter(toExport, exportPath, exportOptions);
				string name = toExport.gameObject.name;
				StringBuilder stringBuilder = new StringBuilder();
				StringBuilder stringBuilder2 = new StringBuilder();
				if (this.generateMaterials)
				{
					stringBuilder.AppendLine("mtllib " + name + ".mtl");
				}
				int num = 0;
				if (this.meshRenderer != null && this.generateMaterials)
				{
					foreach (Material material in this.meshRenderer.sharedMaterials)
					{
						if (!dictionary.ContainsKey(material.name))
						{
							dictionary[material.name] = true;
							stringBuilder2.Append(this.MaterialToString(material));
							stringBuilder2.AppendLine();
						}
					}
				}
				int num2 = (int)Mathf.Clamp(toExport.gameObject.transform.lossyScale.x * toExport.gameObject.transform.lossyScale.z, -1f, 1f);
				foreach (Vector3 vector in this.meshToExport.vertices)
				{
					if (this.applyScale)
					{
						vector = this.MultiplyVec3s(vector, toExport.gameObject.transform.lossyScale);
					}
					if (this.applyRotation)
					{
						vector = this.RotateAroundPoint(vector, Vector3.zero, toExport.gameObject.transform.rotation);
					}
					if (this.applyPosition)
					{
						vector += toExport.gameObject.transform.position;
					}
					vector.x *= -1f;
					stringBuilder.AppendLine(string.Concat(new object[]
					{
						"v ",
						vector.x,
						" ",
						vector.y,
						" ",
						vector.z
					}));
				}
				foreach (Vector3 vector2 in this.meshToExport.normals)
				{
					if (this.applyScale)
					{
						vector2 = this.MultiplyVec3s(vector2, toExport.gameObject.transform.lossyScale.normalized);
					}
					if (this.applyRotation)
					{
						vector2 = this.RotateAroundPoint(vector2, Vector3.zero, toExport.gameObject.transform.rotation);
					}
					vector2.x *= -1f;
					stringBuilder.AppendLine(string.Concat(new object[]
					{
						"vn ",
						vector2.x,
						" ",
						vector2.y,
						" ",
						vector2.z
					}));
				}
				foreach (Vector2 vector3 in this.meshToExport.uv)
				{
					stringBuilder.AppendLine(string.Concat(new object[]
					{
						"vt ",
						vector3.x,
						" ",
						vector3.y
					}));
				}
				for (int k = 0; k < this.meshToExport.subMeshCount; k++)
				{
					if (this.meshRenderer != null && k < this.meshRenderer.sharedMaterials.Length)
					{
						string name2 = this.meshRenderer.sharedMaterials[k].name;
						stringBuilder.AppendLine("usemtl " + name2);
					}
					else
					{
						stringBuilder.AppendLine(string.Concat(new object[]
						{
							"usemtl ",
							name,
							"_sm",
							k
						}));
					}
					int[] triangles = this.meshToExport.GetTriangles(k);
					for (int l = 0; l < triangles.Length; l += 3)
					{
						int index = triangles[l] + 1 + num;
						int index2 = triangles[l + 1] + 1 + num;
						int index3 = triangles[l + 2] + 1 + num;
						if (num2 < 0)
						{
							stringBuilder.AppendLine(string.Concat(new string[]
							{
								"f ",
								this.ConstructOBJString(index),
								" ",
								this.ConstructOBJString(index2),
								" ",
								this.ConstructOBJString(index3)
							}));
						}
						else
						{
							stringBuilder.AppendLine(string.Concat(new string[]
							{
								"f ",
								this.ConstructOBJString(index3),
								" ",
								this.ConstructOBJString(index2),
								" ",
								this.ConstructOBJString(index)
							}));
						}
					}
				}
				num += this.meshToExport.vertices.Length;
				File.WriteAllText(Path.Combine(exportPath, name + ".obj"), stringBuilder.ToString());
				if (this.generateMaterials)
				{
					File.WriteAllText(Path.Combine(exportPath, name + ".mtl"), stringBuilder2.ToString());
				}
				if (OnSuccess != null)
				{
					OnSuccess();
				}
			}

			public async Task ExportMeshToOBJ(Mesh mesh, string exportPath)
			{
				this.InitializeExporter(mesh, exportPath);
				string objectName = this.meshToExport.name;
				StringBuilder sb = new StringBuilder();
				int lastIndex = 0;
				int faceOrder = 1;
				foreach (Vector3 vx in this.meshToExport.vertices)
				{
					await Task.Delay(1);
					Vector3 vector = vx;
					vector.x *= -1f;
					sb.AppendLine(string.Concat(new object[]
					{
						"v ",
						vector.x,
						" ",
						vector.y,
						" ",
						vector.z
					}));
					vx = default(Vector3);
				}
				Vector3[] array = null;
				foreach (Vector3 vx in this.meshToExport.normals)
				{
					await Task.Delay(1);
					Vector3 vector2 = vx;
					vector2.x *= -1f;
					sb.AppendLine(string.Concat(new object[]
					{
						"vn ",
						vector2.x,
						" ",
						vector2.y,
						" ",
						vector2.z
					}));
					vx = default(Vector3);
				}
				array = null;
				foreach (Vector2 v in this.meshToExport.uv)
				{
					await Task.Delay(1);
					sb.AppendLine(string.Concat(new object[]
					{
						"vt ",
						v.x,
						" ",
						v.y
					}));
					v = default(Vector2);
				}
				Vector2[] array2 = null;
				for (int i = 0; i < this.meshToExport.subMeshCount; i++)
				{
					await Task.Delay(1);
					sb.AppendLine(string.Concat(new object[]
					{
						"usemtl ",
						objectName,
						"_sm",
						i
					}));
					int[] tris = this.meshToExport.GetTriangles(i);
					for (int t = 0; t < tris.Length; t += 3)
					{
						await Task.Delay(1);
						int index = tris[t] + 1 + lastIndex;
						int index2 = tris[t + 1] + 1 + lastIndex;
						int index3 = tris[t + 2] + 1 + lastIndex;
						if (faceOrder < 0)
						{
							sb.AppendLine(string.Concat(new string[]
							{
								"f ",
								this.ConstructOBJString(index),
								" ",
								this.ConstructOBJString(index2),
								" ",
								this.ConstructOBJString(index3)
							}));
						}
						else
						{
							sb.AppendLine(string.Concat(new string[]
							{
								"f ",
								this.ConstructOBJString(index3),
								" ",
								this.ConstructOBJString(index2),
								" ",
								this.ConstructOBJString(index)
							}));
						}
					}
					tris = null;
				}
				lastIndex += this.meshToExport.vertices.Length;
				File.WriteAllText(Path.Combine(exportPath, objectName + ".obj"), sb.ToString());
			}

			private string TryExportTexture(string propertyName, Material m, string exportPath)
			{
				if (m.HasProperty(propertyName))
				{
					Texture texture = m.GetTexture(propertyName);
					if (texture != null)
					{
						return this.ExportTexture((Texture2D)texture, exportPath);
					}
				}
				return "false";
			}

			private string ExportTexture(Texture2D t, string exportPath)
			{
				string name = t.name;
				string result;
				try
				{
					Color32[] pixels = null;
					try
					{
						pixels = t.GetPixels32();
					}
					catch (UnityException)
					{
						t = UtilityServicesRuntime.DuplicateTexture(t);
						pixels = t.GetPixels32();
					}
					string text = Path.Combine(exportPath, name + ".png");
					Texture2D texture2D = new Texture2D(t.width, t.height, 5, false);
					texture2D.SetPixels32(pixels);
					File.WriteAllBytes(text, ImageConversion.EncodeToPNG(texture2D));
					result = text;
				}
				catch (Exception)
				{
					Debug.Log("Could not export texture : " + t.name + ". is it readable?");
					result = "null";
				}
				return result;
			}

			private string ConstructOBJString(int index)
			{
				string text = index.ToString();
				return string.Concat(new string[]
				{
					text,
					"/",
					text,
					"/",
					text
				});
			}

			private string MaterialToString(Material m)
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.AppendLine("newmtl " + m.name);
				if (m.HasProperty("_Color"))
				{
					StringBuilder stringBuilder2 = stringBuilder;
					string[] array = new string[6];
					array[0] = "Kd ";
					int num = 1;
					Color color = m.color;
					array[num] = color.r.ToString();
					array[2] = " ";
					int num2 = 3;
					color = m.color;
					array[num2] = color.g.ToString();
					array[4] = " ";
					int num3 = 5;
					color = m.color;
					array[num3] = color.b.ToString();
					stringBuilder2.AppendLine(string.Concat(array));
					if (m.color.a < 1f)
					{
						stringBuilder.AppendLine("Tr " + (1f - m.color.a).ToString());
						StringBuilder stringBuilder3 = stringBuilder;
						string str = "d ";
						color = m.color;
						stringBuilder3.AppendLine(str + color.a.ToString());
					}
				}
				if (m.HasProperty("_SpecColor"))
				{
					Color color2 = m.GetColor("_SpecColor");
					stringBuilder.AppendLine(string.Concat(new string[]
					{
						"Ks ",
						color2.r.ToString(),
						" ",
						color2.g.ToString(),
						" ",
						color2.b.ToString()
					}));
				}
				if (this.exportTextures)
				{
					string text = this.TryExportTexture("_MainTex", m, this.exportPath);
					if (text != "false")
					{
						stringBuilder.AppendLine("map_Kd " + text);
					}
					text = this.TryExportTexture("_SpecMap", m, this.exportPath);
					if (text != "false")
					{
						stringBuilder.AppendLine("map_Ks " + text);
					}
					text = this.TryExportTexture("_BumpMap", m, this.exportPath);
					if (text != "false")
					{
						stringBuilder.AppendLine("map_Bump " + text);
					}
				}
				stringBuilder.AppendLine("illum 2");
				return stringBuilder.ToString();
			}

			public async Task ImportFromLocalFileSystem(string objPath, string texturesFolderPath, string materialsFolderPath, Action<GameObject> Callback, PolyfewRuntime.OBJImportOptions importOptions = null)
			{
				if (Application.platform == 17)
				{
					Debug.LogWarning("The function cannot run on WebGL player. As web apps cannot read from or write to local file system.");
				}
				else
				{
					if (!string.IsNullOrWhiteSpace(objPath))
					{
						objPath = Path.GetFullPath(objPath);
						if (objPath[objPath.Length - 1] == '\\')
						{
							objPath = objPath.Remove(objPath.Length - 1);
						}
						else if (objPath[objPath.Length - 1] == '/')
						{
							objPath = objPath.Remove(objPath.Length - 1);
						}
					}
					if (!string.IsNullOrWhiteSpace(texturesFolderPath))
					{
						texturesFolderPath = Path.GetFullPath(texturesFolderPath);
						if (texturesFolderPath[texturesFolderPath.Length - 1] == '\\')
						{
							texturesFolderPath = texturesFolderPath.Remove(texturesFolderPath.Length - 1);
						}
						else if (texturesFolderPath[texturesFolderPath.Length - 1] == '/')
						{
							texturesFolderPath = texturesFolderPath.Remove(texturesFolderPath.Length - 1);
						}
					}
					if (!string.IsNullOrWhiteSpace(materialsFolderPath))
					{
						materialsFolderPath = Path.GetFullPath(materialsFolderPath);
						if (materialsFolderPath[materialsFolderPath.Length - 1] == '\\')
						{
							materialsFolderPath = materialsFolderPath.Remove(materialsFolderPath.Length - 1);
						}
						else if (materialsFolderPath[materialsFolderPath.Length - 1] == '/')
						{
							materialsFolderPath = materialsFolderPath.Remove(materialsFolderPath.Length - 1);
						}
					}
					if (!File.Exists(objPath))
					{
						throw new FileNotFoundException("The path provided doesn't point to a file. The path might be invalid or the file is non-existant.");
					}
					if (!string.IsNullOrWhiteSpace(texturesFolderPath) && !Directory.Exists(texturesFolderPath))
					{
						Debug.LogWarning("The directory pointed to by the given path for textures is non-existant.");
					}
					if (!string.IsNullOrWhiteSpace(materialsFolderPath) && !Directory.Exists(materialsFolderPath))
					{
						Debug.LogWarning("The directory pointed to by the given path for materials is non-existant.");
					}
					string fileName = Path.GetFileName(objPath);
					string text = Path.GetDirectoryName(objPath);
					string objName = fileName.Split(new char[]
					{
						'.'
					})[0];
					GameObject objectToPopulate = new GameObject();
					objectToPopulate.AddComponent<ObjectImporter>();
					ObjectImporter objImporter = objectToPopulate.GetComponent<ObjectImporter>();
					if (text.Contains("/") && !text.EndsWith("/"))
					{
						text += "/";
					}
					else if (!text.EndsWith("\\"))
					{
						text += "\\";
					}
					if (fileName.Split(new char[]
					{
						'.'
					})[1].ToLower() != "obj")
					{
						Object.DestroyImmediate(objectToPopulate);
						throw new InvalidOperationException("The path provided must point to a wavefront obj file.");
					}
					if (importOptions == null)
					{
						importOptions = new PolyfewRuntime.OBJImportOptions();
					}
					try
					{
						GameObject obj = await objImporter.ImportModelAsync(objName, objPath, null, importOptions, texturesFolderPath, materialsFolderPath);
						Object.Destroy(objImporter);
						Callback(obj);
					}
					catch (Exception ex)
					{
						Object.DestroyImmediate(objectToPopulate);
						throw ex;
					}
				}
			}

			public async void ImportFromNetwork(string objURL, string objName, string diffuseTexURL, string bumpTexURL, string specularTexURL, string opacityTexURL, string materialURL, PolyfewRuntime.ReferencedNumeric<float> downloadProgress, Action<GameObject> OnSuccess, Action<Exception> OnError, PolyfewRuntime.OBJImportOptions importOptions = null)
			{
				if (string.IsNullOrWhiteSpace(objURL))
				{
					throw new InvalidOperationException("Cannot download from empty URL. Please provide a direct URL to the obj file");
				}
				if (string.IsNullOrWhiteSpace(diffuseTexURL))
				{
					Debug.LogWarning("Cannot download from empty URL. Please provide a direct URL to the accompanying texture file.");
				}
				if (string.IsNullOrWhiteSpace(materialURL))
				{
					Debug.LogWarning("Cannot download from empty URL. Please provide a direct URL to the accompanying material file.");
				}
				if (downloadProgress == null)
				{
					throw new ArgumentNullException("downloadProgress", "You must pass a reference to the Download Progress object.");
				}
				GameObject objectToPopulate = new GameObject();
				objectToPopulate.AddComponent<ObjectImporter>();
				ObjectImporter objImporter = objectToPopulate.GetComponent<ObjectImporter>();
				if (importOptions == null)
				{
					importOptions = new PolyfewRuntime.OBJImportOptions();
				}
				try
				{
					GameObject obj = await objImporter.ImportModelFromNetwork(objURL, objName, diffuseTexURL, bumpTexURL, specularTexURL, opacityTexURL, materialURL, downloadProgress, importOptions);
					Object.Destroy(objImporter);
					OnSuccess(obj);
				}
				catch (Exception obj2)
				{
					Object.DestroyImmediate(objectToPopulate);
					OnError(obj2);
				}
			}

			public async void ImportFromNetworkWebGL(string objURL, string objName, string diffuseTexURL, string bumpTexURL, string specularTexURL, string opacityTexURL, string materialURL, PolyfewRuntime.ReferencedNumeric<float> downloadProgress, Action<GameObject> OnSuccess, Action<Exception> OnError, PolyfewRuntime.OBJImportOptions importOptions = null)
			{
				if (string.IsNullOrWhiteSpace(objURL))
				{
					OnError(new InvalidOperationException("Cannot download from empty URL. Please provide a direct URL to the obj file"));
				}
				else
				{
					if (string.IsNullOrWhiteSpace(diffuseTexURL))
					{
						Debug.LogWarning("Cannot download from empty URL. Please provide a direct URL to the accompanying texture file.");
					}
					if (string.IsNullOrWhiteSpace(materialURL))
					{
						Debug.LogWarning("Cannot download from empty URL. Please provide a direct URL to the accompanying material file.");
					}
					if (downloadProgress == null)
					{
						OnError(new ArgumentNullException("downloadProgress", "You must pass a reference to the Download Progress object."));
					}
					else
					{
						GameObject objectToPopulate = new GameObject();
						objectToPopulate.AddComponent<ObjectImporter>();
						ObjectImporter objImporter = objectToPopulate.GetComponent<ObjectImporter>();
						if (importOptions == null)
						{
							importOptions = new PolyfewRuntime.OBJImportOptions();
						}
						objImporter.ImportModelFromNetworkWebGL(objURL, objName, diffuseTexURL, bumpTexURL, specularTexURL, opacityTexURL, materialURL, downloadProgress, importOptions, delegate(GameObject imported)
						{
							Object.Destroy(objImporter);
							OnSuccess(imported);
						}, delegate(Exception exception)
						{
							Object.DestroyImmediate(objectToPopulate);
							OnError(exception);
						});
					}
				}
			}

			private bool applyPosition = true;

			private bool applyRotation = true;

			private bool applyScale = true;

			private bool generateMaterials = true;

			private bool exportTextures = true;

			private string exportPath;

			private MeshFilter meshFilter;

			private Mesh meshToExport;

			private MeshRenderer meshRenderer;
		}
	}
}
