using UnityEngine;
using System.Collections.Generic;
using Grid;

public class SingleSampleBlocksExtractor : BlocksExtractor
{
	public override void Execute (BaseGrid grid, ArchitectureStyle[] allArchitectureStyles, int[] architectureStylesMap)
	{
		_blocks = new List<Block> ();
		foreach (Cell[] row in grid) {
			foreach (Cell cell in row) {
				int i = (int)cell.center.y * (int)grid.bounds.size.x + (int)cell.center.x;
                i = Mathf.Max(0, Mathf.Min(i, architectureStylesMap.Length - 1));
                List<ArchitectureStyle> architectureStyles = new List<ArchitectureStyle> ();
				architectureStyles.Add (allArchitectureStyles[architectureStylesMap[i]]);
				_blocks.Add (new Block (cell.center, cell.width, cell.height, architectureStyles));
			}
		}
	}

}