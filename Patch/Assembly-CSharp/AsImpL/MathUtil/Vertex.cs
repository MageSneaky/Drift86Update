using System;
using UnityEngine;

namespace AsImpL.MathUtil
{
	public class Vertex
	{
		public Vector3 Position { get; private set; }

		public int OriginalIndex { get; private set; }

		public Vertex PreviousVertex
		{
			get
			{
				return this.prevVertex;
			}
			set
			{
				this.triangleHasChanged = (this.prevVertex != value);
				this.prevVertex = value;
			}
		}

		public Vertex NextVertex
		{
			get
			{
				return this.nextVertex;
			}
			set
			{
				this.triangleHasChanged = (this.nextVertex != value);
				this.nextVertex = value;
			}
		}

		public float TriangleArea
		{
			get
			{
				if (this.triangleHasChanged)
				{
					this.ComputeTriangleArea();
				}
				return this.triangleArea;
			}
		}

		public Vertex(int originalIndex, Vector3 position)
		{
			this.OriginalIndex = originalIndex;
			this.Position = position;
		}

		public Vector2 GetPosOnPlane(Vector3 planeNormal)
		{
			Quaternion quaternion = default(Quaternion);
			quaternion.SetFromToRotation(planeNormal, Vector3.back);
			Vector3 vector = quaternion * this.Position;
			return new Vector2(vector.x, vector.y);
		}

		private void ComputeTriangleArea()
		{
			Vector3 vector = this.PreviousVertex.Position - this.Position;
			Vector3 vector2 = this.NextVertex.Position - this.Position;
			this.triangleArea = Vector3.Cross(vector, vector2).magnitude / 2f;
		}

		private Vertex prevVertex;

		private Vertex nextVertex;

		private float triangleArea;

		private bool triangleHasChanged;
	}
}
