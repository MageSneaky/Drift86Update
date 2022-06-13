using System;
using UnityEngine;

public class WaterUVScroller : MonoBehaviour
{
	private void Awake()
	{
		this.m_renderers = base.gameObject.GetComponentsInChildren<Renderer>();
		if (this.m_renderers.Length == 0)
		{
			base.enabled = false;
		}
	}

	private void Update()
	{
		for (int i = 0; i < this.m_renderers.Length; i++)
		{
			Vector3 vector = this.m_direction.normalized;
			this.m_renderers[i].sharedMaterial.SetTextureOffset("_MainTex", Time.time * this.m_speedMainTex * vector);
			this.m_renderers[i].sharedMaterial.SetTextureOffset("_BumpMap", Time.time * this.m_speedBumpMap * vector);
		}
	}

	public Vector2 m_direction = new Vector2(1f, 0f);

	public float m_speedMainTex = 0.1f;

	public float m_speedBumpMap = 0.1f;

	private Renderer[] m_renderers;
}
