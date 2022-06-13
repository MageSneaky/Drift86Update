using System;
using System.Collections;
using BrainFailProductions.PolyFewRuntime;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PolygonReduction : MonoBehaviour
{
	private void Start()
	{
		if (Application.platform == RuntimePlatform.WebGLPlayer)
		{
			this.isWebGL = true;
		}
		this.uninteractivePanel.SetActive(false);
		this.exportButton.interactable = false;
		this.barabarianRef = this.targetObject;
		this.objectMeshPairs = PolyfewRuntime.GetObjectMeshPairs(this.targetObject, true);
		this.trianglesCount.text = string.Concat(PolyfewRuntime.CountTriangles(true, this.targetObject));
	}

	private void Update()
	{
		if (!this.eventSystem)
		{
			return;
		}
		if (this.eventSystem.currentSelectedGameObject && this.eventSystem.currentSelectedGameObject.GetComponent<RectTransform>())
		{
			FlyCamera.deactivated = true;
		}
		else
		{
			FlyCamera.deactivated = false;
		}
		if (this.isWebGL)
		{
			this.exportButton.gameObject.SetActive(false);
			this.importFromFileSystem.gameObject.SetActive(false);
		}
	}

	public void OnReductionChange(float value)
	{
		if (this.disableTemporary)
		{
			return;
		}
		this.didApplyLosslessLast = false;
		if (this.targetObject == null)
		{
			return;
		}
		if (Mathf.Approximately(0f, value))
		{
			this.AssignMeshesFromPairs();
			this.trianglesCount.text = string.Concat(PolyfewRuntime.CountTriangles(true, this.targetObject));
			return;
		}
		PolyfewRuntime.SimplificationOptions simplificationOptions = new PolyfewRuntime.SimplificationOptions();
		simplificationOptions.simplificationStrength = value;
		simplificationOptions.enableSmartlinking = this.enableSmartLinking.isOn;
		simplificationOptions.preserveBorderEdges = this.preserveBorders.isOn;
		simplificationOptions.preserveUVSeamEdges = this.preserveUVSeams.isOn;
		simplificationOptions.preserveUVFoldoverEdges = this.preserveUVFoldover.isOn;
		simplificationOptions.recalculateNormals = this.recalculateNormals.isOn;
		simplificationOptions.regardCurvature = this.regardCurvature.isOn;
		if (this.preserveFace.isOn)
		{
			simplificationOptions.regardPreservationSpheres = true;
			simplificationOptions.preservationSpheres.Add(new PolyfewRuntime.PreservationSphere(this.preservationSphere.position, this.preservationSphere.lossyScale.x, this.preservationStrength.value));
		}
		else
		{
			simplificationOptions.regardPreservationSpheres = false;
		}
		this.trianglesCount.text = string.Concat(PolyfewRuntime.SimplifyObjectDeep(this.objectMeshPairs, simplificationOptions, delegate(GameObject go, PolyfewRuntime.MeshRendererPair mInfo)
		{
		}));
	}

	public void SimplifyLossless()
	{
		this.disableTemporary = true;
		this.reductionStrength.value = 0f;
		this.disableTemporary = false;
		this.didApplyLosslessLast = true;
		PolyfewRuntime.SimplificationOptions simplificationOptions = new PolyfewRuntime.SimplificationOptions
		{
			enableSmartlinking = this.enableSmartLinking.isOn,
			preserveBorderEdges = this.preserveBorders.isOn,
			preserveUVSeamEdges = this.preserveUVSeams.isOn,
			preserveUVFoldoverEdges = this.preserveUVFoldover.isOn,
			recalculateNormals = this.recalculateNormals.isOn,
			regardCurvature = this.regardCurvature.isOn,
			simplifyMeshLossless = true
		};
		if (this.preserveFace.isOn)
		{
			simplificationOptions.regardPreservationSpheres = true;
		}
		else
		{
			simplificationOptions.regardPreservationSpheres = false;
		}
		this.trianglesCount.text = string.Concat(PolyfewRuntime.SimplifyObjectDeep(this.objectMeshPairs, simplificationOptions, delegate(GameObject go, PolyfewRuntime.MeshRendererPair mInfo)
		{
		}));
	}

	public void ImportOBJ()
	{
		PolyfewRuntime.OBJImportOptions objimportOptions = new PolyfewRuntime.OBJImportOptions();
		objimportOptions.zUp = false;
		objimportOptions.localPosition = new Vector3(-2.199f, -1f, -1.7349f);
		objimportOptions.localScale = new Vector3(0.045f, 0.045f, 0.045f);
		string objAbsolutePath = Application.dataPath + "/PolyFew/demo/TestModels/Meat.obj";
		string texturesFolderPath = Application.dataPath + "/PolyFew/demo/TestModels/textures";
		string materialsFolderPath = Application.dataPath + "/PolyFew/demo/TestModels/materials";
		GameObject importedObject;
		PolyfewRuntime.ImportOBJFromFileSystem(objAbsolutePath, texturesFolderPath, materialsFolderPath, delegate(GameObject imp)
		{
			importedObject = imp;
			Debug.Log("Successfully imported GameObject:   " + importedObject.name);
			this.barabarianRef.SetActive(false);
			this.targetObject = importedObject;
			this.ResetSettings();
			this.objectMeshPairs = PolyfewRuntime.GetObjectMeshPairs(this.targetObject, true);
			this.trianglesCount.text = string.Concat(PolyfewRuntime.CountTriangles(true, this.targetObject));
			this.exportButton.interactable = true;
			this.importFromWeb.interactable = false;
			this.importFromFileSystem.interactable = false;
			this.preserveFace.interactable = false;
			this.preservationStrength.interactable = false;
			this.disableTemporary = true;
			this.preservationSphere.gameObject.SetActive(false);
			this.disableTemporary = false;
		}, delegate(Exception ex)
		{
			Debug.LogError("Failed to load OBJ file.   " + ex.ToString());
		}, objimportOptions);
	}

	public void ImportOBJFromNetwork()
	{
		this.isImportingFromNetwork = true;
		PolyfewRuntime.OBJImportOptions objimportOptions = new PolyfewRuntime.OBJImportOptions();
		objimportOptions.zUp = false;
		objimportOptions.localPosition = new Vector3(0.87815f, 1.4417f, -4.4708f);
		objimportOptions.localScale = new Vector3(0.0042f, 0.0042f, 0.0042f);
		string objURL = "https://dl.dropbox.com/s/v09bh0hiivja10e/onion.obj?dl=1";
		string objName = "onion";
		string diffuseTexURL = "https://dl.dropbox.com/s/0u4ij6sddi7a3gc/onion.jpg?dl=1";
		string bumpTexURL = "";
		string specularTexURL = "";
		string opacityTexURL = "";
		string materialURL = "https://dl.dropbox.com/s/fuzryqigs4gxwvv/onion.mtl?dl=1";
		this.progressSlider.value = 0f;
		this.uninteractivePanel.SetActive(true);
		this.downloadProgress = new PolyfewRuntime.ReferencedNumeric<float>(0f);
		base.StartCoroutine(this.UpdateProgress());
		GameObject importedObject;
		PolyfewRuntime.ImportOBJFromNetwork(objURL, objName, diffuseTexURL, bumpTexURL, specularTexURL, opacityTexURL, materialURL, this.downloadProgress, delegate(GameObject imp)
		{
			this.AssignMeshesFromPairs();
			this.isImportingFromNetwork = false;
			importedObject = imp;
			this.barabarianRef.SetActive(false);
			this.targetObject = importedObject;
			this.ResetSettings();
			this.objectMeshPairs = PolyfewRuntime.GetObjectMeshPairs(this.targetObject, true);
			this.trianglesCount.text = string.Concat(PolyfewRuntime.CountTriangles(true, this.targetObject));
			this.exportButton.interactable = true;
			this.uninteractivePanel.SetActive(false);
			this.importFromWeb.interactable = false;
			this.importFromFileSystem.interactable = false;
			this.preserveFace.interactable = false;
			this.preservationStrength.interactable = false;
			this.disableTemporary = true;
			this.preservationSphere.gameObject.SetActive(false);
			this.disableTemporary = false;
		}, delegate(Exception ex)
		{
			this.uninteractivePanel.SetActive(false);
			this.isImportingFromNetwork = false;
			Debug.LogError("Failed to download and import OBJ file.   " + ex.Message);
		}, objimportOptions);
	}

	public void ExportGameObjectToOBJ()
	{
		string persistentDataPath = Application.persistentDataPath;
		GameObject exportObject = GameObject.Find("onion");
		if (exportObject)
		{
			exportObject = exportObject.transform.GetChild(0).GetChild(0).gameObject;
		}
		else
		{
			exportObject = GameObject.Find("Meat");
			if (!exportObject)
			{
				return;
			}
			exportObject = exportObject.transform.GetChild(0).GetChild(0).gameObject;
		}
		PolyfewRuntime.OBJExportOptions exportOptions = new PolyfewRuntime.OBJExportOptions(true, true, true, true, true);
		PolyfewRuntime.ExportGameObjectToOBJ(exportObject, persistentDataPath, delegate
		{
			Debug.Log("Successfully exported GameObject:  " + exportObject.name);
			string text = "Successfully exported the file to:  \n" + Application.persistentDataPath;
			this.StartCoroutine(this.ShowMessage(text));
		}, delegate(Exception ex)
		{
			Debug.LogError("Failed to export OBJ. " + ex.ToString());
		}, exportOptions);
	}

	public void OnToggleStateChanged(bool isOn)
	{
		if (this.disableTemporary)
		{
			return;
		}
		this.preservationSphere.gameObject.SetActive(this.preserveFace.isOn);
		if (this.didApplyLosslessLast)
		{
			this.SimplifyLossless();
			return;
		}
		this.preservationStrength.interactable = this.preserveFace.isOn;
		this.OnReductionChange(this.reductionStrength.value);
	}

	public void OnPreservationStrengthChange(float value)
	{
		this.OnToggleStateChanged(true);
	}

	public void Reset()
	{
		this.ResetSettings();
		this.AssignMeshesFromPairs();
		if (GameObject.Find("onion"))
		{
			this.targetObject.SetActive(false);
		}
		else if (GameObject.Find("Meat"))
		{
			this.targetObject.SetActive(false);
		}
		this.targetObject = this.barabarianRef;
		this.preserveFace.interactable = true;
		this.preservationStrength.interactable = this.preserveFace.isOn;
		this.targetObject.SetActive(true);
		this.objectMeshPairs = PolyfewRuntime.GetObjectMeshPairs(this.targetObject, true);
		this.trianglesCount.text = string.Concat(PolyfewRuntime.CountTriangles(true, this.targetObject));
		this.exportButton.interactable = false;
		this.importFromWeb.interactable = true;
		this.importFromFileSystem.interactable = true;
	}

	public static void OnSliderSelect()
	{
		FlyCamera.deactivated = true;
	}

	public static void OnSliderDeselect()
	{
		FlyCamera.deactivated = false;
	}

	private bool IsMouseOverUI(RectTransform uiElement)
	{
		Vector2 point = uiElement.InverseTransformPoint(Input.mousePosition);
		return uiElement.rect.Contains(point);
	}

	private IEnumerator ShowMessage(string message)
	{
		Debug.Log(message);
		this.message.text = message;
		yield return new WaitForSeconds(4.5f);
		this.message.text = "";
		yield break;
	}

	private void ResetSettings()
	{
		this.disableTemporary = true;
		this.reductionStrength.value = 0f;
		this.preserveUVSeams.isOn = false;
		this.preserveUVFoldover.isOn = false;
		this.preserveBorders.isOn = false;
		this.enableSmartLinking.isOn = true;
		this.preserveFace.isOn = false;
		this.preservationSphere.gameObject.SetActive(false);
		this.disableTemporary = false;
		this.preservationStrength.value = 100f;
	}

	private IEnumerator UpdateProgress()
	{
		for (;;)
		{
			yield return new WaitForSeconds(0.1f);
			this.progressSlider.value = this.downloadProgress.Value;
			this.progress.text = (int)this.downloadProgress.Value + "%";
		}
		yield break;
	}

	private void AssignMeshesFromPairs()
	{
		if (this.objectMeshPairs != null)
		{
			foreach (GameObject gameObject in this.objectMeshPairs.Keys)
			{
				if (gameObject != null)
				{
					PolyfewRuntime.MeshRendererPair meshRendererPair = this.objectMeshPairs[gameObject];
					if (!(meshRendererPair.mesh == null))
					{
						if (meshRendererPair.attachedToMeshFilter)
						{
							MeshFilter component = gameObject.GetComponent<MeshFilter>();
							if (!(component == null))
							{
								component.sharedMesh = meshRendererPair.mesh;
							}
						}
						else if (!meshRendererPair.attachedToMeshFilter)
						{
							SkinnedMeshRenderer component2 = gameObject.GetComponent<SkinnedMeshRenderer>();
							if (!(component2 == null))
							{
								component2.sharedMesh = meshRendererPair.mesh;
							}
						}
					}
				}
			}
		}
	}

	public Slider reductionStrength;

	public Slider preservationStrength;

	public Toggle preserveUVFoldover;

	public Toggle preserveUVSeams;

	public Toggle preserveBorders;

	public Toggle enableSmartLinking;

	public Toggle preserveFace;

	public Toggle recalculateNormals;

	public Toggle regardCurvature;

	public InputField trianglesCount;

	public Text message;

	public Text progress;

	public Button exportButton;

	public Button importFromFileSystem;

	public Button importFromWeb;

	public Slider progressSlider;

	public GameObject uninteractivePanel;

	public GameObject targetObject;

	public Transform preservationSphere;

	public EventSystem eventSystem;

	private PolyfewRuntime.ObjectMeshPairs objectMeshPairs;

	private bool didApplyLosslessLast;

	private bool disableTemporary;

	private GameObject barabarianRef;

	private PolyfewRuntime.ReferencedNumeric<float> downloadProgress = new PolyfewRuntime.ReferencedNumeric<float>(0f);

	private bool isImportingFromNetwork;

	private bool isWebGL;
}
