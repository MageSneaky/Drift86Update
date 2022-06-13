using System;
using UnityEngine;
using UnityEngine.UI;

public class Buttonkey : MonoBehaviour
{
	private void Awake()
	{
		this._button = base.GetComponent<Button>();
	}

	private void Update()
	{
		if (Input.GetKeyDown(this._Key))
		{
			this._button.onClick.Invoke();
		}
	}

	public KeyCode _Key;

	private Button _button;
}
