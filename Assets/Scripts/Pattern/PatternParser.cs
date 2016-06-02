using UnityEngine;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace Pattern
{
	public class PatternParser
	{
		public static Pattern<T>[] ParsePatterns<T> (TextAsset patternsFile, ArchitectureStyle architectureStyle) where T : IPatternItem, new()
		{
			string[] patternsStrings = Regex.Split (patternsFile.text, @"\-");
			Pattern<T>[] patterns = new Pattern<T>[patternsStrings.Length];
			for (int i = 0; i < patterns.Length; i++) {
				string patternString = patternsStrings [i].Trim ();
				if (patternString.Length == 0) {
					continue;
				}
				patterns [i] = ParsePattern<T> (patternString, architectureStyle);
			}
			return patterns;
		}
	
		static Pattern<T> ParsePattern<T> (string patternString, ArchitectureStyle architectureStyle) where T : IPatternItem, new()
		{
			StringReader reader = new StringReader (patternString);
		
			List<string> lines = new List<string> ();
			string line;
			while ((line = reader.ReadLine()) != null) {
				line = line.Trim ();
				if (line.Length == 0) {
					continue;
				}
				lines.Add (line);
			}
		
			int numberOfLines = lines.Count;
			int numberOfColumns = -1;
			Pattern<T> pattern = null;
			for (int y = 0; y < numberOfLines; y++) {
				string[] columns = Regex.Split (lines [y], @"\s+");
			
				if (numberOfColumns == -1) {
					numberOfColumns = columns.Length;
				} else if (numberOfColumns != columns.Length) {
					throw new Exception ("different column sizes in pattern matrix");
				}
			
				if (pattern == null) {
					pattern = new Pattern<T> (numberOfColumns, numberOfLines, architectureStyle);
				}
			
				for (int x = 0; x < numberOfColumns; x++) {
					string elementString = columns [x];
				
					if (elementString.Length < 2) {
						throw new Exception ("invalid element: " + elementString);
					}
				
					char symbol = elementString [0];
					int index = int.Parse (elementString.Substring (1));
				
					pattern.SetElement (x, y, symbol, index);
				}
			}
		
			return pattern;
		}
	
	}
	
}