using UnityEngine;
using Grid;

public class IrregularRectangleGridRoadNetworkGenerator : RoadNetworkGenerator
{
	public override void Execute (RoadNetworkParameters roadNetworkParameters, float[] elevationMap, float[] populationMap, ArchitectureStyle[] architecturalStyleMap, Vector2[] destructionMap)
	{
		_grid = IrregularRectangleGrid.Generate (roadNetworkParameters.gridWidth, 
												 roadNetworkParameters.gridHeight, 
												 roadNetworkParameters.minCellWidth, 
												 roadNetworkParameters.maxCellWidth, 
												 roadNetworkParameters.minCellDepth, 
												 roadNetworkParameters.maxCellDepth,
												 roadNetworkParameters.roadWidth);
	}

}
