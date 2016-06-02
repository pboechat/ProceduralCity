using UnityEngine;
using Grid;

public abstract class RoadNetworkMeshGenerator : MonoBehaviour
{
	public abstract void Execute (RoadNetworkParameters roadNetworkParameters, BaseGrid grid);
	
}

