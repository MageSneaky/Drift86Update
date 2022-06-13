using System;
using UnityEngine;

public static class TransformExtensions
{
	public static T GetSafe<T>(this T[] array, int index)
	{
		if (array == null || index < 0 || index > array.Length)
		{
			return default(T);
		}
		return array[index];
	}

	public static void SetGlobalX(this Transform transform, float x)
	{
		Vector3 position = transform.position;
		position.x = x;
		transform.position = position;
	}

	public static void SetGlobalY(this Transform transform, float y)
	{
		Vector3 position = transform.position;
		position.y = y;
		transform.position = position;
	}

	public static void SetGlobalZ(this Transform transform, float z)
	{
		Vector3 position = transform.position;
		position.z = z;
		transform.position = position;
	}

	public static void SetLocalX(this Transform transform, float x)
	{
		Vector3 position = transform.position;
		position.x = x;
		transform.position = position;
	}

	public static void SetLocalY(this Transform transform, float y)
	{
		Vector3 localPosition = transform.localPosition;
		localPosition.y = y;
		transform.localPosition = localPosition;
	}

	public static void SetLocalZ(this Transform transform, float z)
	{
		Vector3 localPosition = transform.localPosition;
		localPosition.z = z;
		transform.localPosition = localPosition;
	}

	public static void SetAnchoredX(this RectTransform transform, float x)
	{
		Vector2 anchoredPosition = transform.anchoredPosition;
		anchoredPosition.x = x;
		transform.anchoredPosition = anchoredPosition;
	}

	public static void SetAnchoredY(this RectTransform transform, float y)
	{
		Vector2 anchoredPosition = transform.anchoredPosition;
		anchoredPosition.y = y;
		transform.anchoredPosition = anchoredPosition;
	}

	public static void SetAnchoredZ(this RectTransform transform, float z)
	{
		Vector3 anchoredPosition3D = transform.anchoredPosition3D;
		anchoredPosition3D.z = z;
		transform.anchoredPosition3D = anchoredPosition3D;
	}

	public static Vector3 ZeroHeight(this Vector3 vector)
	{
		vector.y = 0f;
		return vector;
	}
}
