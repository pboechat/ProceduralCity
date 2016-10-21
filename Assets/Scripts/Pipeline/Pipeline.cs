using UnityEngine;
using System;

public class Pipeline : MonoBehaviour
{
	[SerializeField]
	private CityMapsGenerator _cityMapsGenerator;
	[SerializeField]
	private TerrainMeshGenerator _terrainMeshGenerator;
	[SerializeField]
	private RoadNetworkGenerator _roadNetworkGenerator;
	[SerializeField]
	private RoadNetworkMeshGenerator _roadNetworkMeshGenerator;
	[SerializeField]
	private BlocksExtractor _blocksExtractor;
	[SerializeField]
	private AllotmentsExtractor _allotmentsExtractor;
	[SerializeField]
	private BuildingsGenerator _buildingsGenerator;
	[SerializeField]
	private BuildingsMeshGenerator _buildingsMeshGenerator;
	
	public void Run (RoadNetworkParameters roadNetworkParameters, ArchitectureStyle[] allArchitectureStyles)
	{
		if (_cityMapsGenerator == null) {
			throw new Exception ("_cityMapsGenerator == null");
		}
		
		if (_terrainMeshGenerator == null) {
			throw new Exception ("_terrainMeshGenerator == null");
		}
		
		if (_roadNetworkGenerator == null) {
			throw new Exception ("_roadNetworkGenerator == null");
		}
		
		if (_roadNetworkMeshGenerator == null) {
			throw new Exception ("_roadNetworkMeshGenerator == null");
		}
		
		if (_blocksExtractor == null) {
			throw new Exception ("_blocksExtractor == null");
		}
		
		if (_allotmentsExtractor == null) {
			throw new Exception ("_allotmentsExtractor == null");
		}
		
		if (_buildingsGenerator == null) {
			throw new Exception ("_buildingsGenerator == null");
		}
		
		if (_buildingsMeshGenerator == null) {
			throw new Exception ("_buildingsMeshGenerator == null");
		}
		
		// ---
		
		// pipeline
		_cityMapsGenerator.Execute (roadNetworkParameters, allArchitectureStyles);
		{
			_terrainMeshGenerator.Execute (roadNetworkParameters, _cityMapsGenerator.elevationMap);
			_roadNetworkGenerator.Execute (roadNetworkParameters, _cityMapsGenerator.elevationMap, _cityMapsGenerator.populationMap, allArchitectureStyles, _cityMapsGenerator.architecturalStylesMap);
		} // parallel
		{
			_roadNetworkMeshGenerator.Execute (roadNetworkParameters, _roadNetworkGenerator.grid);
			_blocksExtractor.Execute (_roadNetworkGenerator.grid, allArchitectureStyles, _cityMapsGenerator.architecturalStylesMap);
		} // parallel
		_allotmentsExtractor.Execute (_roadNetworkGenerator.grid, allArchitectureStyles, _cityMapsGenerator.architecturalStylesMap, _blocksExtractor.blocks);
		_buildingsGenerator.Execute (_allotmentsExtractor.allotments);
		_buildingsMeshGenerator.Execute (_buildingsGenerator.buildings);
	}
	
}
