using UnityEngine;
using System.Collections.Generic;

public class Allotment
{
	public int x;
	public int y;
	public int width;
	public int depth;
	public int widthInTiles;
	public int depthInTiles;
	public Direction[] freeFaces;
	public ArchitectureStyle architectureStyle;
	
	public Allotment (int x, int y, int width, int depth, int widthInTiles, int depthInTiles, Direction[] freeFaces, ArchitectureStyle architectureStyle)
	{
		this.x = x;
		this.y = y;
		this.width = width;
		this.depth = depth;
		this.widthInTiles = widthInTiles;
		this.depthInTiles = depthInTiles;
		this.freeFaces = freeFaces;
		this.architectureStyle = architectureStyle;
	}
	
}