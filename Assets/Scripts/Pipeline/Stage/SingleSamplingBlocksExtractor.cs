using UnityEngine;
using System.Collections.Generic;
using Grid;

public class SingleSamplingBlocksExtractor : BlocksExtractor
{
	public override void Execute (BaseGrid grid, ArchitectureStyle[] architectureStyleMap)
	{
		_blocks = new List<Block> ();
		foreach (Cell[] row in grid) {
			foreach (Cell cell in row) {
				// TODO: create a sampling strategy
				int i = (int)cell.center.y * (int)grid.bounds.size.x + (int)cell.center.x;
			
				List<ArchitectureStyle> architectureStyles = new List<ArchitectureStyle> ();
				architectureStyles.Add (architectureStyleMap [i]);
				
				_blocks.Add (new Block (cell.center, cell.width, cell.height, architectureStyles));
			}
		}
	}

}