using UnityEngine;

public class DrawUtils
{
	private DrawUtils ()
	{
	}
	
	public static void DrawLine (Vector3 start, Vector3 end, Material material)
	{
		material.SetPass (0);
		GL.Begin (GL.LINES);
		GL.Vertex3 (start.x, start.y, start.z);
		GL.Vertex3 (end.x, end.y, end.z);
		GL.End ();
	}
	
	public static void DrawQuad (Vector3 center, float width, float height, Rect uvRect, Material material)
	{
		for (int i = 0; i < material.passCount; i++) {
			material.SetPass (i);
			float halfWidth = width * 0.5f;
			float halfHeight = height * 0.5f;
			GL.Begin (GL.QUADS);
			Vector3 vertex = center + new Vector3 (-halfWidth, -halfHeight, 0);
			GL.TexCoord2 (uvRect.xMin, uvRect.yMin);
			GL.Vertex3 (vertex.x, vertex.y, vertex.z);
			vertex = center + new Vector3 (-halfWidth, halfHeight, 0);
			GL.TexCoord2 (uvRect.xMin, uvRect.yMax);
			GL.Vertex3 (vertex.x, vertex.y, vertex.z);
			vertex = center + new Vector3 (halfWidth, halfHeight, 0);
			GL.TexCoord2 (uvRect.xMax, uvRect.yMax);
			GL.Vertex3 (vertex.x, vertex.y, vertex.z);
			vertex = center + new Vector3 (halfWidth, -halfHeight, 0);
			GL.TexCoord2 (uvRect.xMax, uvRect.yMin);
			GL.Vertex3 (vertex.x, vertex.y, vertex.z);
			GL.End ();
		}
	}
}