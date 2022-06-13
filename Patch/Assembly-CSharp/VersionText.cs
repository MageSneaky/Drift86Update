using System;
using TMPro;
using UnityEngine;

public class VersionText : MonoBehaviour
{
	private void Start()
	{
		this.m_VersionText.text = string.Format("{0}: {1}", "Demo version", Application.version);
	}

	[SerializeField]
	private TextMeshProUGUI m_VersionText;
}
