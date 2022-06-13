using System;
using UnityEngine;

public class TextureUVScrolling : MonoBehaviour
{
	private void Update()
	{
		this.offset -= Time.deltaTime * this.speed;
		this.mat.mainTextureOffset = new Vector2(0f, this.offset);
	}

	public Material mat;

	public float speed = 1f;

	private float offset;
}
