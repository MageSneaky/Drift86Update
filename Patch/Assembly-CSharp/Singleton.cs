using System;
using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
	public static T Instance
	{
		get
		{
			return Singleton<T>.instance;
		}
	}

	private void Awake()
	{
		if (Singleton<T>.instance == null)
		{
			Singleton<T>.instance = (this as T);
			if (this.dontDestroyOnLoad)
			{
				UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
			}
			this.AwakeSingleton();
			return;
		}
		UnityEngine.Object.Destroy(base.gameObject.GetComponent<T>());
	}

	protected abstract void AwakeSingleton();

	[SerializeField]
	private bool dontDestroyOnLoad;

	private static T instance;
}
