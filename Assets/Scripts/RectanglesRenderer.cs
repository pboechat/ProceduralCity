using UnityEngine;
using System.Collections.Generic;

public class RectanglesRenderer : MonoBehaviour
{
	private class RenderList
	{
		public Material material;
		public bool wireframe;
		public List<Rect> rectangles = new List<Rect> ();
		
		public RenderList (Material material, bool wireframe)
		{
			this.material = material;
			this.wireframe = wireframe;
		}
	}
	
	[SerializeField]
	private Material _outlineMaterial;
	private List<RenderList> _renderLists = new List<RenderList> ();
	
	public void Push (Material material, bool wireframe)
	{
		_renderLists.Add (new RenderList (material, wireframe));
	}

    public void Pop()
    {
        if (_renderLists.Count == 0)
            return;
        _renderLists.RemoveAt(_renderLists.Count - 1);
    }
	
	public void Clear ()
	{
		_renderLists.Clear ();
	}
	
	public void Add (Rect rectangle)
	{
		if (_renderLists.Count == 0) {
			return;
		}
		_renderLists [_renderLists.Count - 1].rectangles.Add (rectangle);
	}
	
	public void Remove (Rect rectangle)
	{
		if (_renderLists.Count == 0) {
			return;
		}
		_renderLists [_renderLists.Count - 1].rectangles.Remove (rectangle);
	}

	void DrawWireframe (Rect rectangle, Material material)
	{
		float xMin = rectangle.xMin;
		float yMin = rectangle.yMin;
		float xMax = rectangle.xMax;
		float yMax = rectangle.yMax;
		
		Vector2 p1 = new Vector2 (xMin, yMax);
		Vector2 p2 = new Vector2 (xMax, yMax);
		Vector2 p3 = new Vector2 (xMax, yMin);
		Vector2 p4 = new Vector2 (xMin, yMin);
		DrawUtils.DrawLine (p1, p2, material);
		DrawUtils.DrawLine (p2, p3, material);
		DrawUtils.DrawLine (p3, p4, material);
		DrawUtils.DrawLine (p4, p1, material);
	}

	void OnPostRender ()
	{
		if (_renderLists.Count == 0) {
			return;
		}
		
		for (int i = 0; i < _renderLists.Count; i++) {
			RenderList renderList = _renderLists [i];
			foreach (Rect rectangle in renderList.rectangles) {
				if (renderList.wireframe) {
					DrawWireframe (rectangle, renderList.material);
				} else {
					DrawUtils.DrawQuad (rectangle.center, rectangle.width, rectangle.height, new Rect (0, 0, 1, 1), renderList.material);
					if (_outlineMaterial != null) {
						DrawWireframe (rectangle, _outlineMaterial);
					}
				}
			}
		}
	}
	
}
