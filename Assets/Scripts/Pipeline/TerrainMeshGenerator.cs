using UnityEngine;

public abstract class TerrainMeshGenerator : MonoBehaviour
{
	public abstract void Execute (RoadNetworkParameters roadNetworkParameters, float[] elevationMap);
	
}
