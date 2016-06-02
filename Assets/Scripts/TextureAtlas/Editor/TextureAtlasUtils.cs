using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;
using BinPack;

public class TextureAtlasUtils
{
	private TextureAtlasUtils ()
	{
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
				int textureWidth = Mathf.NextPowerOfTwo (texture.width);
				int textureHeight = Mathf.NextPowerOfTwo (texture.height);
				Rect atlasRect = maxRects.Insert (textureWidth, textureHeight, MaxRects.FreeRectChoiceHeuristic.RectBestAreaFit);
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
				Rect atlasRect2 = maxRects.Insert ((int)atlasRect1.Value.width, (int)atlasRect1.Value.height, MaxRects.FreeRectChoiceHeuristic.RectBestAreaFit);
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
	
	public static Texture2D CreateTexture (Dictionary<string, Rect> atlasRects, int size, string textureFile)
	{
		Texture2D atlasTexture = new Texture2D (size, size, TextureFormat.RGBA32, false);
		foreach (KeyValuePair<string, Rect> atlasRect in atlasRects) {
			Texture2D texture = (Texture2D)UnityEditor.AssetDatabase.LoadAssetAtPath (atlasRect.Key, typeof(Texture2D));
			
			Color[] pixels = texture.GetPixels ();
			atlasTexture.SetPixels ((int)atlasRect.Value.x, (int)atlasRect.Value.y, texture.width, texture.height, pixels);
		}
		atlasTexture.Apply ();
		File.WriteAllBytes (textureFile, atlasTexture.EncodeToPNG ());
		return atlasTexture;
	}
	
	public static Texture2D CreateTexture (Dictionary<Texture2D, Rect> texturesAtlasRects, int size, string textureFile)
	{
		Texture2D atlasTexture = new Texture2D (size, size, TextureFormat.RGBA32, false);
		foreach (KeyValuePair<Texture2D, Rect> textureAtlasRect in texturesAtlasRects) {
			Texture2D texture = textureAtlasRect.Key;
			
			Color[] pixels = texture.GetPixels ();
			atlasTexture.SetPixels ((int)textureAtlasRect.Value.x, (int)textureAtlasRect.Value.y, texture.width, texture.height, pixels);
		}
		atlasTexture.Apply ();
		File.WriteAllBytes (textureFile, atlasTexture.EncodeToPNG ());
		return atlasTexture;
	}
	
}