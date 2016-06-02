using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;

public class TextureAtlas
{
	private string _name;
	private int _size;
	private string _texture;
	private Dictionary<string, Rect> _atlasRects = new Dictionary<string, Rect> ();
	
	public string name {
		get {
			return this._name;
		}
		set {
			this._name = value;
		}
	}

	public int size {
		get {
			return this._size;
		}
		set {
			this._size = value;
		}
	}

	public string texturePath {
		get {
			return this._texture;
		}
		set {
			this._texture = value;
		}
	}
	
	public Dictionary<string, Rect> atlasRects {
		get {
			return this._atlasRects;
		}
		set {
			this._atlasRects = value;
		}
	}
	
	public void Save (string filePath)
	{
		StreamWriter writer = new StreamWriter (filePath);
		writer.WriteLine ("name=" + _name);
		writer.WriteLine ("size=" + _size);
		writer.WriteLine ("texture=" + _texture);
		writer.WriteLine ("atlasRects:");
		foreach (KeyValuePair<string, Rect> atlasRect in _atlasRects) {
			writer.WriteLine (atlasRect.Key + "=" + ToString (atlasRect.Value));
		}
		writer.Close ();
	}
	
	static string ToString (Rect rect)
	{
		return "[" + rect.x + ", " + rect.y + ", " + rect.width + ", " + rect.height + "]";
	}
	
	static void ParseKeyValue (string line, out string key, out string value)
	{
		string[] keyValuePair = line.Split ('=');
		key = keyValuePair [0];
		value = keyValuePair [1];
	}

	static Rect ToRect (string value)
	{
		float x, y, width, height;
		string[] values = value.Split (',');
		x = float.Parse (values [0].Substring (1).Trim ());
		y = float.Parse (values [1].Trim ());
		width = float.Parse (values [2].Trim ());
		height = float.Parse (values [3].Substring (0, values [3].Length - 1).Trim ());
		return new Rect (x, y, width, height);
	}
	
	public static TextureAtlas LoadFromString (string content)
	{
		StringReader reader = new StringReader (content);
		
		TextureAtlas textureAtlas = new TextureAtlas ();
		
		string nameKey;
		ParseKeyValue (reader.ReadLine (), out nameKey, out textureAtlas._name);
		if (nameKey != "name") {
			throw new Exception ("invalid key (expected: name, got: " + nameKey + ")");
		}
		string sizeKey;
		string sizeStr;
		ParseKeyValue (reader.ReadLine (), out sizeKey, out sizeStr);
		if (sizeKey != "size") {
			throw new Exception ("invalid key (expected: size, got: " + sizeKey + ")");
		}
		textureAtlas._size = int.Parse (sizeStr);
		string textureKey;
		ParseKeyValue (reader.ReadLine (), out textureKey, out textureAtlas._texture);
		if (textureKey != "texture") {
			throw new Exception ("invalid key (expected: texture, got: " + textureKey + ")");
		}
		string atlasRectsSection;
		if ((atlasRectsSection = reader.ReadLine ()) != "atlasRects:") {
			throw new Exception ("invalid key (expected: atlasRects, got: " + atlasRectsSection + ")");
		}
		string line;
		while ((line = reader.ReadLine ()) != null) {
			string atlasRectKey;
			string atlasRectValue;
			ParseKeyValue (line, out atlasRectKey, out atlasRectValue);
			textureAtlas._atlasRects.Add (atlasRectKey, ToRect (atlasRectValue));
		}
		reader.Close ();
		
		return textureAtlas;
	}
	
	public static TextureAtlas LoadFromFile (string filePath)
	{
		StreamReader reader = new StreamReader (filePath);
		TextureAtlas textureAtlas = LoadFromString (reader.ReadToEnd ());
		reader.Close ();
		return textureAtlas;
	}
	
}