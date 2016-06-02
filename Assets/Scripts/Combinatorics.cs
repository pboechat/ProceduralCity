using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Combinatorics
{
	private Combinatorics ()
	{
	}
	
	public static Rect[] PossibleAreas (int minWidth, int maxWidth, int minHeight, int maxHeight)
	{
		int[] widths = Enumerable.Range (minWidth, maxWidth - minWidth + 1).ToArray ();
		int[] heights = Enumerable.Range (minHeight, maxHeight - minHeight + 1).ToArray ();
		Rect[] combinations = new Rect[widths.Length * heights.Length];
		for (int i = 0; i < heights.Count(); i++) {
			for (int j = 0; j < widths.Length; j++) {
				combinations [i * widths.Length + j] = new Rect (0, 0, widths [j], heights [i]);
			}
		}
		return combinations;
	}
	
	public static Vector2[] PositionsInArea (int x, int y, int width, int height)
	{
		if (width <= 0 || height <= 0) {
			return null;
		}
		
		int[] widths = Enumerable.Range (x, width).ToArray ();
		int[] heights = Enumerable.Range (y, height).ToArray ();
		Vector2[] combinations = new Vector2[widths.Length * heights.Length];
		for (int i = 0; i < heights.Count(); i++) {
			for (int j = 0; j < widths.Length; j++) {
				combinations [i * widths.Length + j] = new Vector2 (widths [j], heights [i]);
			}
		}
		return combinations;
	}
	
}