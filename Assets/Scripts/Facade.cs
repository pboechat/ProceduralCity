using UnityEngine;
using Pattern;

public class Facade
{
	public Direction direction;
	public int width;
	public int height;
	public int widthInTiles;
	public int heightInTiles;
	public Pattern<FacadeItem> elementsPattern;
	public Pattern<FacadeItem> detailsPattern;
	public Pattern<FacadeOperation> operationsPattern;
	public ArchitectureStyle architectureStyle;
	
	public Facade (Direction direction, int width, int height, int widthInTiles, int heightInTiles, ArchitectureStyle architecturalStyle, Pattern<FacadeItem> elementsPattern, Pattern<FacadeItem> detailsPattern, Pattern<FacadeOperation> operationsPattern)
	{
		this.direction = direction;
		this.width = width;
		this.height = height;
		this.widthInTiles = widthInTiles;
		this.heightInTiles = heightInTiles;
		this.architectureStyle = architecturalStyle;
		this.elementsPattern = elementsPattern;
		this.detailsPattern = detailsPattern;
		this.operationsPattern = operationsPattern;
	}
}