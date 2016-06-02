using UnityEngine;

namespace Grid
{
	public struct Cell
	{
		public int x;
		public int y;
		public Vector2 center;
		public int width;
		public int height;
		
		public Cell (int x, int y, Vector2 center, int width, int height)
		{
			this.x = x;
			this.y = y;
			this.center = center;
			this.width = width;
			this.height = height;
		}
	}
	
}