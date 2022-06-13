using System;
using UnityEngine;

namespace AsImpL
{
	[Serializable]
	public class ImportOptions
	{
		[Tooltip("load the OBJ file assuming its vertical axis is Z instead of Y")]
		public bool zUp = true;

		[Tooltip("Consider diffuse map as already lit (disable lighting) if no other texture is present")]
		public bool litDiffuse;

		[Tooltip("Consider to double-sided (duplicate and flip faces and normals")]
		public bool convertToDoubleSided;

		[Tooltip("Rescaling for the model (1 = no rescaling)")]
		public float modelScaling = 1f;

		[Tooltip("Reuse a model in memory if already loaded")]
		public bool reuseLoaded;

		[Tooltip("Inherit parent layer")]
		public bool inheritLayer;

		[Tooltip("Generate mesh colliders")]
		public bool buildColliders;

		[Tooltip("Generate convex mesh colliders (only active if buildColliders = true)\nNote: it could not work for meshes with too many smooth surface regions.")]
		public bool colliderConvex;

		[Tooltip("Mesh colliders as trigger (only active if colliderConvex = true)")]
		public bool colliderTrigger;

		[Tooltip("Use 32 bit indices when needed, if available")]
		public bool use32bitIndices = true;

		[Tooltip("Hide the loaded object during the loading process")]
		public bool hideWhileLoading;

		[Header("Local Transform for the imported game object")]
		[Tooltip("Position of the object")]
		public Vector3 localPosition = Vector3.zero;

		[Tooltip("Rotation of the object\n(Euler angles)")]
		public Vector3 localEulerAngles = Vector3.zero;

		[Tooltip("Scaling of the object\n([1,1,1] = no rescaling)")]
		public Vector3 localScale = Vector3.one;
	}
}
