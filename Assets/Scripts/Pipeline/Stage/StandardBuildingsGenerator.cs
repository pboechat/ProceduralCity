using UnityEngine;
using System.Collections.Generic;
using Pattern;

public class StandardBuildingsGenerator : BuildingsGenerator
{
	public override void Execute (List<Allotment> allotments)
	{
		_buildings = new List<Building> ();
		foreach (Allotment allotment in allotments) {
			ArchitectureStyle architectureStyle = allotment.architectureStyle;
				
			int heightInTiles = UnityEngine.Random.Range (architectureStyle.minHeight, architectureStyle.maxHeight + 1);
			int height = heightInTiles * architectureStyle.tileHeight;
			
			int widthInTiles = allotment.widthInTiles - architectureStyle.spacing;
			int width = widthInTiles * architectureStyle.tileWidth;
			int depthInTiles = allotment.depthInTiles - architectureStyle.spacing;
			int depth = depthInTiles * architectureStyle.tileWidth;
		
			Facade[] facades = new Facade[allotment.freeFaces.Length];
			int i = 0;
			foreach (Direction freeFace in allotment.freeFaces) {
				int longitude;
				int longitudeInTiles;
			
				if (freeFace == Direction.BACK || freeFace == Direction.FRONT) {
					longitude = width;
					longitudeInTiles = widthInTiles;
				} else {
					longitude = depth;
					longitudeInTiles = depthInTiles;
				}
			
				Pattern<FacadeItem> elementsPattern = architectureStyle.randomElementsPattern.Stretch (longitudeInTiles, heightInTiles);
				Pattern<FacadeItem> detailsPattern = architectureStyle.randomDetailsPattern.Stretch (longitudeInTiles, heightInTiles);
				Pattern<FacadeOperation> operationsPattern;
				if (architectureStyle.usesOperations) {
					operationsPattern = architectureStyle.randomOperationsPattern.Stretch (longitudeInTiles, heightInTiles);
				} else {
					operationsPattern = null;
				}
				PatternEvaluator.Evaluate (elementsPattern, detailsPattern, operationsPattern, architectureStyle);
			
				facades [i++] = new Facade (freeFace, longitude, height, longitudeInTiles, heightInTiles, architectureStyle, elementsPattern, detailsPattern, operationsPattern);
			}
		
			_buildings.Add (new Building (allotment.x, allotment.y, width, height, depth, widthInTiles, heightInTiles, depthInTiles, facades, architectureStyle));
		}
	}

}
