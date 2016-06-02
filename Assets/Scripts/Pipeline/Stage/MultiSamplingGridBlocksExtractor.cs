using UnityEngine;
using System;
using System.Collections.Generic;
using Grid;

public class MultiSamplingGridBlocksExtractor : BlocksExtractor
{
	public override void Execute (BaseGrid grid, ArchitectureStyle[] architectureStyleMap)
	{
		_blocks = new List<Block> ();
		foreach (Cell[] row in grid) {
			foreach (Cell cell in row) {
				// TODO: create a sampling strategy
				int xMin = (int)cell.center.x - (cell.width / 2);
				int yMin = (int)cell.center.y - (cell.height / 2);
				
				List<ArchitectureStyle> architectureStylesSamples = new List<ArchitectureStyle> ();
				for (int y = 0; y < cell.height; y++) {
					for (int x = 0; x < cell.width; x++) {
						int i = ((yMin + y) * (int)grid.bounds.size.x) + (xMin + x);
                        if (i < 0 || i >= architectureStyleMap.Length)
                            throw new Exception("invalid architecture style map index (i: " + i + ", xMin: "+xMin+", yMin: "+yMin+", x: "+x+", y: "+y+ ", cell.center: [" + cell.center.x + ", " + cell.center.y + "], center.size: [" + cell.width + ", " + cell.height + "])");
						architectureStylesSamples.Add (architectureStyleMap [i]);
					}
				}
				
				_blocks.Add (new Block (cell.center, cell.width, cell.height, architectureStylesSamples));
			}
		}
	}

}
