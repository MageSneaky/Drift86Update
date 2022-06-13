using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace AsImpL
{
	public class ObjectBuilder
	{
		public Dictionary<string, Material> ImportedMaterials
		{
			get
			{
				return this.currMaterials;
			}
		}

		public int NumImportedMaterials
		{
			get
			{
				if (this.currMaterials == null)
				{
					return 0;
				}
				return this.currMaterials.Count;
			}
		}

		public void InitBuildMaterials(List<MaterialData> materialData, bool hasColors)
		{
			this.materialData = materialData;
			this.currMaterials = new Dictionary<string, Material>();
			if (materialData == null || materialData.Count == 0)
			{
				string name = "VertexLit";
				if (hasColors)
				{
					name = "Unlit/Simple Vertex Colors Shader";
					if (Shader.Find(name) == null)
					{
						name = "Mobile/Particles/Alpha Blended";
					}
					Debug.Log("No material library defined. Using vertex colors.");
				}
				else
				{
					Debug.LogWarning("No material library defined. Using a default material.");
				}
				this.currMaterials.Add("default", new Material(Shader.Find(name)));
			}
		}

		public bool BuildMaterials(ObjectBuilder.ProgressInfo info)
		{
			if (this.materialData == null)
			{
				Debug.LogWarning("No material library defined.");
				return false;
			}
			if (info.materialsLoaded >= this.materialData.Count)
			{
				return false;
			}
			MaterialData materialData = this.materialData[info.materialsLoaded];
			info.materialsLoaded++;
			if (this.currMaterials.ContainsKey(materialData.materialName))
			{
				Debug.LogWarning("Duplicate material found: " + materialData.materialName + ". Repeated occurence ignored");
			}
			else
			{
				this.currMaterials.Add(materialData.materialName, this.BuildMaterial(materialData));
			}
			return info.materialsLoaded < this.materialData.Count;
		}

		public void StartBuildObjectAsync(DataSet dataSet, GameObject parentObj, Dictionary<string, Material> materials = null)
		{
			this.currDataSet = dataSet;
			this.currParentObj = parentObj;
			if (materials != null)
			{
				this.currMaterials = materials;
			}
		}

		public bool BuildObjectAsync(ref ObjectBuilder.ProgressInfo info)
		{
			bool result = this.BuildNextObject(this.currParentObj, this.currMaterials);
			info.objectsLoaded = this.buildStatus.objCount;
			info.groupsLoaded = this.buildStatus.subObjCount;
			info.numGroups = this.buildStatus.numGroups;
			return result;
		}

		public static void Solve(Mesh origMesh)
		{
			if (origMesh.uv == null || origMesh.uv.Length == 0)
			{
				Debug.LogWarning("Unable to compute tangent space vectors - texture coordinates not defined.");
				return;
			}
			if (origMesh.vertices == null || origMesh.vertices.Length == 0)
			{
				Debug.LogWarning("Unable to compute tangent space vectors - vertices not defined.");
				return;
			}
			if (origMesh.normals == null || origMesh.normals.Length == 0)
			{
				Debug.LogWarning("Unable to compute tangent space vectors - normals not defined.");
				return;
			}
			if (origMesh.triangles == null || origMesh.triangles.Length == 0)
			{
				Debug.LogWarning("Unable to compute tangent space vectors - triangles not defined.");
				return;
			}
			Vector3[] vertices = origMesh.vertices;
			Vector3[] normals = origMesh.normals;
			Vector2[] uv = origMesh.uv;
			int[] triangles = origMesh.triangles;
			int num = origMesh.triangles.Length;
			int num2 = -1;
			for (int i = 0; i < triangles.Length; i++)
			{
				if (num2 < triangles[i])
				{
					num2 = triangles[i];
				}
			}
			if (vertices.Length <= num2)
			{
				Debug.LogWarning("Unable to compute tangent space vectors - not enough vertices: " + vertices.Length.ToString());
				return;
			}
			if (normals.Length <= num2)
			{
				Debug.LogWarning("Unable to compute tangent space vectors - not enough normals.");
				return;
			}
			if (uv.Length <= num2)
			{
				Debug.LogWarning("Unable to compute tangent space vectors - not enough UVs.");
				return;
			}
			int vertexCount = origMesh.vertexCount;
			Vector4[] array = new Vector4[vertexCount];
			Vector3[] array2 = new Vector3[vertexCount];
			Vector3[] array3 = new Vector3[vertexCount];
			int num3 = triangles.Length / 3;
			int num4 = 0;
			for (int j = 0; j < num3; j++)
			{
				int num5 = triangles[num4];
				int num6 = triangles[num4 + 1];
				int num7 = triangles[num4 + 2];
				Vector3 vector = vertices[num5];
				Vector3 vector2 = vertices[num6];
				Vector3 vector3 = vertices[num7];
				Vector2 vector4 = uv[num5];
				Vector2 vector5 = uv[num6];
				Vector2 vector6 = uv[num7];
				float num8 = vector2.x - vector.x;
				float num9 = vector3.x - vector.x;
				float num10 = vector2.y - vector.y;
				float num11 = vector3.y - vector.y;
				float num12 = vector2.z - vector.z;
				float num13 = vector3.z - vector.z;
				float num14 = vector5.x - vector4.x;
				float num15 = vector6.x - vector4.x;
				float num16 = vector5.y - vector4.y;
				float num17 = vector6.y - vector4.y;
				float num18 = 1f / (num14 * num17 - num15 * num16);
				Vector3 b = new Vector3((num17 * num8 - num16 * num9) * num18, (num17 * num10 - num16 * num11) * num18, (num17 * num12 - num16 * num13) * num18);
				Vector3 b2 = new Vector3((num14 * num9 - num15 * num8) * num18, (num14 * num11 - num15 * num10) * num18, (num14 * num13 - num15 * num12) * num18);
				array2[num5] += b;
				array2[num6] += b;
				array2[num7] += b;
				array3[num5] += b2;
				array3[num6] += b2;
				array3[num7] += b2;
				num4 += 3;
			}
			for (int k = 0; k < vertexCount; k++)
			{
				Vector3 lhs = normals[k];
				Vector3 vector7 = array2[k];
				Vector3.OrthoNormalize(ref lhs, ref vector7);
				array[k].x = vector7.x;
				array[k].y = vector7.y;
				array[k].z = vector7.z;
				array[k].w = ((Vector3.Dot(Vector3.Cross(lhs, vector7), array3[k]) < 0f) ? -1f : 1f);
			}
			origMesh.tangents = array;
		}

		public static void BuildMeshCollider(GameObject targetObject, bool convex = false, bool isTrigger = false, bool inflateMesh = false, float skinWidth = 0.01f)
		{
			MeshFilter component = targetObject.GetComponent<MeshFilter>();
			if (component != null && component.sharedMesh != null)
			{
				Mesh sharedMesh = component.sharedMesh;
				MeshCollider meshCollider = targetObject.AddComponent<MeshCollider>();
				meshCollider.sharedMesh = sharedMesh;
				if (convex)
				{
					meshCollider.convex = convex;
					meshCollider.isTrigger = isTrigger;
				}
			}
		}

		protected bool BuildNextObject(GameObject parentObj, Dictionary<string, Material> mats)
		{
			if (this.buildStatus.objCount >= this.currDataSet.objectList.Count)
			{
				return false;
			}
			DataSet.ObjectData objectData = this.currDataSet.objectList[this.buildStatus.objCount];
			if (this.buildStatus.newObject)
			{
				if (this.buildStatus.objCount == 0 && objectData.name == "default")
				{
					this.buildStatus.currObjGameObject = parentObj;
				}
				else
				{
					this.buildStatus.currObjGameObject = new GameObject();
					this.buildStatus.currObjGameObject.transform.parent = parentObj.transform;
					this.buildStatus.currObjGameObject.name = objectData.name;
					this.buildStatus.currObjGameObject.transform.localScale = Vector3.one;
				}
				this.buildStatus.subObjParent = this.buildStatus.currObjGameObject;
				this.buildStatus.newObject = false;
				this.buildStatus.subObjCount = 0;
				this.buildStatus.idxCount = 0;
				this.buildStatus.grpIdx = 0;
				this.buildStatus.grpFaceIdx = 0;
				this.buildStatus.meshPartIdx = 0;
				this.buildStatus.totFaceIdxCount = 0;
				this.buildStatus.numGroups = Mathf.Max(1, objectData.faceGroups.Count);
			}
			bool flag = true;
			if (this.Using32bitIndices())
			{
				flag = false;
			}
			bool flag2 = false;
			DataSet.FaceGroupData faceGroupData = new DataSet.FaceGroupData();
			faceGroupData.name = objectData.faceGroups[this.buildStatus.grpIdx].name;
			faceGroupData.materialName = objectData.faceGroups[this.buildStatus.grpIdx].materialName;
			DataSet.ObjectData objectData2 = new DataSet.ObjectData();
			objectData2.hasNormals = objectData.hasNormals;
			objectData2.hasColors = objectData.hasColors;
			HashSet<int> hashSet = new HashSet<int>();
			int num = (this.buildOptions != null && this.buildOptions.convertToDoubleSided) ? (ObjectBuilder.MAX_INDICES_LIMIT_FOR_A_MESH / 2) : ObjectBuilder.MAX_INDICES_LIMIT_FOR_A_MESH;
			for (int i = this.buildStatus.grpFaceIdx; i < objectData.faceGroups[this.buildStatus.grpIdx].faces.Count; i++)
			{
				if (flag && (hashSet.Count / 3 > ObjectBuilder.MAX_VERT_COUNT / 3 || objectData2.allFaces.Count / 3 > num / 3))
				{
					flag2 = true;
					this.buildStatus.grpFaceIdx = i;
					Debug.LogWarningFormat("Maximum vertex number for a mesh exceeded.\nSplitting object {0} (group {1}, starting from index {2})...", new object[]
					{
						faceGroupData.name,
						this.buildStatus.grpIdx,
						i
					});
					break;
				}
				DataSet.FaceIndices faceIndices = objectData.faceGroups[this.buildStatus.grpIdx].faces[i];
				objectData2.allFaces.Add(faceIndices);
				faceGroupData.faces.Add(faceIndices);
				hashSet.Add(faceIndices.vertIdx);
			}
			if (flag2 || this.buildStatus.meshPartIdx > 0)
			{
				this.buildStatus.meshPartIdx++;
			}
			if (this.buildStatus.meshPartIdx == 1)
			{
				GameObject gameObject = new GameObject();
				gameObject.transform.SetParent(this.buildStatus.currObjGameObject.transform, false);
				gameObject.name = faceGroupData.name;
				this.buildStatus.subObjParent = gameObject;
			}
			if (this.buildStatus.meshPartIdx > 0)
			{
				faceGroupData.name = this.buildStatus.subObjParent.name + "_MeshPart" + this.buildStatus.meshPartIdx;
			}
			objectData2.name = faceGroupData.name;
			objectData2.faceGroups.Add(faceGroupData);
			this.buildStatus.idxCount += objectData2.allFaces.Count;
			if (!flag2)
			{
				this.buildStatus.grpFaceIdx = 0;
				this.buildStatus.grpIdx++;
			}
			this.buildStatus.totFaceIdxCount += objectData2.allFaces.Count;
			if (this.ImportSubObject(this.buildStatus.subObjParent, objectData2, mats) == null)
			{
				Debug.LogWarningFormat("Error loading sub object n.{0}.", new object[]
				{
					this.buildStatus.subObjCount
				});
			}
			this.buildStatus.subObjCount++;
			if (this.buildStatus.totFaceIdxCount >= objectData.allFaces.Count || this.buildStatus.grpIdx >= objectData.faceGroups.Count)
			{
				if (this.buildStatus.totFaceIdxCount != objectData.allFaces.Count)
				{
					Debug.LogWarningFormat("Imported face indices: {0} of {1}", new object[]
					{
						this.buildStatus.totFaceIdxCount,
						objectData.allFaces.Count
					});
					return false;
				}
				this.buildStatus.objCount++;
				this.buildStatus.newObject = true;
			}
			return true;
		}

		private GameObject ImportSubObject(GameObject parentObj, DataSet.ObjectData objData, Dictionary<string, Material> mats)
		{
			bool flag = this.buildOptions != null && this.buildOptions.convertToDoubleSided;
			GameObject gameObject = new GameObject();
			gameObject.name = objData.name;
			int num = 0;
			if (parentObj.transform)
			{
				while (parentObj.transform.Find(gameObject.name))
				{
					num++;
					gameObject.name = objData.name + num;
				}
			}
			gameObject.transform.SetParent(parentObj.transform, false);
			if (objData.allFaces.Count == 0)
			{
				throw new InvalidOperationException("Failed to parse vertex and uv data. It might be that the file is corrupt or is not a valid wavefront OBJ file.");
			}
			Dictionary<string, int> dictionary = new Dictionary<string, int>();
			int num2 = 0;
			foreach (DataSet.FaceIndices fi in objData.allFaces)
			{
				string faceIndicesKey = DataSet.GetFaceIndicesKey(fi);
				int num3;
				if (!dictionary.TryGetValue(faceIndicesKey, out num3))
				{
					dictionary.Add(faceIndicesKey, num2);
					num2++;
				}
			}
			int num4 = flag ? (num2 * 2) : num2;
			Vector3[] array = new Vector3[num4];
			Vector2[] array2 = new Vector2[num4];
			Vector3[] array3 = new Vector3[num4];
			Color32[] array4 = new Color32[num4];
			bool flag2 = this.currDataSet.colorList.Count > 0;
			foreach (DataSet.FaceIndices faceIndices in objData.allFaces)
			{
				string faceIndicesKey2 = DataSet.GetFaceIndicesKey(faceIndices);
				int num5 = dictionary[faceIndicesKey2];
				array[num5] = this.currDataSet.vertList[faceIndices.vertIdx];
				if (flag)
				{
					array[num2 + num5] = array[num5];
				}
				if (flag2)
				{
					array4[num5] = this.currDataSet.colorList[faceIndices.vertIdx];
					if (flag)
					{
						array4[num2 + num5] = array4[num5];
					}
				}
				if (this.currDataSet.uvList.Count > 0)
				{
					array2[num5] = this.currDataSet.uvList[faceIndices.uvIdx];
					if (flag)
					{
						array2[num2 + num5] = array2[num5];
					}
				}
				if (this.currDataSet.normalList.Count > 0 && faceIndices.normIdx >= 0)
				{
					array3[num5] = this.currDataSet.normalList[faceIndices.normIdx];
					if (flag)
					{
						array3[num2 + num5] = -array3[num5];
					}
				}
			}
			bool flag3 = this.currDataSet.normalList.Count > 0 && objData.hasNormals;
			bool flag4 = this.currDataSet.colorList.Count > 0 && objData.hasColors;
			bool flag5 = this.currDataSet.uvList.Count > 0;
			int count = objData.faceGroups[0].faces.Count;
			int num6 = flag ? (count * 2) : count;
			MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
			gameObject.AddComponent<MeshRenderer>();
			Mesh mesh = new Mesh();
			if (this.Using32bitIndices() && (num4 > ObjectBuilder.MAX_VERT_COUNT || num6 > ObjectBuilder.MAX_INDICES_LIMIT_FOR_A_MESH))
			{
				mesh.indexFormat = IndexFormat.UInt32;
			}
			mesh.name = gameObject.name;
			meshFilter.sharedMesh = mesh;
			mesh.vertices = array;
			if (flag5)
			{
				mesh.uv = array2;
			}
			if (flag3)
			{
				mesh.normals = array3;
			}
			if (flag4)
			{
				mesh.colors32 = array4;
			}
			string text = (objData.faceGroups[0].materialName != null) ? objData.faceGroups[0].materialName : "default";
			Renderer component = gameObject.GetComponent<Renderer>();
			if (mats.ContainsKey(text))
			{
				Material sharedMaterial = mats[text];
				component.sharedMaterial = sharedMaterial;
				component.UpdateGIMaterials();
			}
			else if (mats.ContainsKey("default"))
			{
				Material sharedMaterial = mats["default"];
				component.sharedMaterial = sharedMaterial;
				Debug.LogWarning("Material: " + text + " not found. Using the default material.");
			}
			else
			{
				Debug.LogError("Material: " + text + " not found.");
			}
			int[] array5 = new int[num6];
			for (int i = 0; i < count; i++)
			{
				string faceIndicesKey3 = DataSet.GetFaceIndicesKey(objData.faceGroups[0].faces[i]);
				array5[i] = dictionary[faceIndicesKey3];
			}
			if (flag)
			{
				for (int j = 0; j < count; j++)
				{
					array5[j + count] = num2 + array5[j / 3 * 3 + 2 - j % 3];
				}
			}
			mesh.SetTriangles(array5, 0);
			if (!flag3)
			{
				mesh.RecalculateNormals();
			}
			if (flag5)
			{
				ObjectBuilder.Solve(mesh);
			}
			if (this.buildOptions != null && this.buildOptions.buildColliders)
			{
				ObjectBuilder.BuildMeshCollider(gameObject, this.buildOptions.colliderConvex, this.buildOptions.colliderTrigger, false, 0.01f);
			}
			return gameObject;
		}

		private Material BuildMaterial(MaterialData md)
		{
			string name = "Standard";
			bool flag = false;
			ModelUtil.MtlBlendMode mode = (md.overallAlpha < 1f) ? ModelUtil.MtlBlendMode.TRANSPARENT : ModelUtil.MtlBlendMode.OPAQUE;
			object obj = this.buildOptions != null && this.buildOptions.litDiffuse && md.diffuseTex != null && md.bumpTex == null && md.opacityTex == null && md.specularTex == null && !md.hasReflectionTex;
			bool? flag2 = null;
			object obj2 = obj;
			if (obj2 != null)
			{
				flag2 = new bool?(ModelUtil.ScanTransparentPixels(md.diffuseTex, ref mode));
			}
			if (obj2 != null && !flag2.Value)
			{
				name = "Unlit/Texture";
			}
			else if (flag)
			{
				name = "Standard (Specular setup)";
			}
			Material material = new Material(Shader.Find(name));
			material.name = md.materialName;
			float num = Mathf.Log(md.shininess, 2f);
			float num2 = Mathf.Clamp01(num / 10f);
			float num3 = Mathf.Clamp01(num / 10f);
			if (flag)
			{
				material.SetColor("_SpecColor", md.specularColor);
				material.SetFloat("_Shininess", md.shininess / 1000f);
			}
			else
			{
				material.SetFloat("_Metallic", num2);
			}
			if (md.diffuseTex != null)
			{
				if (md.opacityTex != null)
				{
					int width = md.diffuseTex.width;
					int width2 = md.diffuseTex.width;
					Texture2D texture2D = new Texture2D(width, width2, TextureFormat.ARGB32, false);
					Color color = default(Color);
					for (int i = 0; i < texture2D.width; i++)
					{
						for (int j = 0; j < texture2D.height; j++)
						{
							color = md.diffuseTex.GetPixel(i, j);
							color.a *= md.opacityTex.GetPixel(i, j).grayscale;
							texture2D.SetPixel(i, j, color);
						}
					}
					texture2D.name = md.diffuseTexPath;
					texture2D.Apply();
					mode = ModelUtil.MtlBlendMode.FADE;
					material.SetTexture("_MainTex", texture2D);
				}
				else
				{
					if (flag2 == null)
					{
						flag2 = new bool?(ModelUtil.ScanTransparentPixels(md.diffuseTex, ref mode));
					}
					material.SetTexture("_MainTex", md.diffuseTex);
				}
			}
			else if (md.opacityTex != null)
			{
				mode = ModelUtil.MtlBlendMode.FADE;
				int width3 = md.opacityTex.width;
				int width4 = md.opacityTex.width;
				Texture2D texture2D2 = new Texture2D(width3, width4, TextureFormat.ARGB32, false);
				Color color2 = default(Color);
				bool flag3 = false;
				for (int k = 0; k < texture2D2.width; k++)
				{
					for (int l = 0; l < texture2D2.height; l++)
					{
						color2 = md.diffuseColor;
						color2.a = md.overallAlpha * md.opacityTex.GetPixel(k, l).grayscale;
						ModelUtil.DetectMtlBlendFadeOrCutout(color2.a, ref mode, ref flag3);
						texture2D2.SetPixel(k, l, color2);
					}
				}
				texture2D2.name = md.diffuseTexPath;
				texture2D2.Apply();
				material.SetTexture("_MainTex", texture2D2);
			}
			md.diffuseColor.a = md.overallAlpha;
			material.SetColor("_Color", md.diffuseColor);
			md.emissiveColor.a = md.overallAlpha;
			material.SetColor("_EmissionColor", md.emissiveColor);
			if (md.emissiveColor.r > 0f || md.emissiveColor.g > 0f || md.emissiveColor.b > 0f)
			{
				material.EnableKeyword("_EMISSION");
			}
			if (md.bumpTex != null)
			{
				if (md.bumpTexPath.Contains("_normal_map"))
				{
					material.EnableKeyword("_NORMALMAP");
					material.SetFloat("_BumpScale", 0.25f);
					material.SetTexture("_BumpMap", md.bumpTex);
				}
				else
				{
					Texture2D value = ModelUtil.HeightToNormalMap(md.bumpTex, 1f);
					material.SetTexture("_BumpMap", value);
					material.EnableKeyword("_NORMALMAP");
					material.SetFloat("_BumpScale", 1f);
				}
			}
			if (md.specularTex != null)
			{
				Texture2D texture2D3 = new Texture2D(md.specularTex.width, md.specularTex.height, TextureFormat.ARGB32, false);
				Color color3 = default(Color);
				for (int m = 0; m < texture2D3.width; m++)
				{
					for (int n = 0; n < texture2D3.height; n++)
					{
						float grayscale = md.specularTex.GetPixel(m, n).grayscale;
						color3.r = num2 * grayscale;
						color3.g = color3.r;
						color3.b = color3.r;
						if (md.hasReflectionTex)
						{
							color3.a = grayscale;
						}
						else
						{
							color3.a = grayscale * num3;
						}
						texture2D3.SetPixel(m, n, color3);
					}
				}
				texture2D3.Apply();
				if (flag)
				{
					material.EnableKeyword("_SPECGLOSSMAP");
					material.SetTexture("_SpecGlossMap", texture2D3);
				}
				else
				{
					material.EnableKeyword("_METALLICGLOSSMAP");
					material.SetTexture("_MetallicGlossMap", texture2D3);
				}
			}
			if (md.hasReflectionTex)
			{
				if (md.overallAlpha < 1f)
				{
					Color white = Color.white;
					white.a = md.overallAlpha;
					material.SetColor("_Color", white);
					mode = ModelUtil.MtlBlendMode.FADE;
				}
				if (md.specularTex != null)
				{
					material.SetFloat("_Metallic", num2);
				}
				material.SetFloat("_Glossiness", 1f);
			}
			ModelUtil.SetupMaterialWithBlendMode(material, mode);
			return material;
		}

		private bool Using32bitIndices()
		{
			return this.buildOptions == null || this.buildOptions.use32bitIndices;
		}

		public ImportOptions buildOptions;

		private ObjectBuilder.BuildStatus buildStatus = new ObjectBuilder.BuildStatus();

		private DataSet currDataSet;

		private GameObject currParentObj;

		private Dictionary<string, Material> currMaterials;

		private List<MaterialData> materialData;

		private static int MAX_VERTICES_LIMIT_FOR_A_MESH = 65000;

		private static int MAX_INDICES_LIMIT_FOR_A_MESH = 65000;

		private static int MAX_VERT_COUNT = (ObjectBuilder.MAX_VERTICES_LIMIT_FOR_A_MESH - 2) / 3 * 3;

		public class ProgressInfo
		{
			public int materialsLoaded;

			public int objectsLoaded;

			public int groupsLoaded;

			public int numGroups;
		}

		private class BuildStatus
		{
			public bool newObject = true;

			public int objCount;

			public int subObjCount;

			public int idxCount;

			public int grpIdx;

			public int numGroups;

			public int grpFaceIdx;

			public int meshPartIdx;

			public int totFaceIdxCount;

			public GameObject currObjGameObject;

			internal GameObject subObjParent;
		}
	}
}
