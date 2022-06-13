using System;
using UnityEngine;

public class Readme : ScriptableObject
{
	public Texture2D icon;

	public float iconMaxWidth = 128f;

	public string title;

	public string titlesub;

	public Readme.Section[] sections;

	public bool loadedLayout;

	[Serializable]
	public class Section
	{
		public string heading;

		public string text;

		public string linkText;

		public string url;
	}
}
