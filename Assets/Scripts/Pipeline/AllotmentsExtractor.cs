using UnityEngine;
using System.Collections.Generic;
using Grid;

public abstract class AllotmentsExtractor : MonoBehaviour
{
	protected List<Allotment> _allotments;

	public List<Allotment> allotments {
		get {
			return this._allotments;
		}
	}

	public abstract void Execute (BaseGrid grid, ArchitectureStyle[] allArchitectureStyles, int[] architectureStylesMap, List<Block> blocks);

}


