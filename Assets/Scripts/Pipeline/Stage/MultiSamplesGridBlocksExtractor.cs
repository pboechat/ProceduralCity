using UnityEngine;
using System.Collections.Generic;
using Grid;

public class MultiSamplesGridBlocksExtractor : BlocksExtractor
{
    public override void Execute (BaseGrid grid, ArchitectureStyle[] allArchitectureStyles, int[] architectureStylesMap)
	{
        _blocks = new List<Block> ();
		foreach (Cell[] row in grid) {
			foreach (Cell cell in row) {
				int xMin = (int)cell.center.x - (cell.width / 2);
				int yMin = (int)cell.center.y - (cell.height / 2);
				List<ArchitectureStyle> architectureStylesSamples = new List<ArchitectureStyle> ();
				for (int y = 0; y < cell.height; y++) {
					for (int x = 0; x < cell.width; x++) {
						int i = ((yMin + y) * (int)grid.bounds.size.x) + (xMin + x);
                        i = Mathf.Max(0, Mathf.Min(i, architectureStylesMap.Length - 1));
						architectureStylesSamples.Add (allArchitectureStyles[architectureStylesMap[i]]);
					}
				}
				_blocks.Add (new Block (cell.center, cell.width, cell.height, architectureStylesSamples));
			}
		}
	}

}
