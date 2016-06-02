using UnityEngine;
using System.Collections.Generic;

public abstract class BuildingsMeshGenerator : MonoBehaviour
{
	public abstract void Execute (List<Building> buildings);
	
}
