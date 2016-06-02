using UnityEngine;
using Grid;

public class RegularRectangleGridRoadNetworkGenerator : RoadNetworkGenerator
{
	public override void Execute (RoadNetworkParameters roadNetworkParameters, float[] elevationMap, float[] populationMap, ArchitectureStyle[] architecturalStyleMap, Vector2[] destructionMap)
	{
		int averageCellWidth = (roadNetworkParameters.maxCellWidth + roadNetworkParameters.minCellWidth) / 2;
		int averageCellDepth = (roadNetworkParameters.maxCellDepth + roadNetworkParameters.minCellDepth) / 2;
		
		_grid = RegularRectangleGrid.Generate (roadNetworkParameters.gridWidth, 
											   roadNetworkParameters.gridHeight, 
										 	   averageCellWidth, 
											   averageCellDepth, 
											   roadNetworkParameters.roadWidth);
	}

}

