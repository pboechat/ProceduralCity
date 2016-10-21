using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using Grid;

public class SumSetAllotmentsExtractor : AllotmentsExtractor
{
	public override void Execute (BaseGrid grid, ArchitectureStyle[] allArchitectureStyles, int[] architectureStylesMap, List<Block> blocks)
	{
		_allotments = new List<Allotment> ();
		foreach (Block block in blocks) {
			ArchitectureStyle architectureStyle = block.randomArchitectureStyle;
			
			int blockWidthInTiles = block.width / architectureStyle.tileWidth;
			int blockDepthInTiles = block.depth / architectureStyle.tileWidth;

			int[][] possibleWidths = SubsetSum.Combinations (Enumerable.Range (architectureStyle.minWidth + architectureStyle.spacing, architectureStyle.maxWidth + architectureStyle.spacing).ToArray (), blockWidthInTiles);
			int[][] possibleDepths = SubsetSum.Combinations (Enumerable.Range (architectureStyle.minDepth + architectureStyle.spacing, architectureStyle.maxDepth + architectureStyle.spacing).ToArray (), blockDepthInTiles);
			
			int[] widths = possibleWidths [UnityEngine.Random.Range (0, possibleWidths.Length)];
			int[] depths = possibleDepths [UnityEngine.Random.Range (0, possibleDepths.Length)];
			
			int startX = (int)block.center.x - block.width / 2;
			int startZ = (int)block.center.y - block.depth / 2;
			
			int zOffset = 0;
			for (int z2 = 0; z2 < depths.Length; z2++) {
				int xOffset = 0;
				int depthInTiles = depths [z2];
				int depth = depthInTiles * architectureStyle.tileWidth;
				for (int x2 = 0; x2 < widths.Length; x2++) {
					int widthInTiles = widths [x2];
					int width = widthInTiles * architectureStyle.tileWidth;
			
					if ((z2 == 0 || z2 == depths.Length - 1) || (x2 == 0 || x2 == widths.Length - 1)) {
						HashSet<Direction> freeFaces = new HashSet<Direction> ();
				
						if (z2 == 0) {
							freeFaces.Add (Direction.FRONT);
						} else if (z2 == depths.Length - 1) {
							freeFaces.Add (Direction.BACK);
						}
				
						if (x2 == 0) {
							freeFaces.Add (Direction.LEFT);
						} else if (x2 == widths.Length - 1) {
							freeFaces.Add (Direction.RIGHT);
						}
				
						_allotments.Add (new Allotment (startX + xOffset, startZ + zOffset, width, depth, widthInTiles, depthInTiles, freeFaces.ToArray(), architectureStyle));
					}
			
					xOffset += width;
				}
				zOffset += depth;
			}
		}
	}

}