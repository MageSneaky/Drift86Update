using System;
using UnityEngine;

public class TextureAnimator : MonoBehaviour
{
	private void Update()
	{
		int num = this.textures.Length;
		if (num == 0)
		{
			return;
		}
		this.frameTime += Time.deltaTime * this.speed;
		this.material.mainTexture = this.textures[(int)Mathf.Abs(this.frameTime) % num];
	}

	public float speed;

	public Material material;

	public Texture[] textures;

	private float frameTime;
}
