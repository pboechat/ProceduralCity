using UnityEngine;
using System;
using System.Collections.Generic;

public class SymbolicOperations
{
	enum SymbolMatrixDirection
	{
		LEFT,
		RIGHT,
		UP,
		DOWN
	}
	
	public struct SymbolGroup<T> where T: IComparable<T>
	{
		public T symbol;
		public List<Vector2> positions;
		public Rect bounds;
		private bool[] _positionQueryMatrix;
		
		public SymbolGroup (T symbol, List<Vector2> positions, Rect bounds)
		{
			this.symbol = symbol;
			this.positions = positions;
			this.bounds = bounds;
			
			_positionQueryMatrix = new bool[(int)bounds.width * (int)bounds.height];
			for (int y1 = (int)bounds.yMin, y2 = 0; y1 < (int)bounds.yMax; y1++, y2++) {
				for (int x1 = (int)bounds.xMin, x2 = 0; x1 < (int)bounds.xMax; x1++, x2++) {
					_positionQueryMatrix [y2 * (int)bounds.width + x2] = this.positions.Contains (new Vector2 (x1, y1));
				}
			}
		}
		
		int GetQueryMatrixIndex (int x, int y)
		{
			int _y = y - (int)bounds.yMin;
			int _x = x - (int)bounds.xMin;
			return _y * (int)bounds.width + _x;
		}
		
		public bool HasNeighbourAtLeft (Vector2 position)
		{
			if (position.x == 0) {
				return false;
			}
			
			return _positionQueryMatrix [GetQueryMatrixIndex ((int)position.x - 1, (int)position.y)];
		}
		
		public bool HasNeighbourAtRight (Vector2 position)
		{
			if (position.x >= bounds.xMax - 1) {
				return false;
			}
			
			return _positionQueryMatrix [GetQueryMatrixIndex ((int)position.x + 1, (int)position.y)];
		}
		
		public bool HasNeighbourAbove (Vector2 position)
		{
			if (position.y == 0) {
				return false;
			}
			
			return _positionQueryMatrix [GetQueryMatrixIndex ((int)position.x, (int)position.y - 1)];
		}
		
		public bool HasNeighbourBelow (Vector2 position)
		{
			if (position.y >= bounds.yMax - 1) {
				return false;
			}
			
			return _positionQueryMatrix [GetQueryMatrixIndex ((int)position.x, (int)position.y + 1)];
		}
	}
	
	private static SymbolMatrixDirection[] ALL_DIRECTIONS = new SymbolMatrixDirection[] { SymbolMatrixDirection.LEFT, SymbolMatrixDirection.RIGHT, SymbolMatrixDirection.UP, SymbolMatrixDirection.DOWN };
	
	private struct SymbolMatrixPosition<T> where T: IComparable<T>
	{
		private int _x;
		private int _y;
		private T[][] _symbolsMatrix;
		private int _matrixWidth;
		private int _matrixHeight;
		
		public SymbolMatrixPosition (int x, int y, T[][] symbolsMatrix, int matrixWidth, int matrixHeight)
		{
			_x = x;
			_y = y;
			_symbolsMatrix = symbolsMatrix;
			_matrixWidth = matrixWidth;
			_matrixHeight = matrixHeight;
		}
		
		public int x {
			get {
				return _x;
			}
		}
		
		public int y {
			get {
				return _y;
			}
		}
		
		public T symbol {
			get {
				return _symbolsMatrix [_y] [_x];
			}
		}
		
		public bool outOfBounds {
			get {
				return _x == -1 || _y == -1 || _x >= _matrixWidth || _y >= _matrixHeight;
			}
		}
		
		public SymbolMatrixPosition<T> left {
			get {
				return new SymbolMatrixPosition<T> ((_x == 0) ? -1 : _x - 1, _y, _symbolsMatrix, _matrixWidth, _matrixHeight);
			}
		}
		
		public SymbolMatrixPosition<T> right {
			get {
				return new SymbolMatrixPosition<T> (_x + 1, _y, _symbolsMatrix, _matrixWidth, _matrixHeight);
			}
		}
		
		public SymbolMatrixPosition<T> up {
			get {
				return new SymbolMatrixPosition<T> (_x, _y + 1, _symbolsMatrix, _matrixWidth, _matrixHeight);
			}
		}
		
		public SymbolMatrixPosition<T> down {
			get {
				return new SymbolMatrixPosition<T> (_x, (_y == 0) ? -1 : _y - 1, _symbolsMatrix, _matrixWidth, _matrixHeight);
			}
		}
		
		public SymbolMatrixPosition<T> GetNeighbour (SymbolMatrixDirection direction)
		{
			if (direction == SymbolMatrixDirection.LEFT) {
				return left;
			} else if (direction == SymbolMatrixDirection.RIGHT) {
				return right;
			} else if (direction == SymbolMatrixDirection.UP) {
				return up;
			} else {
				return down;
			}
		}
		
		public static bool operator == (SymbolMatrixPosition<T> arg0, SymbolMatrixPosition<T> arg1)
		{
			return arg0._x == arg1._x && arg0._y == arg1._y;
		}
		
		public static bool operator != (SymbolMatrixPosition<T> arg0, SymbolMatrixPosition<T> arg1)
		{
			return arg0._x != arg1._x || arg0._y != arg1._y;
		}
		
		public override bool Equals (object arg0)
		{
			if (arg0 == null) {
				return false;
			}
			
			if (!(arg0 is SymbolMatrixPosition<T>)) {
				return false;
			}
			
			SymbolMatrixPosition<T> other = (SymbolMatrixPosition<T>)arg0;
			return this == other;
		}
		
		public override int GetHashCode ()
		{
			return _x.GetHashCode () | _y.GetHashCode ();
		}
		
		public Vector2 ToVector2 ()
		{
			return new Vector2 (_x, _y);
		}
	}
	
	private SymbolicOperations ()
	{
	}
	
	private static void FindNeighbours<T> (SymbolMatrixPosition<T> position, List<SymbolMatrixPosition<T>> found, T[][] symbolsMatrix) where T: IComparable<T>
	{
		found.Add (position);
		
		foreach (SymbolMatrixDirection direction in ALL_DIRECTIONS) {
			SymbolMatrixPosition<T> neighbour = position.GetNeighbour (direction);
			if (neighbour.outOfBounds) {
				continue;
			}
			
			if (position.symbol.CompareTo (neighbour.symbol) == 0 && !found.Contains (neighbour)) {
				FindNeighbours (neighbour, found, symbolsMatrix);
			}
		}
	}
	
	private static SymbolGroup<T> CreateSymbolGroup<T> (T symbol, List<SymbolMatrixPosition<T>> foundPositions) where T: IComparable<T>
	{
		Rect bounds = new Rect (0, 0, 0, 0);
		List<Vector2> positions = new List<Vector2> ();
		for (int i = 0; i < foundPositions.Count; i++) {
			Vector2 position = foundPositions [i].ToVector2 ();
			
			if (position.x < bounds.xMin) {
				bounds.xMin = position.x;
			} 
			
			if (position.x + 1 > bounds.xMax) {
				bounds.xMax = position.x + 1;
			}
			
			if (position.y < bounds.yMin) {
				bounds.yMin = position.y;
			} 

			if (position.y + 1 > bounds.yMax) {
				bounds.yMax = position.y + 1;
			}
			
			positions.Add (position);
		}
		return new SymbolGroup<T> (symbol, positions, bounds);
	}
	
	public static List<SymbolGroup<T>> ExtractGroups<T> (T[][] symbolsMatrix, int matrixWidth, int matrixHeight) where T: IComparable<T>
	{
		List<SymbolGroup<T>> groups = new List<SymbolGroup<T>> ();
		
		List<SymbolMatrixPosition<T>> all = new List<SymbolMatrixPosition<T>> ();
		for (int y = 0; y < matrixHeight; y++) {
			for (int x = 0; x < matrixWidth; x++) {
				all.Add (new SymbolMatrixPosition<T> (x, y, symbolsMatrix, matrixWidth, matrixHeight));
			}
		}
		
		while (all.Count > 0) {
			List<SymbolMatrixPosition<T>> found = new List<SymbolMatrixPosition<T>> ();
			SymbolMatrixPosition<T> current = all [0];
			FindNeighbours (current, found, symbolsMatrix);
			for (int i = 0; i < found.Count; i++) {
				all.Remove (found [i]);
			}
			groups.Add (CreateSymbolGroup (current.symbol, found));
		}
		
		return groups;
	}
	
}
