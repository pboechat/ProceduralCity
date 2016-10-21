using UnityEngine;
using System.Collections.Generic;
using Grid;

public abstract class BlocksExtractor : MonoBehaviour
{
	protected List<Block> _blocks;

	public List<Block> blocks {
		get {
			return this._blocks;
		}
	}

	public abstract void Execute (BaseGrid grid, ArchitectureStyle[] allArchitectureStyles, int[] architectureStylesMap);

}

