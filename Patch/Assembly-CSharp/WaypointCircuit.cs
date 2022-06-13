using System;
using System.Collections.Generic;
using UnityEngine;

public class WaypointCircuit : MonoBehaviour
{
	public float Length { get; private set; }

	public List<Transform> Waypoints
	{
		get
		{
			return this.m_WaypointList.items;
		}
	}

	public Transform GetLastPoint
	{
		get
		{
			return this.Waypoints[this.Waypoints.Count - 1];
		}
	}

	public void Awake()
	{
		if (this.Waypoints.Count > 1)
		{
			this.CachePositionsAndDistances();
		}
		this.NumPoints = this.Waypoints.Count;
	}

	public WaypointCircuit.RoutePoint GetRoutePoint(float dist)
	{
		Vector3 routePosition = this.GetRoutePosition(dist);
		return new WaypointCircuit.RoutePoint(routePosition, (this.GetRoutePosition(dist + 0.1f) - routePosition).normalized);
	}

	public Vector3 GetRoutePosition(float dist)
	{
		int num = 0;
		if (this.Length == 0f)
		{
			this.Length = this.Distances[this.Distances.Count - 1];
		}
		dist = Mathf.Repeat(dist, this.Length);
		try
		{
			while (this.Distances[num] < dist)
			{
				num++;
			}
		}
		catch (Exception)
		{
			Debug.LogError(dist);
		}
		this.P1n = (num - 1 + this.NumPoints) % this.NumPoints;
		this.p2n = num;
		this.I = Mathf.InverseLerp(this.Distances[this.P1n], this.Distances[this.p2n], dist);
		if (this.m_SmoothRoute)
		{
			this.P0n = (num - 2 + this.NumPoints) % this.NumPoints;
			this.p3n = (num + 1) % this.NumPoints;
			this.p2n %= this.NumPoints;
			this.P0 = this.Points[this.P0n];
			this.P1 = this.Points[this.P1n];
			this.P2 = this.Points[this.p2n];
			this.P3 = this.Points[this.p3n];
			return this.CatmullRom(this.P0, this.P1, this.P2, this.P3, this.I);
		}
		this.P1n = (num - 1 + this.NumPoints) % this.NumPoints;
		this.p2n = num;
		return Vector3.Lerp(this.Points[this.P1n], this.Points[this.p2n], this.I);
	}

	private Vector3 CatmullRom(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float i)
	{
		return 0.5f * (2f * p1 + (-p0 + p2) * i + (2f * p0 - 5f * p1 + 4f * p2 - p3) * i * i + (-p0 + 3f * p1 - 3f * p2 + p3) * i * i * i);
	}

	private void CachePositionsAndDistances()
	{
		this.Points = new List<Vector3>();
		this.Distances = new List<float>();
		float num = 0f;
		for (int i = 0; i < this.Waypoints.Count; i++)
		{
			Transform transform = this.Waypoints[i % this.Waypoints.Count];
			Transform transform2 = this.Waypoints[(i + 1) % this.Waypoints.Count];
			if (transform != null && transform2 != null)
			{
				Vector3 position = transform.position;
				Vector3 position2 = transform2.position;
				this.Points.Add(this.Waypoints[i % this.Waypoints.Count].position);
				this.Distances.Add(num);
				num += (position - position2).magnitude;
			}
		}
		if (this.Length == 0f)
		{
			this.Length = this.Distances[this.Distances.Count - 1];
		}
	}

	private void OnDrawGizmos()
	{
		this.DrawGizmos(false);
	}

	private void OnDrawGizmosSelected()
	{
		this.DrawGizmos(true);
	}

	private void DrawGizmos(bool selected)
	{
		if (!this.m_ShowGizmo)
		{
			return;
		}
		this.m_WaypointList.circuit = this;
		if (this.Waypoints.Count > 1)
		{
			this.NumPoints = this.Waypoints.Count;
			this.CachePositionsAndDistances();
			this.Length = this.Distances[this.Distances.Count - 1];
			Gizmos.color = (selected ? Color.yellow : new Color(1f, 1f, 0f, 0.5f));
			Vector3 vector = this.Waypoints[0].position;
			if (this.m_SmoothRoute)
			{
				for (float num = 0f; num < this.Length; num += this.Length / this.EditorVisualisationSubsteps)
				{
					Vector3 routePosition = this.GetRoutePosition(num + 1f);
					Gizmos.DrawLine(vector, routePosition);
					vector = routePosition;
				}
				Gizmos.DrawLine(vector, this.Waypoints[0].position);
				return;
			}
			for (int i = 0; i < this.Waypoints.Count; i++)
			{
				Vector3 position = this.Waypoints[(i + 1) % this.Waypoints.Count].position;
				Gizmos.DrawLine(vector, position);
				vector = position;
			}
		}
	}

	public WaypointCircuit.WaypointList m_WaypointList = new WaypointCircuit.WaypointList();

	[SerializeField]
	private bool m_ShowGizmo = true;

	[SerializeField]
	private bool m_SmoothRoute = true;

	private int NumPoints;

	private List<Vector3> Points;

	private List<float> Distances;

	public float EditorVisualisationSubsteps = 100f;

	private int P0n;

	private int P1n;

	private int p2n;

	private int p3n;

	private float I;

	private Vector3 P0;

	private Vector3 P1;

	private Vector3 P2;

	private Vector3 P3;

	[Serializable]
	public class WaypointList
	{
		public WaypointCircuit circuit;

		public List<Transform> items = new List<Transform>();
	}

	public struct RoutePoint
	{
		public RoutePoint(Vector3 position, Vector3 direction)
		{
			this.position = position;
			this.direction = direction;
		}

		public Vector3 position;

		public Vector3 direction;
	}
}
