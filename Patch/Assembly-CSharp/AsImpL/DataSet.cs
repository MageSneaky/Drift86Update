using System;
using System.Collections.Generic;
using UnityEngine;

namespace AsImpL
{
	public class DataSet
	{
		public string CurrGroupName
		{
			get
			{
				if (this.currGroup == null)
				{
					return "";
				}
				return this.currGroup.name;
			}
		}

		public bool IsEmpty
		{
			get
			{
				return this.vertList.Count == 0;
			}
		}

		public static string GetFaceIndicesKey(DataSet.FaceIndices fi)
		{
			return string.Concat(new string[]
			{
				fi.vertIdx.ToString(),
				"/",
				fi.uvIdx.ToString(),
				"/",
				fi.normIdx.ToString()
			});
		}

		public static string FixMaterialName(string mtlName)
		{
			return mtlName.Replace(':', '_').Replace('\\', '_').Replace('/', '_').Replace('*', '_').Replace('?', '_').Replace('<', '_').Replace('>', '_').Replace('|', '_');
		}

		public DataSet()
		{
			DataSet.ObjectData objectData = new DataSet.ObjectData();
			objectData.name = "default";
			this.objectList.Add(objectData);
			this.currObjData = objectData;
			DataSet.FaceGroupData faceGroupData = new DataSet.FaceGroupData();
			faceGroupData.name = "default";
			objectData.faceGroups.Add(faceGroupData);
			this.currGroup = faceGroupData;
		}

		public void AddObject(string objectName)
		{
			string materialName = this.currObjData.faceGroups[this.currObjData.faceGroups.Count - 1].materialName;
			if (this.noFaceDefined)
			{
				this.objectList.Remove(this.currObjData);
			}
			DataSet.ObjectData objectData = new DataSet.ObjectData();
			objectData.name = objectName;
			this.objectList.Add(objectData);
			DataSet.FaceGroupData faceGroupData = new DataSet.FaceGroupData();
			faceGroupData.materialName = materialName;
			faceGroupData.name = "default";
			objectData.faceGroups.Add(faceGroupData);
			this.currGroup = faceGroupData;
			this.currObjData = objectData;
		}

		public void AddGroup(string groupName)
		{
			string materialName = this.currObjData.faceGroups[this.currObjData.faceGroups.Count - 1].materialName;
			if (this.currGroup.IsEmpty)
			{
				this.currObjData.faceGroups.Remove(this.currGroup);
			}
			DataSet.FaceGroupData faceGroupData = new DataSet.FaceGroupData();
			faceGroupData.materialName = materialName;
			if (groupName == null)
			{
				groupName = "Unnamed-" + this.unnamedGroupIndex;
				this.unnamedGroupIndex++;
			}
			faceGroupData.name = groupName;
			this.currObjData.faceGroups.Add(faceGroupData);
			this.currGroup = faceGroupData;
		}

		public void AddMaterialName(string matName)
		{
			if (!this.currGroup.IsEmpty)
			{
				this.AddGroup(matName);
			}
			if (this.currGroup.name == "default")
			{
				this.currGroup.name = matName;
			}
			this.currGroup.materialName = matName;
		}

		public void AddVertex(Vector3 vertex)
		{
			this.vertList.Add(vertex);
		}

		public void AddUV(Vector2 uv)
		{
			this.uvList.Add(uv);
		}

		public void AddNormal(Vector3 normal)
		{
			this.normalList.Add(normal);
		}

		public void AddColor(Color color)
		{
			this.colorList.Add(color);
			this.currObjData.hasColors = true;
		}

		public void AddFaceIndices(DataSet.FaceIndices faceIdx)
		{
			this.noFaceDefined = false;
			this.currGroup.faces.Add(faceIdx);
			this.currObjData.allFaces.Add(faceIdx);
			if (faceIdx.normIdx >= 0)
			{
				this.currObjData.hasNormals = true;
			}
		}

		public void PrintSummary()
		{
			string text = string.Concat(new object[]
			{
				"This data set has ",
				this.objectList.Count,
				" object(s)\n  ",
				this.vertList.Count,
				" vertices\n  ",
				this.uvList.Count,
				" uvs\n  ",
				this.normalList.Count,
				" normals"
			});
			foreach (DataSet.ObjectData objectData in this.objectList)
			{
				text = string.Concat(new object[]
				{
					text,
					"\n  ",
					objectData.name,
					" has ",
					objectData.faceGroups.Count,
					" group(s)"
				});
				foreach (DataSet.FaceGroupData faceGroupData in objectData.faceGroups)
				{
					text = string.Concat(new object[]
					{
						text,
						"\n    ",
						faceGroupData.name,
						" has ",
						faceGroupData.faces.Count,
						" faces(s)"
					});
				}
			}
			Debug.Log(text);
		}

		public List<DataSet.ObjectData> objectList = new List<DataSet.ObjectData>();

		public List<Vector3> vertList = new List<Vector3>();

		public List<Vector2> uvList = new List<Vector2>();

		public List<Vector3> normalList = new List<Vector3>();

		public List<Color> colorList = new List<Color>();

		private int unnamedGroupIndex = 1;

		private DataSet.ObjectData currObjData;

		private DataSet.FaceGroupData currGroup;

		private bool noFaceDefined = true;

		public struct FaceIndices
		{
			public int vertIdx;

			public int uvIdx;

			public int normIdx;
		}

		public class ObjectData
		{
			public string name;

			public List<DataSet.FaceGroupData> faceGroups = new List<DataSet.FaceGroupData>();

			public List<DataSet.FaceIndices> allFaces = new List<DataSet.FaceIndices>();

			public bool hasNormals;

			public bool hasColors;
		}

		public class FaceGroupData
		{
			public FaceGroupData()
			{
				this.faces = new List<DataSet.FaceIndices>();
			}

			public bool IsEmpty
			{
				get
				{
					return this.faces.Count == 0;
				}
			}

			public string name;

			public string materialName;

			public List<DataSet.FaceIndices> faces;
		}
	}
}
