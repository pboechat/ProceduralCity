using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using BinPack;

public class TextureAtlas_Manage_Window : EditorWindow
{
	private static TextureAtlas_Manage_Window _window = null;
	private TextAsset _atlasFile;
	private bool _enableAtlasResize;
	private int _minAtlasSize = 128;
	private int _maxAtlasSize = 4096;
	private Texture2D _newTexture;
	private string _newTextureName;
	
	[MenuItem("Texture Atlas/Manage %#m")]
	protected static void ManageTextureAtlas ()
	{
		if (_window == null) {
			_window = (TextureAtlas_Manage_Window)GetWindow ((typeof(TextureAtlas_Manage_Window)));
			if (_window != null) {
				_window.title = "Texture Atlas";
				_window.autoRepaintOnSceneChange = true;
			}
		}
	}
	
	protected void OnDestroy ()
	{
		_window = null;
	}

	void AddNewTexture (TextureAtlas textureAtlas)
	{
		// load atlas texture from disk
		Texture2D atlasTexture = (Texture2D)AssetDatabase.LoadAssetAtPath (textureAtlas.texturePath, typeof(Texture2D));
		
		// cut texture rects from atlas texture
		Dictionary<Texture2D, Rect> textureAtlasRects = TextureAtlasHelper.CutTexturesFromAtlasRects (textureAtlas.atlasRects, atlasTexture);
		
		textureAtlas.atlasRects.Add (_newTextureName, new Rect (0, 0, _newTexture.width, _newTexture.height));
		
		// update atlas rects
		int size;
		textureAtlas.atlasRects = TextureAtlasHelper.RepackRects (textureAtlas.atlasRects, out size, (_enableAtlasResize) ? _minAtlasSize : textureAtlas.size, (_enableAtlasResize) ? _maxAtlasSize : textureAtlas.size);
		
		// update atlas size
		textureAtlas.size = size;
		
		// add new texture to texture rects
		textureAtlasRects.Add (_newTexture, textureAtlas.atlasRects [_newTextureName]);
		
		TextureAtlasHelper.EnableReading (_newTexture);
		
		// recreate atlas texture
		TextureAtlasHelper.CreateTexture (textureAtlasRects, textureAtlas.size, textureAtlas.texturePath);
		
		// save atlas file to disk
		string atlasPath = AssetDatabase.GetAssetPath (_atlasFile);
		textureAtlas.Save (atlasPath);
		AssetDatabase.Refresh();
	}

	void RemoveTextures (TextureAtlas textureAtlas, List<string> atlasRectsToRemove)
	{
		foreach (string atlasRectToRemove in atlasRectsToRemove) {
			textureAtlas.atlasRects.Remove (atlasRectToRemove);
		}
		
		// update atlas rects
		int size;
		textureAtlas.atlasRects = TextureAtlasHelper.RepackRects (textureAtlas.atlasRects, out size, (_enableAtlasResize) ? _minAtlasSize : textureAtlas.size, (_enableAtlasResize) ? _maxAtlasSize : textureAtlas.size);
		
		// update atlas size
		textureAtlas.size = size;
		
		// load atlas texture from disk
		Texture2D atlasTexture = (Texture2D)AssetDatabase.LoadAssetAtPath (textureAtlas.texturePath, typeof(Texture2D));
		
		// cut texture rects from atlas texture
		Dictionary<Texture2D, Rect> textureAtlasRects = TextureAtlasHelper.CutTexturesFromAtlasRects (textureAtlas.atlasRects, atlasTexture);
		
		// recreate atlas texture
		TextureAtlasHelper.CreateTexture (textureAtlasRects, textureAtlas.size, textureAtlas.texturePath);
		
		// save atlas file to disk
		string atlasPath = AssetDatabase.GetAssetPath (_atlasFile);
		textureAtlas.Save (atlasPath);
		AssetDatabase.Refresh();
	}
	
	void OnGUI ()
	{
		_atlasFile = (TextAsset)EditorGUILayout.ObjectField ("Atlas File: ", _atlasFile, typeof(TextAsset), true);
		_enableAtlasResize = EditorGUILayout.BeginToggleGroup ("Enable Atlas Resize", _enableAtlasResize);
		_minAtlasSize = EditorGUILayout.IntField ("Min. Atlas Size: ", _minAtlasSize);
		_maxAtlasSize = EditorGUILayout.IntField ("Max. Atlas Size: ", _maxAtlasSize);
		EditorGUILayout.EndToggleGroup ();
		
		if (_atlasFile == null) {
			return;
		}
		
		EditorGUILayout.Separator ();
		
		TextureAtlas textureAtlas;
		try {
			textureAtlas = TextureAtlas.LoadFromString (_atlasFile.text);
		} catch (Exception e) {
			Debug.LogException (e);
			return;
		}
		
		// ===========
		// ATLAS RECTS
		// ===========
		
		EditorGUILayout.BeginVertical ();
		List<string> atlasRectsToRemove = new List<string> ();
		foreach (KeyValuePair<string, Rect> atlasRect in textureAtlas.atlasRects) {
			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField ("name: " + atlasRect.Key);
			Rect rect = atlasRect.Value;
			EditorGUILayout.LabelField ("x: " + rect.x + "", GUILayout.Width (50));
			EditorGUILayout.LabelField ("y: " + rect.y + "", GUILayout.Width (50));
			EditorGUILayout.LabelField ("width: " + rect.width + "", GUILayout.Width (80));
			EditorGUILayout.LabelField ("height: " + rect.height + "", GUILayout.Width (80));
			if (GUILayout.Button ("Remove", GUILayout.Width (60))) {
				atlasRectsToRemove.Add (atlasRect.Key);
			}
			EditorGUILayout.EndHorizontal ();
		}
		EditorGUILayout.EndVertical ();
			
		if (atlasRectsToRemove.Count > 0) {
			RemoveTextures (textureAtlas, atlasRectsToRemove);
		}
			
		EditorGUILayout.Separator ();
		
		// ===========
		// NEW TEXTURE
		// ===========
		
		EditorGUILayout.BeginVertical ();
		_newTexture = (Texture2D)EditorGUILayout.ObjectField ("Texture: ", _newTexture, typeof(Texture2D), true);
		_newTextureName = EditorGUILayout.TextField ("Name: ", _newTextureName);
		if (_newTexture != null && !string.IsNullOrEmpty (_newTextureName)) {
			if (GUILayout.Button ("Add")) {
				AddNewTexture (textureAtlas);
			}
		}
		EditorGUILayout.EndVertical ();
	}
	
}
