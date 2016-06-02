using UnityEngine;
using System;
using ModelRepository;

namespace Pattern
{
	public class Pattern<T> where T : IPatternItem, new()
	{
		private int _width;
		private int _height;
		private T[][] _items;
		private ArchitectureStyle _architectureStyle;

		public Pattern (int width, int height, ArchitectureStyle architectureStyle)
		{
			_width = width;
			_height = height;
			_items = new T[_height][];
			_architectureStyle = architectureStyle;
		
			for (int i = 0; i < _height; i++) {
				_items [i] = new T[_width];
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
		
		public T[][] ToMatrix ()
		{
			return _items;
		}
		
		public void SetElement (int x, int y, char symbol, int index)
		{
			T item = new T ();
			item.Initialize (symbol, index, _architectureStyle);
			_items [y] [x] = item;
		}
		
		public T GetElement (int x, int y)
		{
			return _items [y] [x];
		}
	
		public virtual Pattern<T> Stretch (int width, int height)
		{
			Pattern<T> otherPattern = new Pattern<T> (width, height, _architectureStyle);
			
			float u = _width / (float)width;
			float v = _height / (float)height;
			for (int y1 = 0; y1 < height; y1++) {
				for (int x1 = 0; x1 < width; x1++) {
					int y2 = Mathf.Min (Mathf.FloorToInt (y1 * v), _height);
					int x2 = Mathf.Min (Mathf.FloorToInt (x1 * u), _width);
					otherPattern._items [y1] [x1] = (T)_items [y2] [x2].Clone ();
				}
			}
			
			return otherPattern;
		}
	}
	
}