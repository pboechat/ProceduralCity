using UnityEngine;
using System.Collections.Generic;

namespace Grid
{
	public class RegularRectangleGrid : BaseGrid
	{
		private RegularRectangleGrid (int width, int height, Cell[][] cells, int padding) : base(width, height, cells, padding)
		{
			// private
		}
	
		public static RegularRectangleGrid Generate (int width, int height, int cellWidth, int cellHeight, int padding = 0)
		{
			Cell[][] cells = new Cell[height][];
			
			float halfWidth = cellWidth / 2.0f;
			float halfHeight = cellHeight / 2.0f;
			
			float yOffset = (float)padding;
			for (int y = 0; y < height; y++) {
				cells [y] = new Cell[width];
				float xOffset = (float)padding;
				for (int x = 0; x < width; x++) {
					cells [y] [x] = new Cell (x, y, new Vector2 (xOffset, yOffset) + new Vector2 (halfWidth, halfHeight), cellWidth, cellHeight);
					xOffset += cellWidth + padding;
				}
				yOffset += (cellHeight + padding);
			}
			
			RegularRectangleGrid grid = new RegularRectangleGrid (width, height, cells, padding);
			grid.CalculateBoundsFromCells ();
			return grid;
		}
	
	}
	
}
