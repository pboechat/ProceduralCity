using UnityEngine;
using System.Collections.Generic;

public class MeshUtils
{
	private MeshUtils ()
	{
	}
	
	////////////////////////////////////////////////////////////////////////////////////////////////////
	public static Mesh CreateTopFaceMesh (float x1, 
										  float x2, 
										  float z1, 
										  float z2, 
										  float y, 
										  int xSegments,
									  	  int zSegments,
										  Rect uv1Rect, 
										  Rect uv2Rect,
										  Color defaultColor)
	{
		Mesh mesh = new Mesh ();
		
		List<Vector3> vertices = new List<Vector3> ();
		List<Vector3> normals = new List<Vector3> ();
		List<int> indices = new List<int> ();
		List<Vector2> uvs1 = new List<Vector2> ();
		List<Vector2> uvs2 = new List<Vector2> ();
		List<Color> colors = new List<Color> ();
		
		CreateTopFace (x1, x2, z1, z2, y, xSegments, xSegments, uv1Rect, uv2Rect, vertices, normals, uvs1, uvs2, indices, colors, defaultColor);
		
		mesh.vertices = vertices.ToArray ();
		mesh.normals = normals.ToArray ();
		mesh.uv = uvs1.ToArray ();
		mesh.uv2 = uvs2.ToArray ();
		mesh.colors = colors.ToArray ();
		mesh.triangles = indices.ToArray ();
		mesh.RecalculateBounds ();
		
		return mesh;
	}
	
	////////////////////////////////////////////////////////////////////////////////////////////////////
	public static Mesh CreateFrontFaceMesh (float x1, 
										  float x2, 
										  float y1, 
										  float y2, 
										  float z,
										  int xSegments,
										  int ySegments,
										  Rect uv1Rect, 
										  Rect uv2Rect,
										  Color defaultColor)
	{
		Mesh mesh = new Mesh ();
		
		List<Vector3> vertices = new List<Vector3> ();
		List<Vector3> normals = new List<Vector3> ();
		List<int> indices = new List<int> ();
		List<Vector2> uvs1 = new List<Vector2> ();
		List<Vector2> uvs2 = new List<Vector2> ();
		List<Color> colors = new List<Color> ();
		
		CreateFrontFace (x1, x2, y1, y2, z, xSegments, ySegments, uv1Rect, uv2Rect, vertices, normals, uvs1, uvs2, indices, colors, defaultColor);
		
		mesh.vertices = vertices.ToArray ();
		mesh.normals = normals.ToArray ();
		mesh.uv = uvs1.ToArray ();
		mesh.uv2 = uvs2.ToArray ();
		mesh.colors = colors.ToArray ();
		mesh.triangles = indices.ToArray ();
		mesh.RecalculateBounds ();
		
		return mesh;
	}
	
	////////////////////////////////////////////////////////////////////////////////////////////////////
	public static Mesh CreateBackFaceMesh (float x1, 
										  float x2, 
										  float y1, 
										  float y2, 
										  float z, 
										  int xSegments,
										  int ySegments,
										  Rect uv1Rect, 
										  Rect uv2Rect,
										  Color defaultColor)
	{
		Mesh mesh = new Mesh ();
		
		List<Vector3> vertices = new List<Vector3> ();
		List<Vector3> normals = new List<Vector3> ();
		List<int> indices = new List<int> ();
		List<Vector2> uvs1 = new List<Vector2> ();
		List<Vector2> uvs2 = new List<Vector2> ();
		List<Color> colors = new List<Color> ();
		
		CreateBackFace (x1, x2, y1, y2, z, xSegments, ySegments, uv1Rect, uv2Rect, vertices, normals, uvs1, uvs2, indices, colors, defaultColor);
		
		mesh.vertices = vertices.ToArray ();
		mesh.normals = normals.ToArray ();
		mesh.uv = uvs1.ToArray ();
		mesh.uv2 = uvs2.ToArray ();
		mesh.colors = colors.ToArray ();
		mesh.triangles = indices.ToArray ();
		mesh.RecalculateBounds ();
		
		return mesh;
	}
	
	////////////////////////////////////////////////////////////////////////////////////////////////////
	public static void CreateTopQuad (float x1, 
		                              float x2, 
		                              float z1, 
		                              float z2, 
		                              float y, 
		                              Rect uv1Rect, 
		                              Rect uv2Rect, 
		                              List<Vector3> vertices, 
		                              List<Vector3> normals, 
		                              List<Vector2> uvs1, 
		                              List<Vector2> uvs2, 
		                              List<int> indices,
									  List<Color> colors,
									  Color defaultColor)
	{
		int i = vertices.Count;
                
		vertices.Add (new Vector3 (x1, y, z2));
		vertices.Add (new Vector3 (x2, y, z2));
		vertices.Add (new Vector3 (x2, y, z1));
		vertices.Add (new Vector3 (x1, y, z1));
                        
		normals.Add (Vector3.up);
		normals.Add (Vector3.up);
		normals.Add (Vector3.up);
		normals.Add (Vector3.up);
                        
		uvs1.Add (new Vector2 (uv1Rect.xMin, uv1Rect.yMin));
		uvs1.Add (new Vector2 (uv1Rect.xMax, uv1Rect.yMin));
		uvs1.Add (new Vector2 (uv1Rect.xMax, uv1Rect.yMax));
		uvs1.Add (new Vector2 (uv1Rect.xMin, uv1Rect.yMax));
                
		uvs2.Add (new Vector2 (uv2Rect.xMin, uv2Rect.yMin));
		uvs2.Add (new Vector2 (uv2Rect.xMax, uv2Rect.yMin));
		uvs2.Add (new Vector2 (uv2Rect.xMax, uv2Rect.yMax));
		uvs2.Add (new Vector2 (uv2Rect.xMin, uv2Rect.yMax));
                        
		indices.Add (i);
		indices.Add (i + 1);
		indices.Add (i + 2);
		indices.Add (i);
		indices.Add (i + 2);
		indices.Add (i + 3);
		
		colors.Add (defaultColor);
		colors.Add (defaultColor);
		colors.Add (defaultColor);
		colors.Add (defaultColor);
	}
	
	////////////////////////////////////////////////////////////////////////////////////////////////////
	public static void CreateTopFace (float x1, 
									  float x2, 
									  float z1, 
									  float z2, 
									  float y, 
									  int xSegments,
									  int zSegments,
									  Rect uv1Rect, 
									  Rect uv2Rect, 
									  List<Vector3> vertices, 
									  List<Vector3> normals, 
									  List<Vector2> uvs1, 
									  List<Vector2> uvs2, 
									  List<int> indices,
									  List<Color> colors,
									  Color defaultColor)
	{
		float xIncrement = (x2 - x1) / (float)xSegments;
		float zIncrement = (z2 - z1) / (float)zSegments;
        
		Vector3 vScan = new Vector3 (x1, 0, z1);
		for (int z = 0; z < zSegments; z++) {
			Vector3 hScan = vScan;
			for (int x = 0; x < xSegments; x++) {
				CreateTopQuad (hScan.x, hScan.x + xIncrement, hScan.z, hScan.z + zIncrement, y, uv1Rect, uv2Rect, vertices, normals, uvs1, uvs2, indices, colors, defaultColor);
				hScan += new Vector3 (xIncrement, 0, 0); 
			}
			vScan += new Vector3 (0, 0, zIncrement); 
		}
	}
	
	////////////////////////////////////////////////////////////////////////////////////////////////////
	public static void CreateRightQuad (float y1, 
                                        float y2, 
                                        float z1, 
                                        float z2, 
                                        float x, 
                                        Rect uv1Rect, 
                                        Rect uv2Rect, 
                                        List<Vector3> vertices, 
                                        List<Vector3> normals, 
                                        List<Vector2> uvs1, 
                                        List<Vector2> uvs2, 
                                        List<int> indices,
									  	List<Color> colors,
									  	Color defaultColor)
	{
		int i = vertices.Count;
                
		vertices.Add (new Vector3 (x, y1, z1));
		vertices.Add (new Vector3 (x, y1, z2));
		vertices.Add (new Vector3 (x, y2, z2));
		vertices.Add (new Vector3 (x, y2, z1));
                        
		normals.Add (Vector3.right);
		normals.Add (Vector3.right);
		normals.Add (Vector3.right);
		normals.Add (Vector3.right);
                        
		uvs1.Add (new Vector2 (uv1Rect.xMin, uv1Rect.yMin));
		uvs1.Add (new Vector2 (uv1Rect.xMax, uv1Rect.yMin));
		uvs1.Add (new Vector2 (uv1Rect.xMax, uv1Rect.yMax));
		uvs1.Add (new Vector2 (uv1Rect.xMin, uv1Rect.yMax));
                
		uvs2.Add (new Vector2 (uv2Rect.xMin, uv2Rect.yMin));
		uvs2.Add (new Vector2 (uv2Rect.xMax, uv2Rect.yMin));
		uvs2.Add (new Vector2 (uv2Rect.xMax, uv2Rect.yMax));
		uvs2.Add (new Vector2 (uv2Rect.xMin, uv2Rect.yMax));
                        
		indices.Add (i);
		indices.Add (i + 1);
		indices.Add (i + 2);
		indices.Add (i);
		indices.Add (i + 2);
		indices.Add (i + 3);
		
		colors.Add (defaultColor);
		colors.Add (defaultColor);
		colors.Add (defaultColor);
		colors.Add (defaultColor);		
	}
	
	////////////////////////////////////////////////////////////////////////////////////////////////////
	public static void CreateRightFace (float y1, 
									    float y2, 
									    float z1, 
									    float z2, 
									    float x, 
										int zSegments,
									  	int ySegments,
									    Rect uv1Rect, 
									    Rect uv2Rect, 
									    List<Vector3> vertices, 
									    List<Vector3> normals, 
									    List<Vector2> uvs1, 
									    List<Vector2> uvs2, 
									    List<int> indices,
										List<Color> colors,
									   	Color defaultColor)
	{
		float zIncrement = (z2 - z1) / (float)zSegments;
		float yIncrement = (y2 - y1) / (float)ySegments;
		
		Vector3 vScan = new Vector3 (0, y1, z1);
		for (int y = 0; y < ySegments; y++) {
			Vector3 hScan = vScan;
			for (int z = 0; z < zSegments; z++) {
				CreateRightQuad (hScan.y, hScan.y + yIncrement, hScan.z, hScan.z + zIncrement, x, uv1Rect, uv2Rect, vertices, normals, uvs1, uvs2, indices, colors, defaultColor);
				hScan += new Vector3 (0, 0, zIncrement); 
			}
			vScan += new Vector3 (0, yIncrement, 0); 
		}
	}
	
	////////////////////////////////////////////////////////////////////////////////////////////////////
	public static void CreateLeftQuad (float y1, 
                                       float y2, 
                                       float z1, 
                                       float z2, 
                                       float x, 
                                       Rect uv1Rect, 
                                       Rect uv2Rect, 
                                       List<Vector3> vertices, 
                                       List<Vector3> normals, 
                                       List<Vector2> uvs1, 
                                       List<Vector2> uvs2, 
                                       List<int> indices,
									   List<Color> colors,
									   Color defaultColor)
	{
		int i = vertices.Count;
                
		vertices.Add (new Vector3 (x, y1, z2));
		vertices.Add (new Vector3 (x, y1, z1));
		vertices.Add (new Vector3 (x, y2, z1));
		vertices.Add (new Vector3 (x, y2, z2));
                        
		normals.Add (Vector3.left);
		normals.Add (Vector3.left);
		normals.Add (Vector3.left);
		normals.Add (Vector3.left);
                        
		uvs1.Add (new Vector2 (uv1Rect.xMin, uv1Rect.yMin));
		uvs1.Add (new Vector2 (uv1Rect.xMax, uv1Rect.yMin));
		uvs1.Add (new Vector2 (uv1Rect.xMax, uv1Rect.yMax));
		uvs1.Add (new Vector2 (uv1Rect.xMin, uv1Rect.yMax));
                
		uvs2.Add (new Vector2 (uv2Rect.xMin, uv2Rect.yMin));
		uvs2.Add (new Vector2 (uv2Rect.xMax, uv2Rect.yMin));
		uvs2.Add (new Vector2 (uv2Rect.xMax, uv2Rect.yMax));
		uvs2.Add (new Vector2 (uv2Rect.xMin, uv2Rect.yMax));
                        
		indices.Add (i);
		indices.Add (i + 1);
		indices.Add (i + 2);
		indices.Add (i);
		indices.Add (i + 2);
		indices.Add (i + 3);
		
		colors.Add (defaultColor);
		colors.Add (defaultColor);
		colors.Add (defaultColor);
		colors.Add (defaultColor);
	}
	
	////////////////////////////////////////////////////////////////////////////////////////////////////
	public static void CreateLeftFace (float y1, 
									   float y2, 
									   float z1, 
									   float z2, 
									   float x, 
									   int zSegments,
									   int ySegments,
									   Rect uv1Rect, 
									   Rect uv2Rect, 
									   List<Vector3> vertices, 
									   List<Vector3> normals, 
									   List<Vector2> uvs1, 
									   List<Vector2> uvs2,
									   List<int> indices,
									   List<Color> colors,
									   Color defaultColor)
	{
		float zIncrement = (z2 - z1) / (float)zSegments;
		float yIncrement = (y2 - y1) / (float)ySegments;
		
		Vector3 vScan = new Vector3 (0, y1, z1);
		for (int y = 0; y < ySegments; y++) {
			Vector3 hScan = vScan;
			for (int z = 0; z < zSegments; z++) {
				CreateLeftQuad (hScan.y, hScan.y + yIncrement, hScan.z, hScan.z + zIncrement, x, uv1Rect, uv2Rect, vertices, normals, uvs1, uvs2, indices, colors, defaultColor);
				hScan += new Vector3 (0, 0, zIncrement); 
			}
			vScan += new Vector3 (0, yIncrement, 0); 
		}
	}
	
	////////////////////////////////////////////////////////////////////////////////////////////////////
	public static void CreateFrontQuad (float x1, 
                                        float x2, 
                                        float y1, 
                                        float y2, 
                                        float z, 
                                        Rect uv1Rect, 
                                        Rect uv2Rect,
                                        List<Vector3> vertices, 
                                        List<Vector3> normals, 
                                        List<Vector2> uvs1, 
                                        List<Vector2> uvs2, 
                                        List<int> indices,
										List<Color> colors,
									   	Color defaultColor)
	{
		int i = vertices.Count;
                
		vertices.Add (new Vector3 (x1, y1, z));
		vertices.Add (new Vector3 (x2, y1, z));
		vertices.Add (new Vector3 (x2, y2, z));
		vertices.Add (new Vector3 (x1, y2, z));
                        
		normals.Add (Vector3.back);
		normals.Add (Vector3.back);
		normals.Add (Vector3.back);
		normals.Add (Vector3.back);
                
		uvs1.Add (new Vector2 (uv1Rect.xMin, uv1Rect.yMin));
		uvs1.Add (new Vector2 (uv1Rect.xMax, uv1Rect.yMin));
		uvs1.Add (new Vector2 (uv1Rect.xMax, uv1Rect.yMax));
		uvs1.Add (new Vector2 (uv1Rect.xMin, uv1Rect.yMax));
                
		uvs2.Add (new Vector2 (uv2Rect.xMin, uv2Rect.yMin));
		uvs2.Add (new Vector2 (uv2Rect.xMax, uv2Rect.yMin));
		uvs2.Add (new Vector2 (uv2Rect.xMax, uv2Rect.yMax));
		uvs2.Add (new Vector2 (uv2Rect.xMin, uv2Rect.yMax));
                
		indices.Add (i);
		indices.Add (i + 1);
		indices.Add (i + 2);
		indices.Add (i);
		indices.Add (i + 2);
		indices.Add (i + 3);
		
		colors.Add (defaultColor);
		colors.Add (defaultColor);
		colors.Add (defaultColor);
		colors.Add (defaultColor);
	}
	
	////////////////////////////////////////////////////////////////////////////////////////////////////
	public static void CreateFrontFace (float x1, 
									    float x2, 
									    float y1, 
									    float y2, 
									    float z, 
										int xSegments,
										int ySegments,
									    Rect uv1Rect, 
									    Rect uv2Rect,
									    List<Vector3> vertices, 
									    List<Vector3> normals, 
									    List<Vector2> uvs1, 
									    List<Vector2> uvs2, 
									    List<int> indices,
										List<Color> colors,
									   	Color defaultColor)
	{
		float xIncrement = (x2 - x1) / (float)xSegments;
		float yIncrement = (y1 - y2) / (float)ySegments;
		
		Vector3 vScan = new Vector3 (x1, y2, 0);
		for (int y = 0; y < ySegments; y++) {
			Vector3 hScan = vScan;
			for (int x = 0; x < xSegments; x++) {
				CreateFrontQuad (hScan.x, hScan.x + xIncrement, hScan.y + yIncrement, hScan.y, z, uv1Rect, uv2Rect, vertices, normals, uvs1, uvs2, indices, colors, defaultColor);
				hScan += new Vector3 (xIncrement, 0, 0); 
			}
			vScan += new Vector3 (0, yIncrement, 0); 
		}
	}
	
	////////////////////////////////////////////////////////////////////////////////////////////////////
	public static void CreateBackQuad (float x1, 
                                       float x2, 
                                       float y1, 
                                       float y2, 
                                       float z, 
                                       Rect uv1Rect, 
                                       Rect uv2Rect, 
                                       List<Vector3> vertices, 
                                       List<Vector3> normals, 
                                       List<Vector2> uvs1, 
                                       List<Vector2> uvs2, 
                                       List<int> indices,
									   List<Color> colors,
									   Color defaultColor)
	{
		int i = vertices.Count;
                
		vertices.Add (new Vector3 (x2, y1, z));
		vertices.Add (new Vector3 (x1, y1, z));
		vertices.Add (new Vector3 (x1, y2, z));
		vertices.Add (new Vector3 (x2, y2, z));
                        
		normals.Add (Vector3.forward);
		normals.Add (Vector3.forward);
		normals.Add (Vector3.forward);
		normals.Add (Vector3.forward);
                
		uvs1.Add (new Vector2 (uv1Rect.xMin, uv1Rect.yMin));
		uvs1.Add (new Vector2 (uv1Rect.xMax, uv1Rect.yMin));
		uvs1.Add (new Vector2 (uv1Rect.xMax, uv1Rect.yMax));
		uvs1.Add (new Vector2 (uv1Rect.xMin, uv1Rect.yMax));
                
		uvs2.Add (new Vector2 (uv2Rect.xMin, uv2Rect.yMin));
		uvs2.Add (new Vector2 (uv2Rect.xMax, uv2Rect.yMin));
		uvs2.Add (new Vector2 (uv2Rect.xMax, uv2Rect.yMax));
		uvs2.Add (new Vector2 (uv2Rect.xMin, uv2Rect.yMax));                
                        
		indices.Add (i);
		indices.Add (i + 1);
		indices.Add (i + 2);
		indices.Add (i);
		indices.Add (i + 2);
		indices.Add (i + 3);
		
		colors.Add (defaultColor);
		colors.Add (defaultColor);
		colors.Add (defaultColor);
		colors.Add (defaultColor);		
	}
	
	////////////////////////////////////////////////////////////////////////////////////////////////////
	public static void CreateBackFace (float x1, 
									   float x2, 
									   float y1, 
									   float y2, 
									   float z, 
									   int xSegments,
									   int ySegments,
									   Rect uv1Rect, 
									   Rect uv2Rect, 
									   List<Vector3> vertices, 
									   List<Vector3> normals, 
									   List<Vector2> uvs1, 
									   List<Vector2> uvs2, 
									   List<int> indices,
									   List<Color> colors,
									   Color defaultColor)
	{
		float xIncrement = (x2 - x1) / (float)xSegments;
		float yIncrement = (y1 - y2) / (float)ySegments;
		
		Vector3 vScan = new Vector3 (x1, y2, 0);
		for (int y = 0; y < ySegments; y++) {
			Vector3 hScan = vScan;
			for (int x = 0; x < xSegments; x++) {
				CreateBackQuad (hScan.x, hScan.x + xIncrement, hScan.y + yIncrement, hScan.y, z, uv1Rect, uv2Rect, vertices, normals, uvs1, uvs2, indices, colors, defaultColor);
				hScan += new Vector3 (xIncrement, 0, 0); 
			}
			vScan += new Vector3 (0, yIncrement, 0); 
		}
	}
	
	////////////////////////////////////////////////////////////////////////////////////////////////////
	public static void CalculateTangents (Mesh mesh)
	{
		Vector4[] tangents = new Vector4 [mesh.vertices.Length];
		Vector3[] tangents1 = new Vector3 [mesh.vertices.Length];
		Vector3[] tangents2 = new Vector3 [mesh.vertices.Length];
	
		for (int i = 0; i < mesh.triangles.Length; i += 3) {
			int i1 = mesh.triangles [i];
			int i2 = mesh.triangles [i + 1];
			int i3 = mesh.triangles [i + 2];
	
			Vector3 v1 = mesh.vertices [i1];
			Vector3 v2 = mesh.vertices [i2];
			Vector3 v3 = mesh.vertices [i3];
	
			Vector2 w1 = mesh.uv [i1];
			Vector2 w2 = mesh.uv [i2];
			Vector2 w3 = mesh.uv [i3];
	
			float x1 = v2.x - v1.x;
			float x2 = v3.x - v1.x;
	
			float y1 = v2.y - v1.y;
			float y2 = v3.y - v1.y;
	
			float z1 = v2.z - v1.z;
			float z2 = v3.z - v1.z;
	
			float s1 = w2.x - w1.x;
			float s2 = w3.x - w1.x;
	
			float t1 = w2.y - w1.y;
			float t2 = w3.y - w1.y;
	
			float r = 1.0f / (s1 * t2 - s2 * t1);
	
			Vector3 sdir = new Vector3 ((t2 * x1 - t1 * x2) * r, 
				           			   (t2 * y1 - t1 * y2) * r,
				           			   (t2 * z1 - t1 * z2) * r);
	
			Vector3 tdir = new Vector3 ((s1 * x2 - s2 * x1) * r, 
						   			   (s1 * y2 - s2 * y1) * r,
						   			   (s1 * z2 - s2 * z1) * r);
	
			tangents1 [i1] += sdir;
			tangents1 [i2] += sdir;
			tangents1 [i3] += sdir;
	
			tangents2 [i1] += tdir;
			tangents2 [i2] += tdir;
			tangents2 [i3] += tdir;
		}
	
		for (int i = 0; i < mesh.vertices.Length; i++) {
			Vector3 normal = mesh.normals [i];
			Vector3 tangent = tangents1 [i];
	
			// gram-schmidt orthogonalization
			Vector3 tmp = Vector3.Normalize (tangent - normal * Vector3.Dot (normal, tangent));
	
			// calculate handedness
			float w = (Vector3.Dot (Vector3.Cross (normal, tangent), tangents2 [i]) < 0.0f) ? -1.0f : 1.0f;
			
			tangents [i] = new Vector4 (tmp.x, tmp.y, tmp.z, w);
		}
		
		mesh.tangents = tangents;
	}
}