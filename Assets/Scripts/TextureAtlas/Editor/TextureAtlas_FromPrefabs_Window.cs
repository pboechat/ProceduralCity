using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using BinPack;

public class TextureAtlas_FromPrefabs_Window : EditorWindow
{
	private class ColorMaskGroup
	{
		public Color colorMask;
		public string meshNameFilter;
	}
	
	private readonly string TEXTURE_FILE_PATTERN = "Assets/{0}/{1}.png";
	private readonly string MATERIAL_FILE_PATTERN = "Assets/{0}/{1}.mat";
	private readonly string COMBINED_MESH_FILE_PATTERN = "Assets/{0}/combined_mesh_{1}.asset";
	private readonly string NEW_MESH_FILE_PATTERN = "Assets/{0}/{1}_{2}.asset";
	private readonly string PREFAB_FILE_PATTERN = "Assets/{0}/{1}.prefab";
	private readonly string ATLAS_FILE_PATTERN = "Assets/{0}/{1}.txt";
	private static TextureAtlas_FromPrefabs_Window _window = null;
	private List<string> _prefabsNames = new List<string> ();
	private bool[] _prefabSelection;
	private string _atlasName = "";
	private string _inputFolder = "Prefabs/input";
	private string _outputFolder = "Prefabs/output";
	private string _meshesFolder = "Prefabs/output/meshes";
	private string _atlasesFolder = "Atlases";
	private string _materialsFolder = "Materials";
	private string _texturesFolder = "Textures";
	private bool _combineChildMeshes;
	private List<ColorMaskGroup> _colorMaskGroups = new List<ColorMaskGroup> ();
	private Color _defaultColor = Color.white;
	private Shader _defaultShader = Shader.Find ("Diffuse");
	private int _minAtlasSize = 128;
	private int _maxAtlasSize = 4096;
	private bool _cleanOutputFolder;
	
	[MenuItem("Texture Atlas/Create From Prefabs %#r")]
	protected static void CreateTextureAtlasFromPrefab ()
	{
		if (_window == null) {
			_window = (TextureAtlas_FromPrefabs_Window)GetWindow ((typeof(TextureAtlas_FromPrefabs_Window)));
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

	Renderer FindRenderer (GameObject gameObject)
	{
		Renderer renderer;
		if ((renderer = gameObject.GetComponent<MeshRenderer> ()) != null) {
			return renderer;
		} else if ((renderer = gameObject.GetComponent<SkinnedMeshRenderer> ()) != null) {
			return renderer;
		} else {
			return null;
		}
	}
	
	Mesh MergeAll (HashSet<MeshFilter> meshFilters, MeshFilter meshFilter, Dictionary<Mesh, Mesh> newMeshes)
	{
		CombineInstance[] combineInstances = new CombineInstance[meshFilters.Count];
		int i = 0;
		foreach (MeshFilter childMeshFilter in meshFilters) {
			CombineInstance combineInstance = new CombineInstance ();
			combineInstance.mesh = newMeshes [childMeshFilter.sharedMesh];
			if (childMeshFilter == meshFilter) {
				combineInstance.transform = Matrix4x4.identity;
			} else {
				combineInstance.transform = Matrix4x4.TRS (childMeshFilter.transform.localPosition, childMeshFilter.transform.localRotation, childMeshFilter.transform.localScale);
			}
			combineInstances [i++] = combineInstance;
		}
		Mesh combinedMesh = new Mesh ();
		combinedMesh.CombineMeshes (combineInstances);
		return combinedMesh;
	}

	void CleanUpFolder (string folder)
	{
		if (!Directory.Exists (folder)) {
			throw new Exception ("directory doesn't exist: " + folder);
		}
		
		foreach (string file in Directory.GetFiles(folder)) {
			File.Delete (file);
		}
	}
	
	Color[] FillColorBuffer (Color color, int count)
	{
		Color[] colors = new Color[count];
		for (int i = 0; i < count; i++) {
			colors [i] = color;
		}
		return colors;
	}

	void Generate ()
	{
		List<GameObject> prefabs = new List<GameObject> ();
		try {
			// ==================
			// BUILD DICTIONARIES
			// ==================
			
			Dictionary<Texture2D, HashSet<MeshFilter>> meshFiltersPerTexture = new Dictionary<Texture2D, HashSet<MeshFilter>> ();
			Dictionary<GameObject, HashSet<MeshFilter>> meshFiltersPerPrefab = new Dictionary<GameObject, HashSet<MeshFilter>> ();
			HashSet<Texture2D> textures = new HashSet<Texture2D> ();
			for (int i = 0; i < _prefabsNames.Count; i++) {
				if (!_prefabSelection [i]) {
					continue;
				}
				string prefabName = _prefabsNames [i];
				GameObject prefab = (GameObject)PrefabUtility.InstantiatePrefab (AssetDatabase.LoadAssetAtPath (string.Format (PREFAB_FILE_PATTERN, _inputFolder, prefabName), typeof(GameObject)));
				PrefabUtility.DisconnectPrefabInstance (prefab);
				prefabs.Add (prefab);
				
				MeshFilter[] childMeshFilters = prefab.GetComponentsInChildren<MeshFilter> ();
				foreach (MeshFilter meshFilter in childMeshFilters) {
					Renderer renderer = FindRenderer (meshFilter.gameObject);
					
					if (renderer == null) {
						Debug.LogWarning ("renderer is null (prefab: " + prefabName + ")");
						continue;
					}
					
					Material material = renderer.sharedMaterial;
					
					if (material.mainTexture == null) {
						Debug.LogWarning ("main texture is null (material: " + material.name + ", prefab: " + prefabName + ")");
						continue;
					}
					
					Texture2D texture = (Texture2D)material.mainTexture;
					
					textures.Add (texture);
					
					HashSet<MeshFilter> prefabMeshFilters;
					if (!meshFiltersPerPrefab.TryGetValue (prefab, out prefabMeshFilters)) {
						prefabMeshFilters = new HashSet<MeshFilter> ();
						meshFiltersPerPrefab.Add (prefab, prefabMeshFilters);
					}
					prefabMeshFilters.Add (meshFilter);
					
					HashSet<MeshFilter> textureMeshFilters;
					if (!meshFiltersPerTexture.TryGetValue (texture, out textureMeshFilters)) {
						textureMeshFilters = new HashSet<MeshFilter> ();
						meshFiltersPerTexture.Add (texture, textureMeshFilters);
					}
					textureMeshFilters.Add (meshFilter);
				}
			}
			
			// ====================
			// CHANGE READ SETTINGS
			// ====================
			
			TextureAtlasHelper.EnableReading (textures);
			
			// ==================
			// CREATE ATLAS RECTS
			// ==================
			
			int size;
			Dictionary<Texture2D, Rect> atlasRectsPerTexture = TextureAtlasHelper.PackTextures (textures, out size, _minAtlasSize, _maxAtlasSize);
			Dictionary<string, Rect> atlasRects = new Dictionary<string, Rect> ();
			foreach (KeyValuePair<Texture2D, Rect> textureAtlasRect in atlasRectsPerTexture) {
				string textureName = TextureAtlasHelper.GetAtlasRectId (textureAtlasRect.Key);
				
				if (atlasRects.ContainsKey (textureName)) {
					int attemptToRename = 1;
					string newTextureName;
					while (true) {
						newTextureName = string.Format ("{0} ({1})", textureName, attemptToRename);
						if (!atlasRects.ContainsKey (newTextureName)) {
							break;
						}
						attemptToRename++;
					}
					Debug.LogWarning ("different textures with same name (newTextureName: " + newTextureName + ")");
					textureName = newTextureName;
				}
				
				atlasRects.Add (textureName, textureAtlasRect.Value);
			}
			
			// ====================
			// CREATE ATLAS TEXTURE
			// ====================
			
			string atlasTexturePath = string.Format (TEXTURE_FILE_PATTERN, _texturesFolder, _atlasName);
			
			AssetDatabase.DeleteAsset (atlasTexturePath);
			Texture2D atlasTexture = TextureAtlasHelper.CreateTexture (atlasRectsPerTexture, size, atlasTexturePath);
			
			// =================
			// UPDATE MESHES UVS
			// =================
			
			Dictionary<Mesh, Mesh> newMeshes = new Dictionary<Mesh, Mesh> ();
			foreach (KeyValuePair<Texture2D, Rect> textureAtlasRect in atlasRectsPerTexture) {
				HashSet<MeshFilter> meshFilters = meshFiltersPerTexture [textureAtlasRect.Key];
				Vector2 scale = new Vector2 (textureAtlasRect.Value.width / (float)size, textureAtlasRect.Value.height / (float)size);
				Vector2 translate = new Vector2 (textureAtlasRect.Value.x / (float)size, textureAtlasRect.Value.y / (float)size);
				foreach (MeshFilter meshFilter in meshFilters) {
					Mesh newMesh = new Mesh ();
					Mesh mesh = meshFilter.sharedMesh;
					
					if (mesh == null) {
						throw new Exception ("mesh is null: " + meshFilter.name);
					}
					
					newMesh.name = mesh.name;
					newMesh.vertices = mesh.vertices;
					newMesh.normals = mesh.normals;
					
					Vector2[] newUv = new Vector2[mesh.uv.Length];
					for (int i = 0; i < mesh.uv.Length; i++) {
						Vector2 uv = mesh.uv [i];
						newUv [i] = translate + new Vector2 (uv.x * scale.x, uv.y * scale.y);
					}
					newMesh.uv = newUv;
	
					newMesh.triangles = mesh.triangles;
					newMesh.tangents = mesh.tangents;
					
					if (_combineChildMeshes) {
						string meshName = meshFilter.name;
						ColorMaskGroup colorMaskGroup = null;
						for (int i = 0; i < _colorMaskGroups.Count; i++) {
							if (meshName.Contains (_colorMaskGroups [i].meshNameFilter)) {
								colorMaskGroup = _colorMaskGroups [i];
								break;
							}
						}
						
						if (colorMaskGroup != null) {
							newMesh.colors = FillColorBuffer (colorMaskGroup.colorMask, mesh.vertices.Length);
						} else {
							newMesh.colors = FillColorBuffer (_defaultColor, mesh.vertices.Length);
						}
					}
					
					newMesh.RecalculateBounds ();
					
					newMeshes.Add (mesh, newMesh);
				}
			}
			
			// ====================
			// BUILD ATLAS MATERIAL
			// ====================
			
			Material atlasMaterial = new Material (_defaultShader);
			string atlasMaterialPath = string.Format (MATERIAL_FILE_PATTERN, _materialsFolder, _atlasName);
			AssetDatabase.DeleteAsset (atlasMaterialPath);
			AssetDatabase.CreateAsset (atlasMaterial, atlasMaterialPath);
			AssetDatabase.SaveAssets ();
			AssetDatabase.Refresh ();
			atlasMaterial = (Material)AssetDatabase.LoadAssetAtPath (atlasMaterialPath, typeof(Material));
			atlasMaterial.mainTexture = atlasTexture;
			
			// ===============
			// SAVE NEW MESHES
			// ===============
			
			if (_cleanOutputFolder) {
				CleanUpFolder ("Assets/" + _outputFolder);
				CleanUpFolder ("Assets/" + _meshesFolder);
			}
			
			foreach (KeyValuePair<GameObject, HashSet<MeshFilter>> prefabMeshFilters in meshFiltersPerPrefab) {
				GameObject prefab = prefabMeshFilters.Key;
				HashSet<MeshFilter> meshFilters = prefabMeshFilters.Value;
				if (_combineChildMeshes) {
					MeshFilter meshFilter;
					meshFilter = prefab.GetComponent<MeshFilter> ();
					
					Mesh sharedMesh = MergeAll (meshFilters, meshFilter, newMeshes);
					
					foreach (MeshFilter meshFilter2 in meshFilters) {
						if (meshFilter2 != meshFilter) {
							DestroyImmediate (meshFilter2.gameObject);
						}
					}
					
					if (meshFilter == null) {
						meshFilter = prefab.AddComponent<MeshFilter> ();
					}
					
					meshFilter.sharedMesh = sharedMesh;
					
					Renderer renderer;
					if ((renderer = FindRenderer (prefab)) == null) {
						renderer = prefab.AddComponent<MeshRenderer> ();
					}
					
					renderer.sharedMaterial = atlasMaterial;
					
					AssetDatabase.CreateAsset (sharedMesh, string.Format (COMBINED_MESH_FILE_PATTERN, _meshesFolder, Guid.NewGuid ().ToString ()));
				} else {
					foreach (MeshFilter meshFilter in meshFilters) {
						Mesh newMesh = newMeshes [meshFilter.sharedMesh];
						
						meshFilter.sharedMesh = newMesh;
						FindRenderer (meshFilter.gameObject).sharedMaterial = atlasMaterial;
						
						AssetDatabase.CreateAsset (newMesh, string.Format (NEW_MESH_FILE_PATTERN, _meshesFolder, newMesh.name, Guid.NewGuid ().ToString ()));
					}
				}
				
				UnityEngine.Object newPrefab = PrefabUtility.CreateEmptyPrefab (string.Format (PREFAB_FILE_PATTERN, _outputFolder, prefab.name));
				PrefabUtility.ReplacePrefab (prefab, newPrefab, ReplacePrefabOptions.ConnectToPrefab);
				DestroyImmediate (prefab);
			}
			
			// ===============
			// SAVE ATLAS FILE
			// ===============
			
			TextureAtlas textureAtlas = new TextureAtlas ();
			textureAtlas.name = _atlasName;
			textureAtlas.size = size;
			textureAtlas.texturePath = atlasTexturePath;
			textureAtlas.atlasRects = atlasRects;
			
			textureAtlas.Save (string.Format (ATLAS_FILE_PATTERN, _atlasesFolder, _atlasName));
			
			AssetDatabase.SaveAssets ();
			AssetDatabase.Refresh ();
		} catch (Exception e) {
			Debug.LogException (e);
			
			foreach (GameObject prefab in prefabs) {
				DestroyImmediate (prefab);
			}
		}
	}
	
	protected virtual void OnGUI ()
	{
		_atlasName = EditorGUILayout.TextField ("Atlas Name: ", _atlasName);
		_inputFolder = EditorGUILayout.TextField ("Input Folder: ", _inputFolder);
		_outputFolder = EditorGUILayout.TextField ("Output Folder: ", _outputFolder);
		_meshesFolder = EditorGUILayout.TextField ("Meshes Folder: ", _meshesFolder);
		_atlasesFolder = EditorGUILayout.TextField ("Atlases Folder: ", _atlasesFolder);
		_materialsFolder = EditorGUILayout.TextField ("Materials Folder: ", _materialsFolder);
		_texturesFolder = EditorGUILayout.TextField ("Textures Folder: ", _texturesFolder);
		_combineChildMeshes = EditorGUILayout.BeginToggleGroup ("Combine Child Meshes", _combineChildMeshes);
		EditorGUILayout.BeginVertical ();
		List<ColorMaskGroup> colorMaskGroupsToRemove = new List<ColorMaskGroup> ();
		foreach (ColorMaskGroup colorMaskGroup in _colorMaskGroups) {
			EditorGUILayout.BeginHorizontal ();
			colorMaskGroup.colorMask = EditorGUILayout.ColorField ("Color Mask: ", colorMaskGroup.colorMask);
			colorMaskGroup.meshNameFilter = EditorGUILayout.TextField ("Mesh Name Filter: ", colorMaskGroup.meshNameFilter);
			if (GUILayout.Button ("Remove")) {
				colorMaskGroupsToRemove.Add (colorMaskGroup);
			}
			EditorGUILayout.EndHorizontal ();
		}
		foreach (ColorMaskGroup colorMaskGroupToRemove in colorMaskGroupsToRemove) {
			_colorMaskGroups.Remove (colorMaskGroupToRemove);
		}
		if (GUILayout.Button ("Add")) {
			_colorMaskGroups.Add (new ColorMaskGroup ());
		}
		EditorGUILayout.EndVertical ();
		EditorGUILayout.EndToggleGroup ();
		_defaultShader = (Shader)EditorGUILayout.ObjectField ("Default Shader: ", _defaultShader, typeof(Shader), true);
		_minAtlasSize = EditorGUILayout.IntField ("Min. Atlas Size: ", _minAtlasSize);
		_maxAtlasSize = EditorGUILayout.IntField ("Max. Atlas Size: ", _maxAtlasSize);
		_cleanOutputFolder = EditorGUILayout.Toggle ("Clean Output Folder?", _cleanOutputFolder);
		
		if (GUILayout.Button ("Refresh")) {
			_prefabsNames.Clear ();
			string[] assetsPaths = AssetDatabase.GetAllAssetPaths ();
			foreach (string assetPath in assetsPaths) {
				if (!assetPath.Contains ("Assets/" + _inputFolder)) {
					continue;
				}
				
				if (assetPath == "Assets/" + _inputFolder) {
					continue;
				}
				
				if (!assetPath.EndsWith (".prefab")) {
					continue;
				}
				
				string modelName = assetPath.Substring (assetPath.LastIndexOf ("/") + 1).Replace (".prefab", "");
				_prefabsNames.Add (modelName);
			}
			
			_prefabSelection = new bool[_prefabsNames.Count];
		}
		
		bool hasSelectedAPrefab = false;
		for (int i = 0; i < _prefabsNames.Count; i++) {
			if ((_prefabSelection [i] = EditorGUILayout.Toggle (_prefabsNames [i], _prefabSelection [i]))) {
				hasSelectedAPrefab = true;
			} 
		}
		
		if (GUILayout.Button ("All")) {
			for (int i = 0; i < _prefabSelection.Length; i++) {
				_prefabSelection [i] = true;
			}
		}
		
		if (GUILayout.Button ("None")) {
			for (int i = 0; i < _prefabSelection.Length; i++) {
				_prefabSelection [i] = false;
			}
		}
		
		if (hasSelectedAPrefab) {
			if (GUILayout.Button ("Generate")) {
				Generate ();
			}
		}
		
		EditorGUIUtility.LookLikeControls ();
	}
	
}
