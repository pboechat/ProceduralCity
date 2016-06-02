using UnityEngine;
using System.Collections.Generic;

public class RoadGraph
{
	public class Vertex
	{
		public float x;
		public float y;
		
		public Vertex (float x, float y)
		{
			this.x = x;
			this.y = y;
		}
		
	}
	
	public class Edge
	{
		public Vertex v1;
		public Vertex v2;
		public bool highway;
		
		public Edge (Vertex v1, Vertex v2)
		{
			this.v1 = v1;
			this.v2 = v2;
		}
		
	}
	
	private List<Vertex> _vertices = new List<Vertex> ();
	private List<Edge> _edges = new List<Edge> ();
	
	public List<Edge> GetEdges ()
	{
		return _edges;
	}
	
	public Vertex AddVertex (float x, float y)
	{
		Vertex v = new Vertex (x, y);
		_vertices.Add (v);
		return v;
	}
	
	public Edge AddEdge (Vertex v1, Vertex v2)
	{
		Edge e = new Edge (v1, v2);
		_edges.Add (e);
		return e;
	}
	
}