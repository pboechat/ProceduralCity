using UnityEngine;
using System.Collections.Generic;
using System;

public class Building
{
	public int x;
	public int y;
	public int width;
	public int height;
	public int depth;
	public int widthInTiles;
	public int heightInTiles;
	public int depthInTiles;
	public ArchitectureStyle architectureStyle;
	private Dictionary<Direction, Facade> _facades = new Dictionary<Direction, Facade> ();
	
	public ICollection<Direction> facadeDirections {
		get {
			return _facades.Keys;
		}
	}
	
	public ICollection<Facade> facades {
		get {
			return _facades.Values;
		}
	}
	
	public Building (int x, int y, int width, int height, int depth, int widthInTiles, int heightInTiles, int depthInTiles, Facade[] facades, ArchitectureStyle architecturalStyle)
	{
		this.x = x;
		this.y = y;
		this.width = width;
		this.height = height;
		this.depth = depth;
		this.widthInTiles = widthInTiles;
		this.heightInTiles = heightInTiles;
		this.depthInTiles = depthInTiles;
		this.architectureStyle = architecturalStyle;
		
		for (int i = 0; i < facades.Length; i++) {
			_facades.Add (facades [i].direction, facades [i]);
		}
	}
	
	public bool HasFacade (Direction direction)
	{
		return _facades.ContainsKey (direction);
	}
	
	public Facade GetFacade (Direction direction)
	{
		Facade facade;
		if (_facades.TryGetValue (direction, out facade)) {
			return facade;
		}
		throw new Exception ("this building has no facade in the direction: " + direction);
	}
	
}