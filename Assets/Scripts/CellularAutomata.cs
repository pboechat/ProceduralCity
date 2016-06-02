using UnityEngine;
using System;
using System.Collections.Generic;

public class CellularAutomata
{
	
	public const int FALSE = 0;
	public const int TRUE = 1;
	
	public class Generation
	{
		
		public int rule1Cutoff;
		public int rule2Cutoff;
		public int repetitions;
		
		public Generation (int rule1Cutoff, int rule2Cutoff, int repetitions)
		{
			this.rule1Cutoff = rule1Cutoff;
			this.rule2Cutoff = rule2Cutoff;
			this.repetitions = repetitions;
		}
		
	}
	
	private int[][] _mapGrid;
	private int[][] _auxiliaryMapGrid;
	private int _fillProbability;
	private int _mapWidth;
	private int _mapHeight;
	private List<Generation> _generations;
	
	public CellularAutomata (int mapWidth, int mapHeight, int fillProbability, List<Generation> generations)
	{
		this._mapWidth = mapWidth;
		this._mapHeight = mapHeight;
		this._fillProbability = fillProbability;
		this._generations = generations;
	}
	
	private int RandomPick ()
	{
		if ((UnityEngine.Random.value * 100.0f) < _fillProbability) {
			return TRUE;
		} else {
			return FALSE;
		}
	}
	
	public void Initialize ()
	{
		_mapGrid = new int[_mapHeight][];
		_auxiliaryMapGrid = new int[_mapHeight][];
		
		for (int y = 0; y < _mapHeight; y++) {
			_mapGrid [y] = new int[_mapWidth];
			_auxiliaryMapGrid [y] = new int[_mapWidth];
		}
		
		for (int y = 1; y < _mapHeight - 1; y++) {
			for (int x = 1; x < _mapWidth - 1; x++) {
				_mapGrid [y] [x] = RandomPick ();
			}
		}
		
		for (int y = 0; y < _mapHeight; y++) {
			for (int x = 0; x < _mapWidth; x++) {
				_auxiliaryMapGrid [y] [x] = TRUE;
			}
		}
		
		for (int y = 0; y < _mapHeight; y++) {
			_mapGrid [y] [0] = _mapGrid [y] [_mapWidth - 1] = TRUE;
		}
		
		for (int x = 0; x < _mapWidth; x++) {
			_mapGrid [0] [x] = _mapGrid [_mapHeight - 1] [x] = TRUE;
		}
	}
	
	public void RunGeneration (Generation generation)
	{
		if (generation == null) {
			throw new Exception ("Null generation");
		}
		
		for (int y = 1; y < _mapHeight - 1; y++) {
			for (int x = 1; x < _mapWidth - 1; x++) {
				int rule1Count = 0;
				int rule2Count = 0;
				
				for (int i = -1; i <= 1; i++) {
					for (int j = -1; j <= 1; j++) {
						if (_mapGrid [y + i] [x + j] != FALSE) {
							rule1Count++;
						}
					}
				}
				
				for (int i = y - 2; i <= y + 2; i++) {
					for (int j = x - 2; j <= x + 2; j++) {
						if (Math.Abs (i - y) == 2 && Math.Abs (j - x) == 2) {
							continue;
						}
						
						if (i < 0 || j < 0 || i >= _mapHeight || j >= _mapWidth) {
							continue;
						}
						
						if (_mapGrid [i] [j] != FALSE) {
							rule2Count++;
						}
					}
				}
				
				if (rule1Count >= generation.rule1Cutoff || rule2Count <= generation.rule2Cutoff) {
					_auxiliaryMapGrid [y] [x] = TRUE;
				} else {
					_auxiliaryMapGrid [y] [x] = FALSE;
				}
			}
		}
		
		for (int y = 1; y < _mapHeight - 1; y++) {
			for (int x = 1; x < _mapWidth - 1; x++) {
				_mapGrid [y] [x] = _auxiliaryMapGrid [y] [x];
			}
		}
	}
	
	public int[][] Generate ()
	{
		Initialize ();
		
		foreach (Generation generation in _generations) {
			for (int j = 0; j < generation.repetitions; j++) {
				RunGeneration (generation);
			}
		}
		
		return _mapGrid;
	}
	
	public void PrintParameters ()
	{
		string parametersString = "W[0](p) = rand[0,100) < " + _fillProbability + "\n";
		
		for (int i = 0; i < _generations.Count; i++) {
			Generation generation = _generations [i];
			
			parametersString += "Repeat " + generation.repetitions + ": W[" + i + "](p) = R1(p) >= " + generation.rule1Cutoff;
			
			if (generation.rule2Cutoff >= 0) {
				parametersString += " || R2(p) <= " + generation.rule2Cutoff + "\n";
			} else {
				parametersString += "\n";
			}
		}
		
		Debug.Log (parametersString);
	}
	
	public void PrintMap ()
	{
		for (int y = 0; y < _mapHeight; y++) {
			string mapString = "";
			for (int x = 0; x < _mapWidth; x++) {
				switch (_mapGrid [y] [x]) {
				case TRUE:
					mapString += '#';
					break;
				case FALSE:
					mapString += '.';
					break;
				}
			}
			Debug.Log (mapString);
		}
	}
	
}

