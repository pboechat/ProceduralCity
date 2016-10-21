using UnityEngine;
using System;
using System.Collections.Generic;
using Grid;
using BinPack;

public class MaxRectsPackAllotmentsExtractor : AllotmentsExtractor
{
	[SerializeField]
	private float _minOccupancyRate;
	[SerializeField]
	private bool _abortInError;
	[SerializeField]
	private bool _tryToFitSmallerAllotmentsFirst = false;
	[SerializeField]
	private MaxRects.FreeRectChoiceHeuristic _allotmentFittingHeuristics = MaxRects.FreeRectChoiceHeuristic.RectContactPointRule;

	static ArchitectureStyle FindSuitableArchitectureStyle (Rect area, ICollection<ArchitectureStyle> architectureStyles)
	{
		List<ArchitectureStyle> suitableArchitectureStyles = new List<ArchitectureStyle> ();
		foreach (ArchitectureStyle architectureStyle in architectureStyles) {
			int tileWidth = architectureStyle.tileWidth;
			int minWidth = (architectureStyle.minWidth + architectureStyle.spacing) * tileWidth;
			int maxWidth = (architectureStyle.maxWidth + architectureStyle.spacing) * tileWidth;
			int minDepth = (architectureStyle.minDepth + architectureStyle.spacing) * tileWidth;
			int maxDepth = (architectureStyle.maxDepth + architectureStyle.spacing) * tileWidth;
			
			int width = (int)area.width;
			int depth = (int)area.height;
			
			if ((width >= minWidth && width <= maxWidth && depth >= minDepth && depth <= maxDepth) ||
				(depth >= minWidth && depth <= maxWidth && width >= minDepth && width <= maxDepth)) {
				suitableArchitectureStyles.Add (architectureStyle);
			}
		}

		return suitableArchitectureStyles [UnityEngine.Random.Range (0, suitableArchitectureStyles.Count)];
	}
	
	public override void Execute (BaseGrid grid, ArchitectureStyle[] allArchitectureStyles, int[] architectureStylesMap, List<Block> blocks)
	{
		_allotments = new List<Allotment> ();
		foreach (Block block in blocks) {
			List<Rect> allPossibleAreas = new List<Rect> ();
			foreach (ArchitectureStyle possibleArchitectureStyle in block.architectureStyles) {
				int tileWidth = possibleArchitectureStyle.tileWidth;
				Rect[] possibleAreas = Combinatorics.PossibleAreas (possibleArchitectureStyle.minWidth + possibleArchitectureStyle.spacing, possibleArchitectureStyle.maxWidth + possibleArchitectureStyle.spacing, possibleArchitectureStyle.minDepth + possibleArchitectureStyle.spacing, possibleArchitectureStyle.maxDepth + possibleArchitectureStyle.spacing);
				foreach (Rect possibleArea in possibleAreas) {
					Rect area = new Rect (0, 0, possibleArea.width * tileWidth, possibleArea.height * tileWidth);
					if (!allPossibleAreas.Contains (area)) {
						allPossibleAreas.Add (area);
					}
				}
			}
			
			List<Rect> allotmentAreas = new List<Rect> ();
			MaxRects maxRects = new MaxRects (block.width, block.depth);
			
			if (_tryToFitSmallerAllotmentsFirst) {
				// sort possible allotment areas by rectangle area
				allPossibleAreas.Sort (delegate(Rect r1, Rect r2)
				{
					float a1 = (r1.width * r1.height);
					float a2 = (r2.width * r2.height);
					return a1.CompareTo (a2);
				});
			}
			
			while (allPossibleAreas.Count > 0) {
				Rect possibleArea;
				if (_tryToFitSmallerAllotmentsFirst) {
					possibleArea = allPossibleAreas [0];
				} else {
					possibleArea = allPossibleAreas [UnityEngine.Random.Range (0, allPossibleAreas.Count)];
				}
				
				Rect allotmentArea = maxRects.Insert ((int)possibleArea.width, (int)possibleArea.height, _allotmentFittingHeuristics);
				
				if (allotmentArea.width == 0 || allotmentArea.height == 0) {
					allPossibleAreas.Remove (possibleArea);
					continue;
				}
				
				allotmentAreas.Add (new Rect (allotmentArea.x, allotmentArea.y, allotmentArea.width, allotmentArea.height));
			}
			
			if (maxRects.Occupancy () < _minOccupancyRate) {
				string message = "block occupancy is less than minimum: " + maxRects.Occupancy ();
				if (_abortInError) {
					throw new Exception (message);
				} else {
					Debug.LogWarning (message);
				}
			}
			
			int blockXStart = (int)block.center.x - block.width / 2;
			int blockZStart = (int)block.center.y - block.depth / 2;
			foreach (Rect allotmentArea in allotmentAreas) {
				ArchitectureStyle architectureStyle = FindSuitableArchitectureStyle (allotmentArea, block.architectureStyles);
				
				bool hasLeftNeighbour = false;
				bool hasRightNeighbour = false;
				bool hasUpperNeighbour = false;
				bool hasLowerNeighbour = false;
				foreach (Rect neighbourAllotmentArea in allotmentAreas) {
					if (neighbourAllotmentArea == allotmentArea) {
						continue;
					}
	
					if (neighbourAllotmentArea.xMax <= allotmentArea.xMin && 
						((neighbourAllotmentArea.yMin >= allotmentArea.yMin && neighbourAllotmentArea.yMin <= allotmentArea.yMax) ||
						 (neighbourAllotmentArea.yMax >= allotmentArea.yMin && neighbourAllotmentArea.yMax <= allotmentArea.yMax) ||
						 (allotmentArea.yMin >= neighbourAllotmentArea.yMin && allotmentArea.yMin <= neighbourAllotmentArea.yMax) ||
						 (allotmentArea.yMax >= neighbourAllotmentArea.yMin && allotmentArea.yMax <= neighbourAllotmentArea.yMax))) {
						hasLeftNeighbour = true;
					} else if (neighbourAllotmentArea.xMin >= allotmentArea.xMax && 
						((neighbourAllotmentArea.yMin >= allotmentArea.yMin && neighbourAllotmentArea.yMin <= allotmentArea.yMax) ||
						 (neighbourAllotmentArea.yMax >= allotmentArea.yMin && neighbourAllotmentArea.yMax <= allotmentArea.yMax) ||
						 (allotmentArea.yMin >= neighbourAllotmentArea.yMin && allotmentArea.yMin <= neighbourAllotmentArea.yMax) ||
						 (allotmentArea.yMax >= neighbourAllotmentArea.yMin && allotmentArea.yMax <= neighbourAllotmentArea.yMax))) {
						hasRightNeighbour = true;
					} else if (neighbourAllotmentArea.yMin >= allotmentArea.yMax && 
						((neighbourAllotmentArea.xMin >= allotmentArea.xMin && neighbourAllotmentArea.xMin <= allotmentArea.xMax) ||
						 (neighbourAllotmentArea.xMax >= allotmentArea.xMin && neighbourAllotmentArea.xMax <= allotmentArea.xMax) ||
						 (allotmentArea.xMin >= neighbourAllotmentArea.xMin && allotmentArea.xMin <= neighbourAllotmentArea.xMax) ||
						 (allotmentArea.xMax >= neighbourAllotmentArea.xMin && allotmentArea.xMax <= neighbourAllotmentArea.xMax))) {
						hasUpperNeighbour = true;
					} else if (neighbourAllotmentArea.yMax <= allotmentArea.yMin && 
						((neighbourAllotmentArea.xMin >= allotmentArea.xMin && neighbourAllotmentArea.xMin <= allotmentArea.xMax) ||
						 (neighbourAllotmentArea.xMax >= allotmentArea.xMin && neighbourAllotmentArea.xMax <= allotmentArea.xMax) ||
						 (allotmentArea.xMin >= neighbourAllotmentArea.xMin && allotmentArea.xMin <= neighbourAllotmentArea.xMax) ||
						 (allotmentArea.xMax >= neighbourAllotmentArea.xMin && allotmentArea.xMax <= neighbourAllotmentArea.xMax))) {
						hasLowerNeighbour = true;
					}
				}
				
				if (hasLeftNeighbour && hasRightNeighbour && hasUpperNeighbour && hasLowerNeighbour) {
					continue;
				}
				
				List<Direction> freeFaces = new List<Direction> ();
				
				if (!hasLowerNeighbour) {
					freeFaces.Add (Direction.FRONT);
				} 
				
				if (!hasUpperNeighbour) {
					freeFaces.Add (Direction.BACK);
				}
			
				if (!hasLeftNeighbour) {
					freeFaces.Add (Direction.LEFT);
				} 
				
				if (!hasRightNeighbour) {
					freeFaces.Add (Direction.RIGHT);
				}
			
				int widthInTiles = (int)allotmentArea.width / architectureStyle.tileWidth;
				int depthInTiles = (int)allotmentArea.height / architectureStyle.tileWidth;
				
				int spacing = architectureStyle.spacing * (architectureStyle.tileWidth / 2);
				
				_allotments.Add (new Allotment (blockXStart + (int)allotmentArea.x + spacing, blockZStart + (int)allotmentArea.y + spacing, (int)allotmentArea.width, (int)allotmentArea.height, widthInTiles, depthInTiles, freeFaces.ToArray (), architectureStyle));
			}
		}
	}

}