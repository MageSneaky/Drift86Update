using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AsImpL;
using UnityEngine;
using UnityMeshSimplifier;

namespace BrainFailProductions.PolyFewRuntime
{
	public class PolyfewRuntime : MonoBehaviour
	{
		public static int SimplifyObjectDeep(GameObject toSimplify, PolyfewRuntime.SimplificationOptions simplificationOptions, Action<GameObject, PolyfewRuntime.MeshRendererPair> OnEachMeshSimplified)
		{
			if (simplificationOptions == null)
			{
				throw new ArgumentNullException("simplificationOptions", "You must provide a SimplificationOptions object.");
			}
			int totalTriangles = 0;
			float simplificationStrength = simplificationOptions.simplificationStrength;
			if (toSimplify == null)
			{
				throw new ArgumentNullException("toSimplify", "You must provide a gameobject to simplify.");
			}
			if (!simplificationOptions.simplifyMeshLossless)
			{
				if (simplificationStrength < 0f || simplificationStrength > 100f)
				{
					throw new ArgumentOutOfRangeException("simplificationStrength", "The allowed values for simplification strength are between [0-100] inclusive.");
				}
				if (Mathf.Approximately(simplificationStrength, 0f))
				{
					return -1;
				}
			}
			if (simplificationOptions.regardPreservationSpheres && (simplificationOptions.preservationSpheres == null || simplificationOptions.preservationSpheres.Count == 0))
			{
				simplificationOptions.preservationSpheres = new List<PolyfewRuntime.PreservationSphere>();
				simplificationOptions.regardPreservationSpheres = false;
			}
			PolyfewRuntime.ObjectMeshPairs objectMeshPairs = PolyfewRuntime.GetObjectMeshPairs(toSimplify, true);
			if (!PolyfewRuntime.AreAnyFeasibleMeshes(objectMeshPairs))
			{
				throw new InvalidOperationException("No mesh/meshes found nested under the provided gameobject to simplify.");
			}
			bool flag = false;
			if (PolyfewRuntime.CountTriangles(objectMeshPairs) >= 2000 && objectMeshPairs.Count >= 2)
			{
				flag = true;
			}
			if (Application.platform == 17)
			{
				flag = false;
			}
			float quality = 1f - simplificationStrength / 100f;
			int count = objectMeshPairs.Count;
			int meshesHandled = 0;
			int threadsRunning = 0;
			bool isError = false;
			string error = "";
			object threadLock1 = new object();
			object threadLock2 = new object();
			object threadLock3 = new object();
			if (flag)
			{
				List<PolyfewRuntime.CustomMeshActionStructure> meshAssignments = new List<PolyfewRuntime.CustomMeshActionStructure>();
				List<PolyfewRuntime.CustomMeshActionStructure> callbackFlusher = new List<PolyfewRuntime.CustomMeshActionStructure>();
				using (Dictionary<GameObject, PolyfewRuntime.MeshRendererPair>.Enumerator enumerator = objectMeshPairs.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						KeyValuePair<GameObject, PolyfewRuntime.MeshRendererPair> keyValuePair = enumerator.Current;
						GameObject gameObject = keyValuePair.Key;
						if (gameObject == null)
						{
							int num = meshesHandled;
							meshesHandled = num + 1;
						}
						else
						{
							PolyfewRuntime.MeshRendererPair meshRendererPair = keyValuePair.Value;
							if (meshRendererPair.mesh == null)
							{
								int num = meshesHandled;
								meshesHandled = num + 1;
							}
							else
							{
								MeshSimplifier meshSimplifier = new MeshSimplifier();
								PolyfewRuntime.SetParametersForSimplifier(simplificationOptions, meshSimplifier);
								ToleranceSphere[] array = new ToleranceSphere[simplificationOptions.preservationSpheres.Count];
								if (!meshRendererPair.attachedToMeshFilter && simplificationOptions.regardPreservationSpheres)
								{
									meshSimplifier.isSkinned = true;
									SkinnedMeshRenderer component = gameObject.GetComponent<SkinnedMeshRenderer>();
									meshSimplifier.boneWeightsOriginal = meshRendererPair.mesh.boneWeights;
									meshSimplifier.bindPosesOriginal = meshRendererPair.mesh.bindposes;
									meshSimplifier.bonesOriginal = component.bones;
									int num2 = 0;
									foreach (PolyfewRuntime.PreservationSphere preservationSphere in simplificationOptions.preservationSpheres)
									{
										gameObject.transform.InverseTransformPoint(preservationSphere.worldPosition);
										ToleranceSphere toleranceSphere = new ToleranceSphere
										{
											diameter = preservationSphere.diameter,
											localToWorldMatrix = gameObject.transform.localToWorldMatrix,
											worldPosition = preservationSphere.worldPosition,
											targetObject = gameObject,
											preservationStrength = preservationSphere.preservationStrength
										};
										array[num2] = toleranceSphere;
										num2++;
									}
									meshSimplifier.toleranceSpheres = array;
								}
								else if (meshRendererPair.attachedToMeshFilter && simplificationOptions.regardPreservationSpheres)
								{
									int num3 = 0;
									foreach (PolyfewRuntime.PreservationSphere preservationSphere2 in simplificationOptions.preservationSpheres)
									{
										ToleranceSphere toleranceSphere2 = new ToleranceSphere
										{
											diameter = preservationSphere2.diameter,
											localToWorldMatrix = gameObject.transform.localToWorldMatrix,
											worldPosition = preservationSphere2.worldPosition,
											targetObject = gameObject,
											preservationStrength = preservationSphere2.preservationStrength
										};
										array[num3] = toleranceSphere2;
										num3++;
									}
									meshSimplifier.toleranceSpheres = array;
								}
								meshSimplifier.Initialize(meshRendererPair.mesh, simplificationOptions.regardPreservationSpheres);
								int num = threadsRunning;
								threadsRunning = num + 1;
								while (callbackFlusher.Count > 0)
								{
									PolyfewRuntime.CustomMeshActionStructure customMeshActionStructure = callbackFlusher[0];
									callbackFlusher.RemoveAt(0);
									if (customMeshActionStructure != null && OnEachMeshSimplified != null)
									{
										OnEachMeshSimplified(customMeshActionStructure.gameObject, customMeshActionStructure.meshRendererPair);
									}
								}
								Action <>9__1;
								Task.Factory.StartNew(delegate()
								{
									PolyfewRuntime.MeshRendererPair meshRendererPair = meshRendererPair;
									GameObject gameObject = gameObject;
									Action action;
									if ((action = <>9__1) == null)
									{
										action = (<>9__1 = delegate()
										{
											Mesh mesh2 = meshSimplifier.ToMesh();
											PolyfewRuntime.AssignReducedMesh(gameObject, meshRendererPair.mesh, mesh2, meshRendererPair.attachedToMeshFilter, true);
											if (meshSimplifier.RecalculateNormals)
											{
												mesh2.RecalculateNormals();
												mesh2.RecalculateTangents();
											}
											totalTriangles += mesh2.triangles.Length / 3;
										});
									}
									PolyfewRuntime.CustomMeshActionStructure item = new PolyfewRuntime.CustomMeshActionStructure(meshRendererPair, gameObject, action);
									try
									{
										if (!simplificationOptions.simplifyMeshLossless)
										{
											meshSimplifier.SimplifyMesh(quality);
										}
										else
										{
											meshSimplifier.SimplifyMeshLossless();
										}
										object obj = threadLock1;
										lock (obj)
										{
											meshAssignments.Add(item);
											int num6 = threadsRunning;
											threadsRunning = num6 - 1;
											num6 = meshesHandled;
											meshesHandled = num6 + 1;
										}
										obj = threadLock3;
										lock (obj)
										{
											PolyfewRuntime.CustomMeshActionStructure item2 = new PolyfewRuntime.CustomMeshActionStructure(meshRendererPair, gameObject, delegate()
											{
											});
											callbackFlusher.Add(item2);
										}
									}
									catch (Exception ex)
									{
										object obj = threadLock2;
										lock (obj)
										{
											int num6 = threadsRunning;
											threadsRunning = num6 - 1;
											num6 = meshesHandled;
											meshesHandled = num6 + 1;
											isError = true;
											error = ex.ToString();
										}
									}
								}, CancellationToken.None, TaskCreationOptions.RunContinuationsAsynchronously, TaskScheduler.Current);
							}
						}
					}
					goto IL_612;
				}
				IL_5DB:
				PolyfewRuntime.CustomMeshActionStructure customMeshActionStructure2 = callbackFlusher[0];
				callbackFlusher.RemoveAt(0);
				if (customMeshActionStructure2 != null && OnEachMeshSimplified != null)
				{
					OnEachMeshSimplified(customMeshActionStructure2.gameObject, customMeshActionStructure2.meshRendererPair);
				}
				IL_612:
				if (callbackFlusher.Count > 0)
				{
					goto IL_5DB;
				}
				while (meshesHandled < count && !isError)
				{
					while (callbackFlusher.Count > 0)
					{
						PolyfewRuntime.CustomMeshActionStructure customMeshActionStructure3 = callbackFlusher[0];
						callbackFlusher.RemoveAt(0);
						if (customMeshActionStructure3 != null && OnEachMeshSimplified != null)
						{
							OnEachMeshSimplified(customMeshActionStructure3.gameObject, customMeshActionStructure3.meshRendererPair);
						}
					}
				}
				if (isError)
				{
					goto IL_A1C;
				}
				using (List<PolyfewRuntime.CustomMeshActionStructure>.Enumerator enumerator3 = meshAssignments.GetEnumerator())
				{
					while (enumerator3.MoveNext())
					{
						PolyfewRuntime.CustomMeshActionStructure customMeshActionStructure4 = enumerator3.Current;
						if (customMeshActionStructure4 != null)
						{
							customMeshActionStructure4.action();
						}
					}
					goto IL_A1C;
				}
			}
			foreach (KeyValuePair<GameObject, PolyfewRuntime.MeshRendererPair> keyValuePair2 in objectMeshPairs)
			{
				GameObject key = keyValuePair2.Key;
				if (!(key == null))
				{
					PolyfewRuntime.MeshRendererPair value = keyValuePair2.Value;
					if (!(value.mesh == null))
					{
						MeshSimplifier meshSimplifier2 = new MeshSimplifier();
						PolyfewRuntime.SetParametersForSimplifier(simplificationOptions, meshSimplifier2);
						ToleranceSphere[] array2 = new ToleranceSphere[simplificationOptions.preservationSpheres.Count];
						if (!value.attachedToMeshFilter && simplificationOptions.regardPreservationSpheres)
						{
							meshSimplifier2.isSkinned = true;
							SkinnedMeshRenderer component2 = key.GetComponent<SkinnedMeshRenderer>();
							meshSimplifier2.boneWeightsOriginal = value.mesh.boneWeights;
							meshSimplifier2.bindPosesOriginal = value.mesh.bindposes;
							meshSimplifier2.bonesOriginal = component2.bones;
							int num4 = 0;
							foreach (PolyfewRuntime.PreservationSphere preservationSphere3 in simplificationOptions.preservationSpheres)
							{
								ToleranceSphere toleranceSphere3 = new ToleranceSphere
								{
									diameter = preservationSphere3.diameter,
									localToWorldMatrix = key.transform.localToWorldMatrix,
									worldPosition = preservationSphere3.worldPosition,
									targetObject = key,
									preservationStrength = preservationSphere3.preservationStrength
								};
								array2[num4] = toleranceSphere3;
								num4++;
							}
							meshSimplifier2.toleranceSpheres = array2;
						}
						else if (value.attachedToMeshFilter && simplificationOptions.regardPreservationSpheres)
						{
							int num5 = 0;
							foreach (PolyfewRuntime.PreservationSphere preservationSphere4 in simplificationOptions.preservationSpheres)
							{
								ToleranceSphere toleranceSphere4 = new ToleranceSphere
								{
									diameter = preservationSphere4.diameter,
									localToWorldMatrix = key.transform.localToWorldMatrix,
									worldPosition = preservationSphere4.worldPosition,
									targetObject = key,
									preservationStrength = preservationSphere4.preservationStrength
								};
								array2[num5] = toleranceSphere4;
								num5++;
							}
							meshSimplifier2.toleranceSpheres = array2;
						}
						meshSimplifier2.Initialize(value.mesh, simplificationOptions.regardPreservationSpheres);
						if (!simplificationOptions.simplifyMeshLossless)
						{
							meshSimplifier2.SimplifyMesh(quality);
						}
						else
						{
							meshSimplifier2.SimplifyMeshLossless();
						}
						if (OnEachMeshSimplified != null)
						{
							OnEachMeshSimplified(key, value);
						}
						Mesh mesh = meshSimplifier2.ToMesh();
						mesh.bindposes = value.mesh.bindposes;
						mesh.name = value.mesh.name.Replace("-POLY_REDUCED", "") + "-POLY_REDUCED";
						if (meshSimplifier2.RecalculateNormals)
						{
							mesh.RecalculateNormals();
							mesh.RecalculateTangents();
						}
						if (value.attachedToMeshFilter)
						{
							MeshFilter component3 = key.GetComponent<MeshFilter>();
							if (component3 != null)
							{
								component3.sharedMesh = mesh;
							}
						}
						else
						{
							SkinnedMeshRenderer component4 = key.GetComponent<SkinnedMeshRenderer>();
							if (component4 != null)
							{
								component4.sharedMesh = mesh;
							}
						}
						totalTriangles += mesh.triangles.Length / 3;
					}
				}
			}
			IL_A1C:
			return totalTriangles;
		}

		public static PolyfewRuntime.ObjectMeshPairs SimplifyObjectDeep(GameObject toSimplify, PolyfewRuntime.SimplificationOptions simplificationOptions)
		{
			if (simplificationOptions == null)
			{
				throw new ArgumentNullException("simplificationOptions", "You must provide a SimplificationOptions object.");
			}
			float simplificationStrength = simplificationOptions.simplificationStrength;
			PolyfewRuntime.ObjectMeshPairs toReturn = new PolyfewRuntime.ObjectMeshPairs();
			if (toSimplify == null)
			{
				throw new ArgumentNullException("toSimplify", "You must provide a gameobject to simplify.");
			}
			if (!simplificationOptions.simplifyMeshLossless)
			{
				if (simplificationStrength < 0f || simplificationStrength > 100f)
				{
					throw new ArgumentOutOfRangeException("simplificationStrength", "The allowed values for simplification strength are between [0-100] inclusive.");
				}
				if (Mathf.Approximately(simplificationStrength, 0f))
				{
					return null;
				}
			}
			if (simplificationOptions.regardPreservationSpheres && (simplificationOptions.preservationSpheres == null || simplificationOptions.preservationSpheres.Count == 0))
			{
				simplificationOptions.preservationSpheres = new List<PolyfewRuntime.PreservationSphere>();
				simplificationOptions.regardPreservationSpheres = false;
			}
			PolyfewRuntime.ObjectMeshPairs objectMeshPairs = PolyfewRuntime.GetObjectMeshPairs(toSimplify, true);
			if (!PolyfewRuntime.AreAnyFeasibleMeshes(objectMeshPairs))
			{
				throw new InvalidOperationException("No mesh/meshes found nested under the provided gameobject to simplify.");
			}
			bool flag = false;
			if (PolyfewRuntime.CountTriangles(objectMeshPairs) >= 2000 && objectMeshPairs.Count >= 2)
			{
				flag = true;
			}
			if (Application.platform == 17)
			{
				flag = false;
			}
			float quality = 1f - simplificationStrength / 100f;
			int count = objectMeshPairs.Count;
			int meshesHandled = 0;
			int threadsRunning = 0;
			bool isError = false;
			string error = "";
			object threadLock1 = new object();
			object threadLock2 = new object();
			if (flag)
			{
				List<PolyfewRuntime.CustomMeshActionStructure> meshAssignments = new List<PolyfewRuntime.CustomMeshActionStructure>();
				foreach (KeyValuePair<GameObject, PolyfewRuntime.MeshRendererPair> keyValuePair in objectMeshPairs)
				{
					GameObject gameObject = keyValuePair.Key;
					if (gameObject == null)
					{
						int num = meshesHandled;
						meshesHandled = num + 1;
					}
					else
					{
						PolyfewRuntime.MeshRendererPair meshRendererPair = keyValuePair.Value;
						if (meshRendererPair.mesh == null)
						{
							int num = meshesHandled;
							meshesHandled = num + 1;
						}
						else
						{
							MeshSimplifier meshSimplifier = new MeshSimplifier();
							PolyfewRuntime.SetParametersForSimplifier(simplificationOptions, meshSimplifier);
							ToleranceSphere[] array = new ToleranceSphere[simplificationOptions.preservationSpheres.Count];
							if (!meshRendererPair.attachedToMeshFilter && simplificationOptions.regardPreservationSpheres)
							{
								meshSimplifier.isSkinned = true;
								SkinnedMeshRenderer component = gameObject.GetComponent<SkinnedMeshRenderer>();
								meshSimplifier.boneWeightsOriginal = meshRendererPair.mesh.boneWeights;
								meshSimplifier.bindPosesOriginal = meshRendererPair.mesh.bindposes;
								meshSimplifier.bonesOriginal = component.bones;
								int num2 = 0;
								foreach (PolyfewRuntime.PreservationSphere preservationSphere in simplificationOptions.preservationSpheres)
								{
									ToleranceSphere toleranceSphere = new ToleranceSphere
									{
										diameter = preservationSphere.diameter,
										localToWorldMatrix = gameObject.transform.localToWorldMatrix,
										worldPosition = preservationSphere.worldPosition,
										targetObject = gameObject,
										preservationStrength = preservationSphere.preservationStrength
									};
									array[num2] = toleranceSphere;
									num2++;
								}
								meshSimplifier.toleranceSpheres = array;
							}
							else if (meshRendererPair.attachedToMeshFilter && simplificationOptions.regardPreservationSpheres)
							{
								int num3 = 0;
								foreach (PolyfewRuntime.PreservationSphere preservationSphere2 in simplificationOptions.preservationSpheres)
								{
									ToleranceSphere toleranceSphere2 = new ToleranceSphere
									{
										diameter = preservationSphere2.diameter,
										localToWorldMatrix = gameObject.transform.localToWorldMatrix,
										worldPosition = preservationSphere2.worldPosition,
										targetObject = gameObject,
										preservationStrength = preservationSphere2.preservationStrength
									};
									array[num3] = toleranceSphere2;
									num3++;
								}
								meshSimplifier.toleranceSpheres = array;
							}
							meshSimplifier.Initialize(meshRendererPair.mesh, simplificationOptions.regardPreservationSpheres);
							int num = threadsRunning;
							threadsRunning = num + 1;
							Action <>9__1;
							Task.Factory.StartNew(delegate()
							{
								PolyfewRuntime.MeshRendererPair meshRendererPair = meshRendererPair;
								GameObject gameObject = gameObject;
								Action action;
								if ((action = <>9__1) == null)
								{
									action = (<>9__1 = delegate()
									{
										Mesh mesh2 = meshSimplifier.ToMesh();
										mesh2.bindposes = meshRendererPair.mesh.bindposes;
										mesh2.name = meshRendererPair.mesh.name.Replace("-POLY_REDUCED", "") + "-POLY_REDUCED";
										if (meshSimplifier.RecalculateNormals)
										{
											mesh2.RecalculateNormals();
											mesh2.RecalculateTangents();
										}
										PolyfewRuntime.MeshRendererPair value4 = new PolyfewRuntime.MeshRendererPair(meshRendererPair.attachedToMeshFilter, mesh2);
										toReturn.Add(gameObject, value4);
									});
								}
								PolyfewRuntime.CustomMeshActionStructure item = new PolyfewRuntime.CustomMeshActionStructure(meshRendererPair, gameObject, action);
								try
								{
									if (!simplificationOptions.simplifyMeshLossless)
									{
										meshSimplifier.SimplifyMesh(quality);
									}
									else
									{
										meshSimplifier.SimplifyMeshLossless();
									}
									object obj = threadLock1;
									lock (obj)
									{
										meshAssignments.Add(item);
										int num6 = threadsRunning;
										threadsRunning = num6 - 1;
										num6 = meshesHandled;
										meshesHandled = num6 + 1;
									}
								}
								catch (Exception ex)
								{
									object obj = threadLock2;
									lock (obj)
									{
										int num6 = threadsRunning;
										threadsRunning = num6 - 1;
										num6 = meshesHandled;
										meshesHandled = num6 + 1;
										isError = true;
										error = ex.ToString();
									}
								}
							}, CancellationToken.None, TaskCreationOptions.RunContinuationsAsynchronously, TaskScheduler.Current);
						}
					}
				}
				while (meshesHandled < count && !isError)
				{
				}
				if (isError)
				{
					goto IL_912;
				}
				using (List<PolyfewRuntime.CustomMeshActionStructure>.Enumerator enumerator3 = meshAssignments.GetEnumerator())
				{
					while (enumerator3.MoveNext())
					{
						PolyfewRuntime.CustomMeshActionStructure customMeshActionStructure = enumerator3.Current;
						if (customMeshActionStructure != null)
						{
							customMeshActionStructure.action();
						}
					}
					goto IL_912;
				}
			}
			foreach (KeyValuePair<GameObject, PolyfewRuntime.MeshRendererPair> keyValuePair2 in objectMeshPairs)
			{
				GameObject key = keyValuePair2.Key;
				if (!(key == null))
				{
					PolyfewRuntime.MeshRendererPair value = keyValuePair2.Value;
					if (!(value.mesh == null))
					{
						MeshSimplifier meshSimplifier2 = new MeshSimplifier();
						PolyfewRuntime.SetParametersForSimplifier(simplificationOptions, meshSimplifier2);
						ToleranceSphere[] array2 = new ToleranceSphere[simplificationOptions.preservationSpheres.Count];
						if (!value.attachedToMeshFilter && simplificationOptions.regardPreservationSpheres)
						{
							meshSimplifier2.isSkinned = true;
							SkinnedMeshRenderer component2 = key.GetComponent<SkinnedMeshRenderer>();
							meshSimplifier2.boneWeightsOriginal = value.mesh.boneWeights;
							meshSimplifier2.bindPosesOriginal = value.mesh.bindposes;
							meshSimplifier2.bonesOriginal = component2.bones;
							int num4 = 0;
							foreach (PolyfewRuntime.PreservationSphere preservationSphere3 in simplificationOptions.preservationSpheres)
							{
								ToleranceSphere toleranceSphere3 = new ToleranceSphere
								{
									diameter = preservationSphere3.diameter,
									localToWorldMatrix = key.transform.localToWorldMatrix,
									worldPosition = preservationSphere3.worldPosition,
									targetObject = key,
									preservationStrength = preservationSphere3.preservationStrength
								};
								array2[num4] = toleranceSphere3;
								num4++;
							}
							meshSimplifier2.toleranceSpheres = array2;
						}
						else if (value.attachedToMeshFilter && simplificationOptions.regardPreservationSpheres)
						{
							int num5 = 0;
							foreach (PolyfewRuntime.PreservationSphere preservationSphere4 in simplificationOptions.preservationSpheres)
							{
								ToleranceSphere toleranceSphere4 = new ToleranceSphere
								{
									diameter = preservationSphere4.diameter,
									localToWorldMatrix = key.transform.localToWorldMatrix,
									worldPosition = preservationSphere4.worldPosition,
									targetObject = key,
									preservationStrength = preservationSphere4.preservationStrength
								};
								array2[num5] = toleranceSphere4;
								num5++;
							}
							meshSimplifier2.toleranceSpheres = array2;
						}
						meshSimplifier2.Initialize(value.mesh, simplificationOptions.regardPreservationSpheres);
						if (!simplificationOptions.simplifyMeshLossless)
						{
							meshSimplifier2.SimplifyMesh(quality);
						}
						else
						{
							meshSimplifier2.SimplifyMeshLossless();
						}
						Mesh mesh = meshSimplifier2.ToMesh();
						mesh.bindposes = value.mesh.bindposes;
						mesh.name = value.mesh.name.Replace("-POLY_REDUCED", "") + "-POLY_REDUCED";
						if (meshSimplifier2.RecalculateNormals)
						{
							mesh.RecalculateNormals();
							mesh.RecalculateTangents();
						}
						if (value.attachedToMeshFilter)
						{
							MeshFilter component3 = key.GetComponent<MeshFilter>();
							PolyfewRuntime.MeshRendererPair value2 = new PolyfewRuntime.MeshRendererPair(true, mesh);
							toReturn.Add(key, value2);
							if (component3 != null)
							{
								component3.sharedMesh = mesh;
							}
						}
						else
						{
							SkinnedMeshRenderer component4 = key.GetComponent<SkinnedMeshRenderer>();
							PolyfewRuntime.MeshRendererPair value3 = new PolyfewRuntime.MeshRendererPair(false, mesh);
							toReturn.Add(key, value3);
							if (component4 != null)
							{
								component4.sharedMesh = mesh;
							}
						}
					}
				}
			}
			IL_912:
			return toReturn;
		}

		public static int SimplifyObjectDeep(PolyfewRuntime.ObjectMeshPairs objectMeshPairs, PolyfewRuntime.SimplificationOptions simplificationOptions, Action<GameObject, PolyfewRuntime.MeshRendererPair> OnEachMeshSimplified)
		{
			if (simplificationOptions == null)
			{
				throw new ArgumentNullException("simplificationOptions", "You must provide a SimplificationOptions object.");
			}
			int totalTriangles = 0;
			float simplificationStrength = simplificationOptions.simplificationStrength;
			if (objectMeshPairs == null)
			{
				throw new ArgumentNullException("objectMeshPairs", "You must provide the objectMeshPairs structure to simplify.");
			}
			if (!simplificationOptions.simplifyMeshLossless)
			{
				if (simplificationStrength < 0f || simplificationStrength > 100f)
				{
					throw new ArgumentOutOfRangeException("simplificationStrength", "The allowed values for simplification strength are between [0-100] inclusive.");
				}
				if (Mathf.Approximately(simplificationStrength, 0f))
				{
					return -1;
				}
			}
			if (!PolyfewRuntime.AreAnyFeasibleMeshes(objectMeshPairs))
			{
				throw new InvalidOperationException("No mesh/meshes found nested under the provided gameobject to simplify.");
			}
			if (simplificationOptions.regardPreservationSpheres && (simplificationOptions.preservationSpheres == null || simplificationOptions.preservationSpheres.Count == 0))
			{
				simplificationOptions.preservationSpheres = new List<PolyfewRuntime.PreservationSphere>();
				simplificationOptions.regardPreservationSpheres = false;
			}
			bool flag = false;
			if (PolyfewRuntime.CountTriangles(objectMeshPairs) >= 2000 && objectMeshPairs.Count >= 2)
			{
				flag = true;
			}
			if (Application.platform == 17)
			{
				flag = false;
			}
			float quality = 1f - simplificationStrength / 100f;
			int count = objectMeshPairs.Count;
			int meshesHandled = 0;
			int threadsRunning = 0;
			bool isError = false;
			string error = "";
			object threadLock1 = new object();
			object threadLock2 = new object();
			object threadLock3 = new object();
			if (flag)
			{
				List<PolyfewRuntime.CustomMeshActionStructure> meshAssignments = new List<PolyfewRuntime.CustomMeshActionStructure>();
				List<PolyfewRuntime.CustomMeshActionStructure> callbackFlusher = new List<PolyfewRuntime.CustomMeshActionStructure>();
				using (Dictionary<GameObject, PolyfewRuntime.MeshRendererPair>.Enumerator enumerator = objectMeshPairs.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						KeyValuePair<GameObject, PolyfewRuntime.MeshRendererPair> keyValuePair = enumerator.Current;
						GameObject gameObject = keyValuePair.Key;
						if (gameObject == null)
						{
							int num = meshesHandled;
							meshesHandled = num + 1;
						}
						else
						{
							PolyfewRuntime.MeshRendererPair meshRendererPair = keyValuePair.Value;
							if (meshRendererPair.mesh == null)
							{
								int num = meshesHandled;
								meshesHandled = num + 1;
							}
							else
							{
								MeshSimplifier meshSimplifier = new MeshSimplifier();
								PolyfewRuntime.SetParametersForSimplifier(simplificationOptions, meshSimplifier);
								ToleranceSphere[] array = new ToleranceSphere[simplificationOptions.preservationSpheres.Count];
								if (!meshRendererPair.attachedToMeshFilter && simplificationOptions.regardPreservationSpheres)
								{
									meshSimplifier.isSkinned = true;
									SkinnedMeshRenderer component = gameObject.GetComponent<SkinnedMeshRenderer>();
									meshSimplifier.boneWeightsOriginal = meshRendererPair.mesh.boneWeights;
									meshSimplifier.bindPosesOriginal = meshRendererPair.mesh.bindposes;
									meshSimplifier.bonesOriginal = component.bones;
									int num2 = 0;
									foreach (PolyfewRuntime.PreservationSphere preservationSphere in simplificationOptions.preservationSpheres)
									{
										ToleranceSphere toleranceSphere = new ToleranceSphere
										{
											diameter = preservationSphere.diameter,
											localToWorldMatrix = gameObject.transform.localToWorldMatrix,
											worldPosition = preservationSphere.worldPosition,
											targetObject = gameObject,
											preservationStrength = preservationSphere.preservationStrength
										};
										array[num2] = toleranceSphere;
										num2++;
									}
									meshSimplifier.toleranceSpheres = array;
								}
								else if (meshRendererPair.attachedToMeshFilter && simplificationOptions.regardPreservationSpheres)
								{
									int num3 = 0;
									foreach (PolyfewRuntime.PreservationSphere preservationSphere2 in simplificationOptions.preservationSpheres)
									{
										ToleranceSphere toleranceSphere2 = new ToleranceSphere
										{
											diameter = preservationSphere2.diameter,
											localToWorldMatrix = gameObject.transform.localToWorldMatrix,
											worldPosition = preservationSphere2.worldPosition,
											targetObject = gameObject,
											preservationStrength = preservationSphere2.preservationStrength
										};
										array[num3] = toleranceSphere2;
										num3++;
									}
									meshSimplifier.toleranceSpheres = array;
								}
								meshSimplifier.Initialize(meshRendererPair.mesh, simplificationOptions.regardPreservationSpheres);
								int num = threadsRunning;
								threadsRunning = num + 1;
								while (callbackFlusher.Count > 0)
								{
									PolyfewRuntime.CustomMeshActionStructure customMeshActionStructure = callbackFlusher[0];
									callbackFlusher.RemoveAt(0);
									if (customMeshActionStructure != null && OnEachMeshSimplified != null)
									{
										OnEachMeshSimplified(customMeshActionStructure.gameObject, customMeshActionStructure.meshRendererPair);
									}
								}
								Action <>9__1;
								Task.Factory.StartNew(delegate()
								{
									PolyfewRuntime.MeshRendererPair meshRendererPair = meshRendererPair;
									GameObject gameObject = gameObject;
									Action action;
									if ((action = <>9__1) == null)
									{
										action = (<>9__1 = delegate()
										{
											Mesh mesh2 = meshSimplifier.ToMesh();
											PolyfewRuntime.AssignReducedMesh(gameObject, meshRendererPair.mesh, mesh2, meshRendererPair.attachedToMeshFilter, true);
											if (meshSimplifier.RecalculateNormals)
											{
												mesh2.RecalculateNormals();
												mesh2.RecalculateTangents();
											}
											totalTriangles += mesh2.triangles.Length / 3;
										});
									}
									PolyfewRuntime.CustomMeshActionStructure item = new PolyfewRuntime.CustomMeshActionStructure(meshRendererPair, gameObject, action);
									try
									{
										if (!simplificationOptions.simplifyMeshLossless)
										{
											meshSimplifier.SimplifyMesh(quality);
										}
										else
										{
											meshSimplifier.SimplifyMeshLossless();
										}
										object obj = threadLock1;
										lock (obj)
										{
											meshAssignments.Add(item);
											int num6 = threadsRunning;
											threadsRunning = num6 - 1;
											num6 = meshesHandled;
											meshesHandled = num6 + 1;
										}
										obj = threadLock3;
										lock (obj)
										{
											PolyfewRuntime.CustomMeshActionStructure item2 = new PolyfewRuntime.CustomMeshActionStructure(meshRendererPair, gameObject, delegate()
											{
											});
											callbackFlusher.Add(item2);
										}
									}
									catch (Exception ex)
									{
										object obj = threadLock2;
										lock (obj)
										{
											int num6 = threadsRunning;
											threadsRunning = num6 - 1;
											num6 = meshesHandled;
											meshesHandled = num6 + 1;
											isError = true;
											error = ex.ToString();
										}
									}
								}, CancellationToken.None, TaskCreationOptions.RunContinuationsAsynchronously, TaskScheduler.Current);
							}
						}
					}
					goto IL_5E4;
				}
				IL_5AD:
				PolyfewRuntime.CustomMeshActionStructure customMeshActionStructure2 = callbackFlusher[0];
				callbackFlusher.RemoveAt(0);
				if (customMeshActionStructure2 != null && OnEachMeshSimplified != null)
				{
					OnEachMeshSimplified(customMeshActionStructure2.gameObject, customMeshActionStructure2.meshRendererPair);
				}
				IL_5E4:
				if (callbackFlusher.Count > 0)
				{
					goto IL_5AD;
				}
				while (meshesHandled < count && !isError)
				{
					while (callbackFlusher.Count > 0)
					{
						PolyfewRuntime.CustomMeshActionStructure customMeshActionStructure3 = callbackFlusher[0];
						callbackFlusher.RemoveAt(0);
						if (customMeshActionStructure3 != null && OnEachMeshSimplified != null)
						{
							OnEachMeshSimplified(customMeshActionStructure3.gameObject, customMeshActionStructure3.meshRendererPair);
						}
					}
				}
				if (isError)
				{
					goto IL_9ED;
				}
				using (List<PolyfewRuntime.CustomMeshActionStructure>.Enumerator enumerator3 = meshAssignments.GetEnumerator())
				{
					while (enumerator3.MoveNext())
					{
						PolyfewRuntime.CustomMeshActionStructure customMeshActionStructure4 = enumerator3.Current;
						if (customMeshActionStructure4 != null)
						{
							customMeshActionStructure4.action();
						}
					}
					goto IL_9ED;
				}
			}
			foreach (KeyValuePair<GameObject, PolyfewRuntime.MeshRendererPair> keyValuePair2 in objectMeshPairs)
			{
				GameObject key = keyValuePair2.Key;
				if (!(key == null))
				{
					PolyfewRuntime.MeshRendererPair value = keyValuePair2.Value;
					if (!(value.mesh == null))
					{
						MeshSimplifier meshSimplifier2 = new MeshSimplifier();
						PolyfewRuntime.SetParametersForSimplifier(simplificationOptions, meshSimplifier2);
						ToleranceSphere[] array2 = new ToleranceSphere[simplificationOptions.preservationSpheres.Count];
						if (!value.attachedToMeshFilter && simplificationOptions.regardPreservationSpheres)
						{
							meshSimplifier2.isSkinned = true;
							SkinnedMeshRenderer component2 = key.GetComponent<SkinnedMeshRenderer>();
							meshSimplifier2.boneWeightsOriginal = value.mesh.boneWeights;
							meshSimplifier2.bindPosesOriginal = value.mesh.bindposes;
							meshSimplifier2.bonesOriginal = component2.bones;
							int num4 = 0;
							foreach (PolyfewRuntime.PreservationSphere preservationSphere3 in simplificationOptions.preservationSpheres)
							{
								ToleranceSphere toleranceSphere3 = new ToleranceSphere
								{
									diameter = preservationSphere3.diameter,
									localToWorldMatrix = key.transform.localToWorldMatrix,
									worldPosition = preservationSphere3.worldPosition,
									targetObject = key,
									preservationStrength = preservationSphere3.preservationStrength
								};
								array2[num4] = toleranceSphere3;
								num4++;
							}
							meshSimplifier2.toleranceSpheres = array2;
						}
						else if (value.attachedToMeshFilter && simplificationOptions.regardPreservationSpheres)
						{
							int num5 = 0;
							foreach (PolyfewRuntime.PreservationSphere preservationSphere4 in simplificationOptions.preservationSpheres)
							{
								ToleranceSphere toleranceSphere4 = new ToleranceSphere
								{
									diameter = preservationSphere4.diameter,
									localToWorldMatrix = key.transform.localToWorldMatrix,
									worldPosition = preservationSphere4.worldPosition,
									targetObject = key,
									preservationStrength = preservationSphere4.preservationStrength
								};
								array2[num5] = toleranceSphere4;
								num5++;
							}
							meshSimplifier2.toleranceSpheres = array2;
						}
						meshSimplifier2.Initialize(value.mesh, simplificationOptions.regardPreservationSpheres);
						if (!simplificationOptions.simplifyMeshLossless)
						{
							meshSimplifier2.SimplifyMesh(quality);
						}
						else
						{
							meshSimplifier2.SimplifyMeshLossless();
						}
						if (OnEachMeshSimplified != null)
						{
							OnEachMeshSimplified(key, value);
						}
						Mesh mesh = meshSimplifier2.ToMesh();
						mesh.bindposes = value.mesh.bindposes;
						mesh.name = value.mesh.name.Replace("-POLY_REDUCED", "") + "-POLY_REDUCED";
						if (meshSimplifier2.RecalculateNormals)
						{
							mesh.RecalculateNormals();
							mesh.RecalculateTangents();
						}
						if (value.attachedToMeshFilter)
						{
							MeshFilter component3 = key.GetComponent<MeshFilter>();
							if (component3 != null)
							{
								component3.sharedMesh = mesh;
							}
						}
						else
						{
							SkinnedMeshRenderer component4 = key.GetComponent<SkinnedMeshRenderer>();
							if (component4 != null)
							{
								component4.sharedMesh = mesh;
							}
						}
						totalTriangles += mesh.triangles.Length / 3;
					}
				}
			}
			IL_9ED:
			return totalTriangles;
		}

		public static List<Mesh> SimplifyMeshes(List<Mesh> meshesToSimplify, PolyfewRuntime.SimplificationOptions simplificationOptions, Action<Mesh> OnEachMeshSimplified)
		{
			PolyfewRuntime.<>c__DisplayClass13_0 CS$<>8__locals1 = new PolyfewRuntime.<>c__DisplayClass13_0();
			CS$<>8__locals1.simplificationOptions = simplificationOptions;
			CS$<>8__locals1.simplifiedMeshes = new List<Mesh>();
			if (CS$<>8__locals1.simplificationOptions == null)
			{
				throw new ArgumentNullException("simplificationOptions", "You must provide a SimplificationOptions object.");
			}
			float simplificationStrength = CS$<>8__locals1.simplificationOptions.simplificationStrength;
			if (meshesToSimplify == null)
			{
				throw new ArgumentNullException("meshesToSimplify", "You must provide a meshes list to simplify.");
			}
			if (meshesToSimplify.Count == 0)
			{
				throw new InvalidOperationException("You must provide a non-empty list of meshes to simplify.");
			}
			if (!CS$<>8__locals1.simplificationOptions.simplifyMeshLossless)
			{
				if (simplificationStrength < 0f || simplificationStrength > 100f)
				{
					throw new ArgumentOutOfRangeException("simplificationStrength", "The allowed values for simplification strength are between [0-100] inclusive.");
				}
				if (Mathf.Approximately(simplificationStrength, 0f))
				{
					return null;
				}
			}
			if (PolyfewRuntime.CountTriangles(meshesToSimplify) >= 2000)
			{
				int count = meshesToSimplify.Count;
			}
			RuntimePlatform platform = Application.platform;
			CS$<>8__locals1.quality = 1f - simplificationStrength / 100f;
			int count2 = meshesToSimplify.Count;
			CS$<>8__locals1.meshesHandled = 0;
			CS$<>8__locals1.threadsRunning = 0;
			CS$<>8__locals1.isError = false;
			CS$<>8__locals1.error = "";
			CS$<>8__locals1.threadLock1 = new object();
			CS$<>8__locals1.threadLock2 = new object();
			CS$<>8__locals1.threadLock3 = new object();
			if (true)
			{
				PolyfewRuntime.<>c__DisplayClass13_1 CS$<>8__locals2 = new PolyfewRuntime.<>c__DisplayClass13_1();
				CS$<>8__locals2.CS$<>8__locals1 = CS$<>8__locals1;
				CS$<>8__locals2.meshAssignments = new List<PolyfewRuntime.CustomMeshActionStructure>();
				CS$<>8__locals2.callbackFlusher = new List<PolyfewRuntime.CustomMeshActionStructure>();
				using (List<Mesh>.Enumerator enumerator = meshesToSimplify.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						PolyfewRuntime.<>c__DisplayClass13_2 CS$<>8__locals3 = new PolyfewRuntime.<>c__DisplayClass13_2();
						CS$<>8__locals3.CS$<>8__locals2 = CS$<>8__locals2;
						CS$<>8__locals3.meshToSimplify = enumerator.Current;
						if (CS$<>8__locals3.meshToSimplify == null)
						{
							int num = CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.meshesHandled;
							CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.meshesHandled = num + 1;
						}
						else
						{
							MeshSimplifier meshSimplifier = new MeshSimplifier();
							PolyfewRuntime.SetParametersForSimplifier(CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.simplificationOptions, meshSimplifier);
							meshSimplifier.Initialize(CS$<>8__locals3.meshToSimplify, false);
							int num = CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.threadsRunning;
							CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.threadsRunning = num + 1;
							while (CS$<>8__locals3.CS$<>8__locals2.callbackFlusher.Count > 0)
							{
								PolyfewRuntime.CustomMeshActionStructure customMeshActionStructure = CS$<>8__locals3.CS$<>8__locals2.callbackFlusher[0];
								CS$<>8__locals3.CS$<>8__locals2.callbackFlusher.RemoveAt(0);
								if (OnEachMeshSimplified != null)
								{
									OnEachMeshSimplified(customMeshActionStructure.meshRendererPair.mesh);
								}
							}
							Action <>9__1;
							Task.Factory.StartNew(delegate()
							{
								PolyfewRuntime.MeshRendererPair meshRendererPair = null;
								GameObject gameObject = null;
								Action action;
								if ((action = <>9__1) == null)
								{
									action = (<>9__1 = delegate()
									{
										Mesh mesh3 = meshSimplifier.ToMesh();
										mesh3.bindposes = CS$<>8__locals3.meshToSimplify.bindposes;
										mesh3.name = CS$<>8__locals3.meshToSimplify.name.Replace("-POLY_REDUCED", "") + "-POLY_REDUCED";
										if (meshSimplifier.RecalculateNormals)
										{
											mesh3.RecalculateNormals();
											mesh3.RecalculateTangents();
										}
										CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.simplifiedMeshes.Add(mesh3);
									});
								}
								PolyfewRuntime.CustomMeshActionStructure item = new PolyfewRuntime.CustomMeshActionStructure(meshRendererPair, gameObject, action);
								try
								{
									if (!CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.simplificationOptions.simplifyMeshLossless)
									{
										meshSimplifier.SimplifyMesh(CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.quality);
									}
									else
									{
										meshSimplifier.SimplifyMeshLossless();
									}
									object obj = CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.threadLock1;
									lock (obj)
									{
										CS$<>8__locals3.CS$<>8__locals2.meshAssignments.Add(item);
										int num2 = CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.threadsRunning;
										CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.threadsRunning = num2 - 1;
										num2 = CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.meshesHandled;
										CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.meshesHandled = num2 + 1;
									}
									obj = CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.threadLock3;
									lock (obj)
									{
										PolyfewRuntime.CustomMeshActionStructure item2 = new PolyfewRuntime.CustomMeshActionStructure(new PolyfewRuntime.MeshRendererPair(true, CS$<>8__locals3.meshToSimplify), null, delegate()
										{
										});
										CS$<>8__locals3.CS$<>8__locals2.callbackFlusher.Add(item2);
									}
								}
								catch (Exception ex)
								{
									object obj = CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.threadLock2;
									lock (obj)
									{
										int num2 = CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.threadsRunning;
										CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.threadsRunning = num2 - 1;
										num2 = CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.meshesHandled;
										CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.meshesHandled = num2 + 1;
										CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.isError = true;
										CS$<>8__locals3.CS$<>8__locals2.CS$<>8__locals1.error = ex.ToString();
									}
								}
							}, CancellationToken.None, TaskCreationOptions.RunContinuationsAsynchronously, TaskScheduler.Current);
						}
					}
					goto IL_30E;
				}
				IL_2DF:
				PolyfewRuntime.CustomMeshActionStructure customMeshActionStructure2 = CS$<>8__locals2.callbackFlusher[0];
				CS$<>8__locals2.callbackFlusher.RemoveAt(0);
				if (OnEachMeshSimplified != null)
				{
					OnEachMeshSimplified(customMeshActionStructure2.meshRendererPair.mesh);
				}
				IL_30E:
				if (CS$<>8__locals2.callbackFlusher.Count > 0)
				{
					goto IL_2DF;
				}
				while (CS$<>8__locals2.CS$<>8__locals1.meshesHandled < count2 && !CS$<>8__locals2.CS$<>8__locals1.isError)
				{
					while (CS$<>8__locals2.callbackFlusher.Count > 0)
					{
						PolyfewRuntime.CustomMeshActionStructure customMeshActionStructure3 = CS$<>8__locals2.callbackFlusher[0];
						CS$<>8__locals2.callbackFlusher.RemoveAt(0);
						if (OnEachMeshSimplified != null)
						{
							OnEachMeshSimplified(customMeshActionStructure3.meshRendererPair.mesh);
						}
					}
				}
				if (CS$<>8__locals2.CS$<>8__locals1.isError)
				{
					goto IL_4B5;
				}
				using (List<PolyfewRuntime.CustomMeshActionStructure>.Enumerator enumerator2 = CS$<>8__locals2.meshAssignments.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						PolyfewRuntime.CustomMeshActionStructure customMeshActionStructure4 = enumerator2.Current;
						if (customMeshActionStructure4 != null)
						{
							customMeshActionStructure4.action();
						}
					}
					goto IL_4B5;
				}
			}
			foreach (Mesh mesh in meshesToSimplify)
			{
				if (!(mesh == null))
				{
					MeshSimplifier meshSimplifier2 = new MeshSimplifier();
					PolyfewRuntime.SetParametersForSimplifier(CS$<>8__locals1.simplificationOptions, meshSimplifier2);
					meshSimplifier2.Initialize(mesh, false);
					if (!CS$<>8__locals1.simplificationOptions.simplifyMeshLossless)
					{
						meshSimplifier2.SimplifyMesh(CS$<>8__locals1.quality);
					}
					else
					{
						meshSimplifier2.SimplifyMeshLossless();
					}
					if (OnEachMeshSimplified != null)
					{
						OnEachMeshSimplified(mesh);
					}
					Mesh mesh2 = meshSimplifier2.ToMesh();
					mesh2.bindposes = mesh.bindposes;
					mesh2.name = mesh.name.Replace("-POLY_REDUCED", "") + "-POLY_REDUCED";
					if (meshSimplifier2.RecalculateNormals)
					{
						mesh2.RecalculateNormals();
						mesh2.RecalculateTangents();
					}
					CS$<>8__locals1.simplifiedMeshes.Add(mesh2);
				}
			}
			IL_4B5:
			return CS$<>8__locals1.simplifiedMeshes;
		}

		public static PolyfewRuntime.ObjectMeshPairs GetObjectMeshPairs(GameObject forObject, bool includeInactive)
		{
			if (forObject == null)
			{
				throw new ArgumentNullException("forObject", "You must provide a gameobject to get the ObjectMeshPairs for.");
			}
			PolyfewRuntime.ObjectMeshPairs objectMeshPairs = new PolyfewRuntime.ObjectMeshPairs();
			MeshFilter[] componentsInChildren = forObject.GetComponentsInChildren<MeshFilter>(includeInactive);
			if (componentsInChildren != null && componentsInChildren.Length != 0)
			{
				foreach (MeshFilter meshFilter in componentsInChildren)
				{
					if (meshFilter.sharedMesh)
					{
						PolyfewRuntime.MeshRendererPair value = new PolyfewRuntime.MeshRendererPair(true, meshFilter.sharedMesh);
						objectMeshPairs.Add(meshFilter.gameObject, value);
					}
				}
			}
			SkinnedMeshRenderer[] componentsInChildren2 = forObject.GetComponentsInChildren<SkinnedMeshRenderer>(includeInactive);
			if (componentsInChildren2 != null && componentsInChildren2.Length != 0)
			{
				foreach (SkinnedMeshRenderer skinnedMeshRenderer in componentsInChildren2)
				{
					if (skinnedMeshRenderer.sharedMesh)
					{
						PolyfewRuntime.MeshRendererPair value2 = new PolyfewRuntime.MeshRendererPair(false, skinnedMeshRenderer.sharedMesh);
						objectMeshPairs.Add(skinnedMeshRenderer.gameObject, value2);
					}
				}
			}
			return objectMeshPairs;
		}

		public static void CombineMeshesInGameObject(GameObject forObject, bool skipInactiveChildObjects, Action<string, string> OnError, PolyfewRuntime.MeshCombineTarget combineTarget = PolyfewRuntime.MeshCombineTarget.SkinnedAndStatic)
		{
			if (forObject == null)
			{
				if (OnError != null)
				{
					OnError("Argument Null Exception", "You must provide a gameobject whose meshes will be combined.");
				}
				return;
			}
			Renderer[] childRenderersForCombining = UtilityServicesRuntime.GetChildRenderersForCombining(forObject, skipInactiveChildObjects);
			if (childRenderersForCombining == null || childRenderersForCombining.Length == 0)
			{
				if (OnError != null)
				{
					OnError("Operation Failed", "No feasible renderers found under the provided object to combine.");
				}
				return;
			}
			HashSet<Transform> hashSet = new HashSet<Transform>();
			HashSet<Transform> hashSet2 = new HashSet<Transform>();
			MeshRenderer[] array;
			SkinnedMeshRenderer[] array2;
			if (skipInactiveChildObjects)
			{
				array = (from renderer in childRenderersForCombining
				where renderer.enabled && renderer as MeshRenderer != null && renderer.transform.GetComponent<MeshFilter>() != null && renderer.transform.GetComponent<MeshFilter>().sharedMesh != null
				select renderer as MeshRenderer).ToArray<MeshRenderer>();
				array2 = (from renderer in childRenderersForCombining
				where renderer.enabled && renderer as SkinnedMeshRenderer != null && renderer.transform.GetComponent<SkinnedMeshRenderer>().sharedMesh != null
				select renderer as SkinnedMeshRenderer).ToArray<SkinnedMeshRenderer>();
			}
			else
			{
				array = (from renderer in childRenderersForCombining
				where renderer as MeshRenderer != null && renderer.transform.GetComponent<MeshFilter>() != null && renderer.transform.GetComponent<MeshFilter>().sharedMesh != null
				select renderer as MeshRenderer).ToArray<MeshRenderer>();
				array2 = (from renderer in childRenderersForCombining
				where renderer as SkinnedMeshRenderer != null && renderer.transform.GetComponent<SkinnedMeshRenderer>().sharedMesh != null
				select renderer as SkinnedMeshRenderer).ToArray<SkinnedMeshRenderer>();
			}
			if (array != null)
			{
				foreach (MeshRenderer meshRenderer in array)
				{
					hashSet.Add(meshRenderer.transform);
				}
			}
			if (array2 != null)
			{
				foreach (SkinnedMeshRenderer skinnedMeshRenderer in array2)
				{
					hashSet2.Add(skinnedMeshRenderer.transform);
				}
			}
			if (combineTarget == PolyfewRuntime.MeshCombineTarget.StaticOnly)
			{
				array2 = new SkinnedMeshRenderer[0];
			}
			else if (combineTarget == PolyfewRuntime.MeshCombineTarget.SkinnedOnly)
			{
				array = new MeshRenderer[0];
			}
			MeshCombiner.StaticRenderer[] staticRenderers = MeshCombiner.GetStaticRenderers(array);
			MeshCombiner.SkinnedRenderer[] skinnedRenderers = MeshCombiner.GetSkinnedRenderers(array2);
			int num = (from renderer in array2
			where renderer.sharedMesh != null
			select renderer).Count<SkinnedMeshRenderer>();
			int num2 = (staticRenderers == null) ? 0 : staticRenderers.Length;
			if (skinnedRenderers != null)
			{
				int num3 = skinnedRenderers.Length;
			}
			if ((num2 == 0 || num2 == 1) && (num == 0 || num == 1))
			{
				string arg = "Nothing combined in GameObject \"" + forObject.name + "\". Not enough feasible renderers/meshes to combine.";
				if (combineTarget == PolyfewRuntime.MeshCombineTarget.StaticOnly)
				{
					arg = "Nothing combined in GameObject \"" + forObject.name + "\". Not enough feasible static meshes to combine. Consider selecting the option of combining both skinned and static meshes.";
				}
				else if (combineTarget == PolyfewRuntime.MeshCombineTarget.SkinnedOnly)
				{
					arg = "Nothing combined in GameObject \"" + forObject.name + "\". Not enough feasible skinned meshes to combine. Consider selecting the option of combining both skinned and static meshes.";
				}
				if (OnError != null)
				{
					OnError("Operation Failed", arg);
				}
				return;
			}
			SkinnedMeshRenderer[] array5 = null;
			MeshCombiner.StaticRenderer[] array6 = MeshCombiner.CombineStaticMeshes(forObject.transform, -1, array, false, "");
			MeshCombiner.SkinnedRenderer[] array7 = MeshCombiner.CombineSkinnedMeshes(forObject.transform, -1, array2, ref array5, false, "");
			if (array5 != null)
			{
				SkinnedMeshRenderer[] array4 = array5;
				for (int i = 0; i < array4.Length; i++)
				{
					array4[i].enabled = false;
				}
			}
			if (array != null)
			{
				MeshRenderer[] array3 = array;
				for (int i = 0; i < array3.Length; i++)
				{
					array3[i].enabled = false;
				}
			}
			int num4 = (array6 == null) ? 0 : array6.Length;
			int num5 = (array7 == null) ? 0 : array7.Length;
			Transform transform = forObject.transform;
			HashSet<Transform> hashSet3 = new HashSet<Transform>();
			HashSet<Transform> hashSet4 = new HashSet<Transform>();
			for (int j = 0; j < num4; j++)
			{
				MeshCombiner.StaticRenderer staticRenderer = array6[j];
				Mesh mesh = staticRenderer.mesh;
				MeshRenderer meshRenderer2 = UtilityServicesRuntime.CreateStaticLevelRenderer(string.Format("{0}_combined_static", staticRenderer.name.Replace("_combined", "")), transform, staticRenderer.transform, mesh, staticRenderer.materials);
				hashSet3.Add(meshRenderer2.transform);
				meshRenderer2.transform.parent = forObject.transform;
			}
			for (int k = 0; k < num5; k++)
			{
				MeshCombiner.SkinnedRenderer skinnedRenderer = array7[k];
				Mesh mesh2 = skinnedRenderer.mesh;
				SkinnedMeshRenderer skinnedMeshRenderer2 = UtilityServicesRuntime.CreateSkinnedLevelRenderer(string.Format("{0}_combined_skinned", skinnedRenderer.name.Replace("_combined", "")), transform, skinnedRenderer.transform, mesh2, skinnedRenderer.materials, skinnedRenderer.rootBone, skinnedRenderer.bones);
				hashSet4.Add(skinnedMeshRenderer2.transform);
				skinnedMeshRenderer2.transform.parent = forObject.transform;
			}
			GameObject gameObject = new GameObject(forObject.name + "_bonesHiererachy");
			gameObject.transform.parent = forObject.transform;
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.localRotation = Quaternion.identity;
			gameObject.transform.localScale = Vector3.one;
			Transform[] array8 = new Transform[forObject.transform.childCount];
			for (int l = 0; l < forObject.transform.childCount; l++)
			{
				array8[l] = forObject.transform.GetChild(l);
			}
			foreach (Transform transform2 in array8)
			{
				if (combineTarget == PolyfewRuntime.MeshCombineTarget.SkinnedAndStatic)
				{
					if (!hashSet4.Contains(transform2) && !hashSet3.Contains(transform2))
					{
						transform2.parent = gameObject.transform;
					}
				}
				else if (combineTarget == PolyfewRuntime.MeshCombineTarget.StaticOnly)
				{
					if (!hashSet3.Contains(transform2) && !hashSet2.Contains(transform2))
					{
						transform2.parent = gameObject.transform;
					}
				}
				else if (!hashSet4.Contains(transform2) && !hashSet.Contains(transform2))
				{
					transform2.parent = gameObject.transform;
				}
			}
			if (array5 != null)
			{
				foreach (SkinnedMeshRenderer skinnedMeshRenderer3 in array5)
				{
					if (!(skinnedMeshRenderer3 == null))
					{
						skinnedMeshRenderer3.sharedMesh = null;
					}
				}
			}
			if (array != null)
			{
				foreach (MeshRenderer meshRenderer3 in array)
				{
					if (!(meshRenderer3 == null))
					{
						MeshFilter component = meshRenderer3.GetComponent<MeshFilter>();
						if (!(component == null))
						{
							component.sharedMesh = null;
						}
					}
				}
			}
		}

		public static GameObject CombineMeshesFromRenderers(Transform rootTransform, MeshRenderer[] originalMeshRenderers, SkinnedMeshRenderer[] originalSkinnedMeshRenderers, Action<string, string> OnError)
		{
			if (rootTransform == null)
			{
				if (OnError != null)
				{
					OnError("Argument Null Exception", "You must provide a root transform to create the combined meshes based from.");
				}
				return null;
			}
			if ((originalMeshRenderers == null || originalMeshRenderers.Length == 0) && (originalSkinnedMeshRenderers == null || originalSkinnedMeshRenderers.Length == 0))
			{
				if (OnError != null)
				{
					OnError("Operation Failed", "Both the Static and Skinned renderers list is empty. Atleast one of them must be non empty.");
				}
				return null;
			}
			if (originalMeshRenderers == null)
			{
				originalMeshRenderers = new MeshRenderer[0];
			}
			if (originalSkinnedMeshRenderers == null)
			{
				originalSkinnedMeshRenderers = new SkinnedMeshRenderer[0];
			}
			originalMeshRenderers = (from renderer in originalMeshRenderers
			where renderer.transform.GetComponent<MeshFilter>() != null && renderer.transform.GetComponent<MeshFilter>().sharedMesh != null
			select renderer).ToArray<MeshRenderer>();
			originalSkinnedMeshRenderers = (from renderer in originalSkinnedMeshRenderers
			where renderer.transform.GetComponent<SkinnedMeshRenderer>().sharedMesh != null
			select renderer).ToArray<SkinnedMeshRenderer>();
			if ((originalMeshRenderers == null || originalMeshRenderers.Length == 0) && (originalSkinnedMeshRenderers == null || originalSkinnedMeshRenderers.Length == 0))
			{
				if (OnError != null)
				{
					OnError("Operation Failed", "Couldn't combine any meshes. Couldn't find any feasible renderers in the provided lists to combine.");
				}
				return null;
			}
			SkinnedMeshRenderer[] array = null;
			MeshCombiner.StaticRenderer[] array2 = MeshCombiner.CombineStaticMeshes(rootTransform, -1, originalMeshRenderers, false, "");
			MeshCombiner.SkinnedRenderer[] array3 = MeshCombiner.CombineSkinnedMeshes(rootTransform, -1, originalSkinnedMeshRenderers, ref array, false, "");
			if ((array2 == null || array2.Length == 0) && (array3 == null || array3.Length == 0))
			{
				if (OnError != null)
				{
					OnError("Operation Failed", "Couldn't combine any meshes due to unknown reasons.");
				}
				return null;
			}
			GameObject gameObject = new GameObject(rootTransform.name + "_Combined_Meshes");
			Transform transform = gameObject.transform;
			if (array2 != null)
			{
				foreach (MeshCombiner.StaticRenderer staticRenderer in array2)
				{
					Mesh mesh = staticRenderer.mesh;
					UtilityServicesRuntime.CreateStaticLevelRenderer(string.Format("{0}_combined_static", staticRenderer.name.Replace("_combined", "")), transform, staticRenderer.transform, mesh, staticRenderer.materials);
				}
			}
			if (array3 != null)
			{
				foreach (MeshCombiner.SkinnedRenderer skinnedRenderer in array3)
				{
					Mesh mesh2 = skinnedRenderer.mesh;
					UtilityServicesRuntime.CreateSkinnedLevelRenderer(string.Format("{0}_combined_skinned", skinnedRenderer.name.Replace("_combined", "")), transform, skinnedRenderer.transform, mesh2, skinnedRenderer.materials, skinnedRenderer.rootBone, skinnedRenderer.bones);
				}
			}
			return gameObject;
		}

		public static async void ImportOBJFromFileSystem(string objAbsolutePath, string texturesFolderPath, string materialsFolderPath, Action<GameObject> OnSuccess, Action<Exception> OnError, PolyfewRuntime.OBJImportOptions importOptions = null)
		{
			UtilityServicesRuntime.OBJExporterImporter objexporterImporter = new UtilityServicesRuntime.OBJExporterImporter();
			bool isWorking = true;
			try
			{
				await objexporterImporter.ImportFromLocalFileSystem(objAbsolutePath, texturesFolderPath, materialsFolderPath, delegate(GameObject importedObject)
				{
					isWorking = false;
					OnSuccess(importedObject);
				}, importOptions);
				goto IL_13C;
			}
			catch (Exception obj)
			{
				isWorking = false;
				OnError(obj);
				goto IL_13C;
			}
			IL_E4:
			await Task.Delay(1);
			IL_13C:
			if (!isWorking)
			{
				return;
			}
			goto IL_E4;
		}

		public static async void ImportOBJFromNetwork(string objURL, string objName, string diffuseTexURL, string bumpTexURL, string specularTexURL, string opacityTexURL, string materialURL, PolyfewRuntime.ReferencedNumeric<float> downloadProgress, Action<GameObject> OnSuccess, Action<Exception> OnError, PolyfewRuntime.OBJImportOptions importOptions = null)
		{
			UtilityServicesRuntime.OBJExporterImporter objexporterImporter = new UtilityServicesRuntime.OBJExporterImporter();
			bool isWorking = true;
			objexporterImporter.ImportFromNetwork(objURL, objName, diffuseTexURL, bumpTexURL, specularTexURL, opacityTexURL, materialURL, downloadProgress, delegate(GameObject importedObject)
			{
				isWorking = false;
				OnSuccess(importedObject);
			}, delegate(Exception ex)
			{
				isWorking = false;
				OnError(ex);
			}, importOptions);
			while (isWorking)
			{
				await Task.Delay(1);
			}
		}

		public static async void ExportGameObjectToOBJ(GameObject toExport, string exportPath, Action OnSuccess, Action<Exception> OnError, PolyfewRuntime.OBJExportOptions exportOptions = null)
		{
			UtilityServicesRuntime.OBJExporterImporter objexporterImporter = new UtilityServicesRuntime.OBJExporterImporter();
			bool isWorking = true;
			try
			{
				objexporterImporter.ExportGameObjectToOBJ(toExport, exportPath, exportOptions, delegate
				{
					isWorking = false;
					OnSuccess();
				});
				goto IL_D9;
			}
			catch (Exception obj)
			{
				isWorking = false;
				OnError(obj);
				goto IL_D9;
			}
			IL_81:
			await Task.Delay(1);
			IL_D9:
			if (!isWorking)
			{
				return;
			}
			goto IL_81;
		}

		public static int CountTriangles(bool countDeep, GameObject forObject)
		{
			int num = 0;
			if (forObject == null)
			{
				return 0;
			}
			if (countDeep)
			{
				MeshFilter[] componentsInChildren = forObject.GetComponentsInChildren<MeshFilter>(true);
				if (componentsInChildren != null && componentsInChildren.Length != 0)
				{
					foreach (MeshFilter meshFilter in componentsInChildren)
					{
						if (meshFilter.sharedMesh)
						{
							num += meshFilter.sharedMesh.triangles.Length / 3;
						}
					}
				}
				SkinnedMeshRenderer[] componentsInChildren2 = forObject.GetComponentsInChildren<SkinnedMeshRenderer>(true);
				if (componentsInChildren2 != null && componentsInChildren2.Length != 0)
				{
					foreach (SkinnedMeshRenderer skinnedMeshRenderer in componentsInChildren2)
					{
						if (skinnedMeshRenderer.sharedMesh)
						{
							num += skinnedMeshRenderer.sharedMesh.triangles.Length / 3;
						}
					}
				}
			}
			else
			{
				MeshFilter component = forObject.GetComponent<MeshFilter>();
				SkinnedMeshRenderer component2 = forObject.GetComponent<SkinnedMeshRenderer>();
				if (component && component.sharedMesh)
				{
					num = component.sharedMesh.triangles.Length / 3;
				}
				else if (component2 && component2.sharedMesh)
				{
					num = component2.sharedMesh.triangles.Length / 3;
				}
			}
			return num;
		}

		public static int CountTriangles(List<Mesh> toCount)
		{
			int num = 0;
			if (toCount == null || toCount.Count == 0)
			{
				return 0;
			}
			foreach (Mesh mesh in toCount)
			{
				if (mesh != null)
				{
					num += mesh.triangles.Length / 3;
				}
			}
			return num;
		}

		private static void SetParametersForSimplifier(PolyfewRuntime.SimplificationOptions simplificationOptions, MeshSimplifier meshSimplifier)
		{
			meshSimplifier.RecalculateNormals = simplificationOptions.recalculateNormals;
			meshSimplifier.EnableSmartLink = simplificationOptions.enableSmartlinking;
			meshSimplifier.PreserveUVSeamEdges = simplificationOptions.preserveUVSeamEdges;
			meshSimplifier.PreserveUVFoldoverEdges = simplificationOptions.preserveUVFoldoverEdges;
			meshSimplifier.PreserveBorderEdges = simplificationOptions.preserveBorderEdges;
			meshSimplifier.MaxIterationCount = simplificationOptions.maxIterations;
			meshSimplifier.Aggressiveness = (double)simplificationOptions.aggressiveness;
			meshSimplifier.RegardCurvature = simplificationOptions.regardCurvature;
			meshSimplifier.UseSortedEdgeMethod = simplificationOptions.useEdgeSort;
		}

		private static bool AreAnyFeasibleMeshes(PolyfewRuntime.ObjectMeshPairs objectMeshPairs)
		{
			if (objectMeshPairs == null || objectMeshPairs.Count == 0)
			{
				return false;
			}
			foreach (KeyValuePair<GameObject, PolyfewRuntime.MeshRendererPair> keyValuePair in objectMeshPairs)
			{
				PolyfewRuntime.MeshRendererPair value = keyValuePair.Value;
				GameObject key = keyValuePair.Key;
				if (!(key == null) && value != null)
				{
					if (value.attachedToMeshFilter)
					{
						if (!(key.GetComponent<MeshFilter>() == null) && !(value.mesh == null))
						{
							return true;
						}
					}
					else if (!value.attachedToMeshFilter && !(key.GetComponent<SkinnedMeshRenderer>() == null) && !(value.mesh == null))
					{
						return true;
					}
				}
			}
			return false;
		}

		private static void AssignReducedMesh(GameObject gameObject, Mesh originalMesh, Mesh reducedMesh, bool attachedToMeshfilter, bool assignBindposes)
		{
			if (assignBindposes)
			{
				reducedMesh.bindposes = originalMesh.bindposes;
			}
			reducedMesh.name = originalMesh.name.Replace("-POLY_REDUCED", "") + "-POLY_REDUCED";
			if (attachedToMeshfilter)
			{
				MeshFilter component = gameObject.GetComponent<MeshFilter>();
				if (component != null)
				{
					component.sharedMesh = reducedMesh;
					return;
				}
			}
			else
			{
				SkinnedMeshRenderer component2 = gameObject.GetComponent<SkinnedMeshRenderer>();
				if (component2 != null)
				{
					component2.sharedMesh = reducedMesh;
				}
			}
		}

		private static int CountTriangles(PolyfewRuntime.ObjectMeshPairs objectMeshPairs)
		{
			int num = 0;
			if (objectMeshPairs == null)
			{
				return 0;
			}
			foreach (KeyValuePair<GameObject, PolyfewRuntime.MeshRendererPair> keyValuePair in objectMeshPairs)
			{
				if (!(keyValuePair.Key == null) && keyValuePair.Value != null && !(keyValuePair.Value.mesh == null))
				{
					num += keyValuePair.Value.mesh.triangles.Length / 3;
				}
			}
			return num;
		}

		private const int MAX_LOD_COUNT = 8;

		[Serializable]
		public class ObjectMeshPairs : Dictionary<GameObject, PolyfewRuntime.MeshRendererPair>
		{
		}

		public enum MeshCombineTarget
		{
			SkinnedAndStatic,
			StaticOnly,
			SkinnedOnly
		}

		[Serializable]
		public class MeshRendererPair
		{
			public MeshRendererPair(bool attachedToMeshFilter, Mesh mesh)
			{
				this.attachedToMeshFilter = attachedToMeshFilter;
				this.mesh = mesh;
			}

			public void Destruct()
			{
				if (this.mesh != null)
				{
					Object.DestroyImmediate(this.mesh);
				}
			}

			public bool attachedToMeshFilter;

			public Mesh mesh;
		}

		[Serializable]
		public class CustomMeshActionStructure
		{
			public CustomMeshActionStructure(PolyfewRuntime.MeshRendererPair meshRendererPair, GameObject gameObject, Action action)
			{
				this.meshRendererPair = meshRendererPair;
				this.gameObject = gameObject;
				this.action = action;
			}

			public PolyfewRuntime.MeshRendererPair meshRendererPair;

			public GameObject gameObject;

			public Action action;
		}

		[Serializable]
		public class SimplificationOptions
		{
			public SimplificationOptions()
			{
			}

			public SimplificationOptions(float simplificationStrength, bool simplifyOptimal, bool enableSmartlink, bool recalculateNormals, bool preserveUVSeamEdges, bool preserveUVFoldoverEdges, bool preserveBorderEdges, bool regardToleranceSphere, List<PolyfewRuntime.PreservationSphere> preservationSpheres, bool regardCurvature, int maxIterations, float aggressiveness, bool useEdgeSort)
			{
				this.simplificationStrength = simplificationStrength;
				this.simplifyMeshLossless = simplifyOptimal;
				this.enableSmartlinking = enableSmartlink;
				this.recalculateNormals = recalculateNormals;
				this.preserveUVSeamEdges = preserveUVSeamEdges;
				this.preserveUVFoldoverEdges = preserveUVFoldoverEdges;
				this.preserveBorderEdges = preserveBorderEdges;
				this.regardPreservationSpheres = regardToleranceSphere;
				this.preservationSpheres = preservationSpheres;
				this.regardCurvature = regardCurvature;
				this.maxIterations = maxIterations;
				this.aggressiveness = aggressiveness;
				this.useEdgeSort = useEdgeSort;
			}

			public float simplificationStrength;

			public bool simplifyMeshLossless;

			public bool enableSmartlinking = true;

			public bool recalculateNormals;

			public bool preserveUVSeamEdges;

			public bool preserveUVFoldoverEdges;

			public bool preserveBorderEdges;

			public bool regardPreservationSpheres;

			public List<PolyfewRuntime.PreservationSphere> preservationSpheres = new List<PolyfewRuntime.PreservationSphere>();

			public bool regardCurvature;

			public int maxIterations = 100;

			public float aggressiveness = 7f;

			public bool useEdgeSort;
		}

		[Serializable]
		public class PreservationSphere
		{
			public PreservationSphere(Vector3 worldPosition, float diameter, float preservationStrength)
			{
				this.worldPosition = worldPosition;
				this.diameter = diameter;
				this.preservationStrength = preservationStrength;
			}

			public Vector3 worldPosition;

			public float diameter;

			public float preservationStrength = 100f;
		}

		[Serializable]
		public class OBJImportOptions : ImportOptions
		{
		}

		[Serializable]
		public class OBJExportOptions
		{
			public OBJExportOptions(bool applyPosition, bool applyRotation, bool applyScale, bool generateMaterials, bool exportTextures)
			{
				this.applyPosition = applyPosition;
				this.applyRotation = applyRotation;
				this.applyScale = applyScale;
				this.generateMaterials = generateMaterials;
				this.exportTextures = exportTextures;
			}

			public readonly bool applyPosition = true;

			public readonly bool applyRotation = true;

			public readonly bool applyScale = true;

			public readonly bool generateMaterials = true;

			public readonly bool exportTextures = true;
		}

		public class ReferencedNumeric<T> where T : struct, IComparable, IComparable<T>, IConvertible, IEquatable<T>, IFormattable
		{
			public T Value
			{
				get
				{
					return this.val;
				}
				set
				{
					this.val = value;
				}
			}

			public ReferencedNumeric(T value)
			{
				this.val = value;
			}

			private T val;
		}
	}
}
