using UnityEngine;
using System;
using System.Collections.Generic;

public class DirectionHelper
{
	private DirectionHelper ()
	{
	}
	
	public static Direction GetLeft (Direction direction)
	{
		switch (direction) {
		case Direction.FRONT:
			return Direction.LEFT;
		case Direction.LEFT:
			return Direction.BACK;
		case Direction.BACK:
			return Direction.RIGHT;
		case Direction.RIGHT:
			return Direction.FRONT;
		default:
			throw new Exception ("invalid direction: " + direction);
		}
	}
	
	public static bool Contains (Direction direction1, ICollection<Direction> directions)
	{
		foreach (Direction direction2 in directions) {
			if (direction1 == direction2) {
				return true;
			}
		}
		return false;
	}
	
	public static bool HasLeft (Direction direction, ICollection<Direction> directions)
	{
		return Contains (GetLeft (direction), directions);
	}
	
}