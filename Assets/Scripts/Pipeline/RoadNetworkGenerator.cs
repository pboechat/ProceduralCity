using UnityEngine;
using Grid;

public abstract class RoadNetworkGenerator : MonoBehaviour
{
	protected BaseGrid _grid;
	
	public BaseGrid grid {
		get {
			return this._grid;
		}
	}
	
	public abstract void Execute (RoadNetworkParameters roadNetworkParameters, float[] elevationMap, float[] populationMap, ArchitectureStyle[] architecturalStyleMap, Vector2[] destructionMap);
	
}
