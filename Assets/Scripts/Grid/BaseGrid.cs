using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Grid
{
	public abstract class BaseGrid : IEnumerable<Cell[]>, IEnumerator<Cell[]>
	{
		private int _current;
		protected Bounds _bounds;
		protected int _width;
		protected int _height;
		protected Cell[][] _cells;
		protected int _padding;
	
		protected BaseGrid (int width, int height, Cell[][] cells, int padding = 0)
		{
			_current = -1;
			_width = width;
			_height = height;
			_cells = cells;
			_padding = padding;
		}
		
		public Bounds bounds {
			get {
				return _bounds;
			}
		}
		
		public Cell[] this [int i] {
			get {
				return _cells [i];
			}
		}
	
		public int width {
			get {
				return _width;
			}
		}
	
		public int height {
			get {
				return _height;
			}
		}
	
		public int size {
			get {
				return _width * _height;
			}
		}
		
		protected void CalculateBoundsFromCells ()
		{
			Vector2 min = new Vector2 (10000, 10000), max = new Vector2 (-10000, -10000);
			for (int y = 0; y < height; y++) {
				for (int x = 0; x < width; x++) {
					Vector2 cellMin, cellMax;
					Cell cell = _cells [y] [x];
					float halfWidth = cell.width / 2.0f;
					float halfHeight = cell.height / 2.0f;
					
					cellMin = cell.center - new Vector2 (halfWidth, halfHeight);
					cellMax = cell.center + new Vector2 (halfWidth, halfHeight);
					
					min.x = Mathf.Min (min.x, cellMin.x);
					min.y = Mathf.Min (min.y, cellMin.y);
					max.x = Mathf.Max (max.x, cellMax.x);
					max.y = Mathf.Max (max.y, cellMax.y);
				}
			}
			
			float margin = 2.0f * _padding;
			Vector2 size = (max - min) + new Vector2 (margin, margin);
			Vector2 center = min + (size / 2.0f) - new Vector2 (_padding, _padding);
			
			_bounds = new Bounds (new Vector3 (center.x, center.y, 0.0f), new Vector3 (size.x, size.y, 0.0f));
		}
		
		public Cell[] Current {
			get {
				return _cells [_current];
			}
		}

		public bool MoveNext ()
		{
			if (_current < _height - 1) {
				_current++;
				return true;
			} else {
				return false;
			}
		}

		public void Reset ()
		{
			_current = -1;
		}

		object IEnumerator.Current {
			get {
				return _cells [_current];
			}
		}

		public void Dispose ()
		{
			_current = -1;
		}

		IEnumerator<Cell[]> IEnumerable<Cell[]>.GetEnumerator ()
		{
			return this;
		}

		IEnumerator IEnumerable.GetEnumerator ()
		{
			return this;
		}
	}
	
}