using UnityEngine;
using System.Collections.Generic;
using Grid;

public class ByPassBlocksExtractor : BlocksExtractor
{
    public override void Execute(BaseGrid grid, ArchitectureStyle[] allArchitectureStyles, int[] architectureStylesMap)
    {
        _blocks = new List<Block>();
        foreach (Cell[] row in grid)
            foreach (Cell cell in row)
                _blocks.Add(new Block(cell.center, cell.width, cell.height, new List<ArchitectureStyle>(allArchitectureStyles)));
    }

}
