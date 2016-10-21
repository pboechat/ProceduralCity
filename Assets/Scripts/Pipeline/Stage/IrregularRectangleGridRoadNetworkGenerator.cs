using UnityEngine;
using Grid;

public class IrregularRectangleGridRoadNetworkGenerator : RoadNetworkGenerator
{
	public override void Execute (RoadNetworkParameters roadNetworkParameters, float[] elevationMap, float[] populationMap, ArchitectureStyle[] allAchitectureStyles, int[] architecturalStylesMap)
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
