using UnityEngine;
using System;
using ModelRepository;

namespace Pattern
{
	public class PatternEvaluator
	{
		public static void Evaluate (Pattern<FacadeItem> elementsPattern, Pattern<FacadeItem> detailsPattern, Pattern<FacadeOperation> operationsPattern, ArchitectureStyle architectureStyle)
		{
			if (elementsPattern.width != detailsPattern.width || (operationsPattern != null && elementsPattern.width != operationsPattern.width) ||
				elementsPattern.height != detailsPattern.height || (operationsPattern != null && elementsPattern.height != operationsPattern.height)) {
				throw new Exception ("patterns do not match in size");
			}
						
			int width = elementsPattern.width;
			int height = elementsPattern.height;
			for (int y = 0; y < height; y++) {
				IModel previousElementModel = null;
				int x2 = 0;
				for (int x1 = 0; x1 < width; x1++) {
					FacadeItem element = elementsPattern.GetElement (x1, y);
					
					if (element.invalid) {
						continue;
					}
					
					if (element.model != previousElementModel) {
						x2 = x1;
					}
					
					previousElementModel = element.model;
					
					if (architectureStyle.groundFloor == GroundFloor.FIRST_FLOOR_FOOTER && y == height - 1) {
						element.BottomVariant ();
					} else if (/*architectureStyle.header &&*/ y == 0) {
						element.TopVariant ();
					}
					
					if (element.size > 1) {
						// pattern occurring outside its frequency (i.e.: occurrence of a 2 sized pattern in an odd frequency)
						if ((x1 - x2) % element.size != 0) {
							element.Invalidate ();
							detailsPattern.SetElement (x1, y, 'e', 0);
							continue;
						}
						
						// pattern overflowing other patterns
						bool substituted = false;
						for (int i = 1; i < element.size && (i + x1) < width; i++) {
							FacadeItem neighbourElement = elementsPattern.GetElement (x1 + i, y);
							// cannot compare neighbour to possibly "transformed" element, so compare to previous element model
							if (neighbourElement.model != previousElementModel) {
								elementsPattern.SetElement (x1, y, architectureStyle.defaultFacadeElementSymbol, architectureStyle.defaultFacadeElementIndex);
								detailsPattern.SetElement (x1, y, architectureStyle.defaultFacadeDetailSymbol, architectureStyle.defaultFacadeDetailIndex);
								substituted = true;
								break;
							}
						}
						
						if (substituted) {
							for (int i = 1; i < element.size && (i + x1) < width; i++) {
								elementsPattern.SetElement (x1 + i, y, architectureStyle.defaultFacadeElementSymbol, architectureStyle.defaultFacadeElementIndex);
								detailsPattern.SetElement (x1 + i, y, architectureStyle.defaultFacadeDetailSymbol, architectureStyle.defaultFacadeDetailIndex);
							}
							
							continue;
						}
					}
					
					// facade item overflowing the facade
					if (x1 + element.size > width) {
						elementsPattern.SetElement (x1, y, architectureStyle.defaultFacadeElementSymbol, architectureStyle.defaultFacadeElementIndex);
						detailsPattern.SetElement (x1, y, architectureStyle.defaultFacadeDetailSymbol, architectureStyle.defaultFacadeDetailIndex);
						continue;
					}
					
					if (operationsPattern != null) {
						// facade item spreading among different elevation levels
						FacadeOperation operation = operationsPattern.GetElement (x1, y);
						bool substituted = false;
						for (int i = 1; i < element.size && (i + x1) < width; i++) {
							FacadeOperation neighbourOperation = operationsPattern.GetElement (x1 + i, y);
							if (neighbourOperation != operation) {
								elementsPattern.SetElement (x1, y, architectureStyle.defaultFacadeElementSymbol, architectureStyle.defaultFacadeElementIndex);
								detailsPattern.SetElement (x1, y, architectureStyle.defaultFacadeDetailSymbol, architectureStyle.defaultFacadeDetailIndex);
								substituted = true;
							}
						}
						
						if (substituted) {
							for (int i = 1; i < element.size && (i + x1) < width; i++) {
								elementsPattern.SetElement (x1 + i, y, architectureStyle.defaultFacadeElementSymbol, architectureStyle.defaultFacadeElementIndex);
								detailsPattern.SetElement (x1 + i, y, architectureStyle.defaultFacadeDetailSymbol, architectureStyle.defaultFacadeDetailIndex);
							}
							continue;
						}
					}
					
					if (!element.allowsDetail) {
						detailsPattern.SetElement (x1, y, 'e', 0);
					}
				}
			}
		}
	
	}
	
}