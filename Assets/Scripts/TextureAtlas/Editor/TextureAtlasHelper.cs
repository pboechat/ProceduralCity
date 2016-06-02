using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections.Generic;
using BinPack;

public class TextureAtlasHelper
{
	private TextureAtlasHelper ()
	{
	}
	
	public static string GetAtlasRectId (Texture2D texture)
	{
		string texturePath = AssetDatabase.GetAssetPath (texture);
		int start = texturePath.LastIndexOf ("/");
		int end = texturePath.LastIndexOf (".");
		return texturePath.Substring (start + 1, end - start - 1);
	}
	
	public static void EnableReading (ICollection<Texture2D> textures)
	{
		foreach (Texture2D texture in textures) {
			EnableReading (texture);
		}
	}
	
	public static void EnableReading (Texture2D texture)
	{
		string texturePath = AssetDatabase.GetAssetPath (texture);
		TextureImporter textureImporter = (TextureImporter)AssetImporter.GetAtPath (texturePath);
		textureImporter.isReadable = true;
		TextureImporterSettings settings = new TextureImporterSettings ();
		textureImporter.ReadTextureSettings (settings);
		settings.readable = true;
		textureImporter.SetTextureSettings (settings);
		AssetDatabase.ImportAsset (texturePath, ImportAssetOptions.ForceUpdate);
	}
	
	public static Dictionary<Texture2D, Rect> PackTextures (HashSet<Texture2D> textures, out int size, int minAtlasSize, int maxAtlasSize)
	{
		MaxRects maxRects;
		size = minAtlasSize;
		Dictionary<Texture2D, Rect> atlasRectsPerTexture = new Dictionary<Texture2D, Rect> ();
		while (true) {
			maxRects = new MaxRects (size, size, false);
			bool fit = true;
			atlasRectsPerTexture.Clear ();
			foreach (Texture2D texture in textures) {
				Rect atlasRect = maxRects.Insert (texture.width, texture.height, MaxRects.FreeRectChoiceHeuristic.RectBestAreaFit);
				if (atlasRect.width == 0 || atlasRect.height == 0) {
					size *= 2;
					
					if (size > maxAtlasSize) {
						throw new Exception ("texture atlas bigger than " + maxAtlasSize + "x" + maxAtlasSize);
					}
					fit = false;
					break;
				}
				atlasRectsPerTexture.Add (texture, atlasRect);
			}
			if (fit) {
				break;
			}
		}
		return atlasRectsPerTexture;
	}
	
	public static Dictionary<string, Rect> RepackRects (Dictionary<string, Rect> atlasRects1, out int size, int minAtlasSize, int maxAtlasSize)
	{
		MaxRects maxRects;
		size = minAtlasSize;
		Dictionary<string, Rect> atlasRects2 = new Dictionary<string, Rect> ();
		while (true) {
			maxRects = new MaxRects (size, size, false);
			bool fit = true;
			atlasRects2.Clear ();
			foreach (KeyValuePair<string, Rect> atlasRect1 in atlasRects1) {
				int width = Mathf.CeilToInt (atlasRect1.Value.width);
				int height = Mathf.CeilToInt (atlasRect1.Value.height);
				
				Rect atlasRect2 = maxRects.Insert (width, height, MaxRects.FreeRectChoiceHeuristic.RectBestAreaFit);
				if (atlasRect2.width == 0 || atlasRect2.height == 0) {
					size *= 2;
					
					if (size > maxAtlasSize) {
						throw new Exception ("texture atlas bigger than " + maxAtlasSize + "x" + maxAtlasSize);
					}
					fit = false;
					break;
				}
				atlasRects2.Add (atlasRect1.Key, atlasRect2);
			}
			if (fit) {
				break;
			}
		}
		return atlasRects2;
	}
	
	public static Dictionary<Texture2D, Rect> CutTexturesFromAtlasRects (Dictionary<string, Rect> atlasRects, Texture2D atlasTexture)
	{
		Dictionary<Texture2D, Rect> texturesAtlasRects = new Dictionary<Texture2D, Rect> ();
		foreach (KeyValuePair<string, Rect> atlasRect in atlasRects) {
			int width = Mathf.CeilToInt (atlasRect.Value.width);
			int height = Mathf.CeilToInt (atlasRect.Value.height);
			int x = Mathf.CeilToInt (atlasRect.Value.x);
			int y = Mathf.CeilToInt (atlasRect.Value.y);
			
			Texture2D texture = new Texture2D (width, height, TextureFormat.RGBA32, false);
			Color[] pixels = atlasTexture.GetPixels (x, y, width, height);
			texture.SetPixels (pixels);
			texture.Apply ();
			texturesAtlasRects.Add (texture, atlasRect.Value);
		}
		return texturesAtlasRects;
	}
	
	public static Texture2D CreateTexture (Dictionary<Texture2D, Rect> texturesAtlasRects, int size, string atlasTexturePath)
	{
		// assemble texture atlas
		Texture2D atlasTexture = new Texture2D (size, size, TextureFormat.RGBA32, false);
		foreach (KeyValuePair<Texture2D, Rect> textureAtlasRect in texturesAtlasRects) {
			Texture2D texture = textureAtlasRect.Key;
			
			Color[] pixels = texture.GetPixels ();
			
			int x = Mathf.CeilToInt (textureAtlasRect.Value.x);
			int y = Mathf.CeilToInt (textureAtlasRect.Value.y);
			
			atlasTexture.SetPixels (x, y, texture.width, texture.height, pixels);
		}
		atlasTexture.Apply ();
		
		// save to disk
		File.WriteAllBytes (atlasTexturePath, atlasTexture.EncodeToPNG ());
		
		// refresh assetdatabase
		AssetDatabase.SaveAssets ();
		AssetDatabase.Refresh ();
		
		// change texture importer settings
		TextureImporter textureImporter = (TextureImporter)AssetImporter.GetAtPath (atlasTexturePath);
		TextureImporterSettings settings = new TextureImporterSettings ();
		textureImporter.ReadTextureSettings (settings);
		settings.readable = true;
		settings.mipmapEnabled = false;
		settings.filterMode = FilterMode.Point;
		settings.maxTextureSize = size;
		settings.textureFormat = TextureImporterFormat.DXT5;
		textureImporter.SetTextureSettings (settings);
		
		// reimport texture
		AssetDatabase.ImportAsset (atlasTexturePath, ImportAssetOptions.ForceUpdate);
		
		// reload from disk
		atlasTexture = (Texture2D)AssetDatabase.LoadAssetAtPath (atlasTexturePath, typeof(Texture2D));
		
		return atlasTexture;
	}
	
}