using UnityEngine;
using System.Collections.Generic;

namespace Grid
{
	public class IrregularRectangleGrid : BaseGrid
	{
		private IrregularRectangleGrid (int width, int height, Cell[][] cells, int padding) : base(width, height, cells, padding)
		{
			// private
		}
	
		public static IrregularRectangleGrid Generate (int width, int height, int minCellWidth, int maxCellWidth, int minCellHeight, int maxCellHeight, int padding = 0)
		{
			Cell[][] cells = new Cell[height][];
			
			int worstCaseGridWidth = (maxCellWidth + padding) * width + padding;
			float worstCaseGridHalfWidth = worstCaseGridWidth / 2.0f;
			int worstCaseGridHeight = (maxCellHeight + padding) * height + padding;
			float worstCaseGridHalfHeight = worstCaseGridHeight / 2.0f;
			
			int halfHeight = (height + 1) / 2;
			int halfWidth = (width + 1) / 2;
			
			float yOffset = worstCaseGridHalfHeight + (float)padding;
			for (int y = halfHeight - 1; y >= 0; y--) {
				cells [y] = new Cell[width];
				
				int cellHeight = Random.Range (minCellHeight, maxCellHeight + 1);
				float halfCellHeight = cellHeight / 2.0f;
				
				float xOffset = worstCaseGridHalfWidth;
				for (int x = halfWidth - 1; x >= 0; x--) {
					int cellWidth = Random.Range (minCellWidth, maxCellWidth + 1);
					float halfCellWidth = cellWidth / 2.0f;
					cells [y] [x] = new Cell (x, y, new Vector2 (xOffset, yOffset) - new Vector2 (halfCellWidth, halfCellHeight), cellWidth, cellHeight);
					xOffset -= (cellWidth + padding);
				}
				
				xOffset = worstCaseGridHalfWidth + (float)padding;
				for (int x = halfWidth; x < width; x++) {
					int cellWidth = Random.Range (minCellWidth, maxCellWidth + 1);
					float halfCellWidth = cellWidth / 2.0f;
					cells [y] [x] = new Cell (x, y, new Vector2 (xOffset, yOffset) + new Vector2 (halfCellWidth, -halfCellHeight), cellWidth, cellHeight);
					xOffset += (float)(cellWidth + padding);
				}
				
				yOffset -= (cellHeight + padding);
			}
			
			yOffset = worstCaseGridHalfHeight + 2.0f * padding;
			for (int y = halfHeight; y < height; y++) {
				cells [y] = new Cell[width];
				
				int cellHeight = Random.Range (minCellHeight, maxCellHeight + 1);
				float halfCellHeight = cellHeight / 2.0f;
				
				float xOffset = worstCaseGridHalfWidth;
				for (int x = halfWidth - 1; x >= 0; x--) {
					int cellWidth = Random.Range (minCellWidth, maxCellWidth + 1);
					float halfCellWidth = cellWidth / 2.0f;
					cells [y] [x] = new Cell (x, y, new Vector2 (xOffset, yOffset) + new Vector2 (-halfCellWidth, halfCellHeight), cellWidth, cellHeight);
					xOffset -= (cellWidth + padding);
				}
				
				xOffset = worstCaseGridHalfWidth + (float)padding;
				for (int x = halfWidth; x < width; x++) {
					int cellWidth = Random.Range (minCellWidth, maxCellWidth + 1);
					float halfCellWidth = cellWidth / 2.0f;
					cells [y] [x] = new Cell (x, y, new Vector2 (xOffset, yOffset) + new Vector2 (halfCellWidth, halfCellHeight), cellWidth, cellHeight);
					xOffset += (float)(cellWidth + padding);
				}
				
				yOffset += (cellHeight + padding);
			}
			
			IrregularRectangleGrid grid = new IrregularRectangleGrid (width, height, cells, padding);
			Vector3 size = new Vector3 ((maxCellWidth + padding) * width + padding, (maxCellHeight + padding) * height + padding, 0.0f);
			Vector3 center = size / 2.0f;
			grid._bounds = new Bounds (center, size);
			return grid;
		}
	
	}
	
}