using UnityEngine;
using System.Collections.Generic;
using Pattern;

public class BuildingMeshGeneratorExecutor : MonoBehaviour
{
	[SerializeField]
	private BuildingsMeshGenerator _buildingsMeshGenerator;
	[SerializeField]
	private ArchitectureStyle _architectureStyle;
	[SerializeField]
	private int _seed;
	[SerializeField]
	private Camera _camera;
	
	void Start ()
	{
		UnityEngine.Random.seed = _seed;
		
		int widthInTiles = UnityEngine.Random.Range (_architectureStyle.minWidth, _architectureStyle.maxWidth);
		int heightInTiles = UnityEngine.Random.Range (_architectureStyle.minHeight, _architectureStyle.maxHeight);
		int depthInTiles = UnityEngine.Random.Range (_architectureStyle.minDepth, _architectureStyle.maxDepth);
		int tileWidth = _architectureStyle.tileWidth;
		int tileHeight = _architectureStyle.tileHeight;
		
		int width = widthInTiles * tileWidth;
		int height = heightInTiles * tileHeight;
		int depth = depthInTiles * tileWidth;
		
		Pattern<FacadeItem> elementsPattern = _architectureStyle.randomElementsPattern.Stretch (widthInTiles, heightInTiles);
		Pattern<FacadeItem> detailsPattern = _architectureStyle.randomDetailsPattern.Stretch (widthInTiles, heightInTiles);
		Pattern<FacadeOperation> operationsPattern;
		if (_architectureStyle.usesOperations) {
			operationsPattern = _architectureStyle.randomOperationsPattern.Stretch (widthInTiles, heightInTiles);
		} else {
			operationsPattern = null;
		}
		PatternEvaluator.Evaluate (elementsPattern, detailsPattern, operationsPattern, _architectureStyle);
		
		List<Facade> facades = new List<Facade> ();
		
		Facade facade = new Facade (Direction.FRONT, width, height, widthInTiles, heightInTiles, _architectureStyle, elementsPattern, detailsPattern, operationsPattern);
		facades.Add (facade);
		
		facade = new Facade (Direction.BACK, width, height, widthInTiles, heightInTiles, _architectureStyle, elementsPattern, detailsPattern, operationsPattern);
		facades.Add (facade);
		
		elementsPattern = _architectureStyle.randomElementsPattern.Stretch (depthInTiles, heightInTiles);
		detailsPattern = _architectureStyle.randomDetailsPattern.Stretch (depthInTiles, heightInTiles);
		if (_architectureStyle.usesOperations) {
			operationsPattern = _architectureStyle.randomOperationsPattern.Stretch (depthInTiles, heightInTiles);
		} else {
			operationsPattern = null;
		}
		PatternEvaluator.Evaluate (elementsPattern, detailsPattern, operationsPattern, _architectureStyle);
		
		facade = new Facade (Direction.LEFT, depth, height, depthInTiles, heightInTiles, _architectureStyle, elementsPattern, detailsPattern, operationsPattern);
		facades.Add (facade);
		
		facade = new Facade (Direction.RIGHT, depth, height, depthInTiles, heightInTiles, _architectureStyle, elementsPattern, detailsPattern, operationsPattern);
		facades.Add (facade);
		
		Building building = new Building (0, 0, width, height, depth, widthInTiles, heightInTiles, depthInTiles, facades.ToArray(), _architectureStyle);
		
		List<Building> buildings = new List<Building> ();
		buildings.Add (building);
		
		_buildingsMeshGenerator.Execute (buildings);
		
		float distance = height / (2.0f * _camera.near * Mathf.Tan (Mathf.Deg2Rad * _camera.fov / 2.0f));
		_camera.transform.position = new Vector3 (width / 2.0f, height / 2.0f, -distance);
	}
}
