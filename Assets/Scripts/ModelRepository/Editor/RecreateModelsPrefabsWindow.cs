using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;

public class RecreateModelsPrefabsWindow : EditorWindow
{
	private static bool _isWindowOpen;
	private List<string> _modelsNames = new List<string> ();
	private bool[] _modelSelections;
	private string _modelsFolder = "Models";
	private string _materialsFolder = "Materials";
	private string _texturesFolder = "Textures";
	private string _modelRepositoryFolder = "Resources/Repository";
	private Vector3 _rotation = Vector3.zero;
	
	[MenuItem("Model Repository/Recreate Models Prebafs")]
	protected static void RecreateModelsPrebafs ()
	{
		if (!_isWindowOpen) {
			_isWindowOpen = true;
			RecreateModelsPrefabsWindow editor = (RecreateModelsPrefabsWindow)GetWindow ((typeof(RecreateModelsPrefabsWindow)));
			if (editor) {
				editor.autoRepaintOnSceneChange = true;
			}
		}
	}
	
	protected void OnDestroy ()
	{
		_isWindowOpen = false;
	}
	
	Renderer FindProperRenderer (GameObject model)
	{
		if (model.GetComponent<Renderer>() != null) {
			return model.GetComponent<Renderer>();
		}
		
		if (model.name.Contains ("window") || model.name.Contains ("balcony")) {
			for (int i = 0; i < model.transform.childCount; i++) {
				GameObject child = model.transform.GetChild (i).gameObject;
				if (child.name == "frame") {
					if (child.GetComponent<Renderer>() == null) {
						break;
					}
					
					return child.GetComponent<Renderer>();
				}
			}
		}
		
		// FIXME: checking invariants
		throw new Exception ("there's no renderer in: " + model.name);
	}
	
	bool TryToFindTexture (string textureName, out Texture2D texture)
	{
		texture = (Texture2D)AssetDatabase.LoadAssetAtPath ("Assets/" + _texturesFolder + "/" + textureName, typeof(Texture2D));
		return (texture != null);
	}
	
	Material CreateMaterial (string materialName)
	{
		Material material = new Material (Shader.Find ("Diffuse"));
		Texture2D texture;
		string textureName = materialName.Replace (".mat", ".jpg");
		if (!TryToFindTexture (textureName, out texture)) {
			textureName = materialName.Replace (".mat", ".png");
			if (!TryToFindTexture (textureName, out texture)) {
				throw new Exception ("Texture not found: " + textureName);
			}
		}
		material.SetTexture ("_MainTex", texture);
		AssetDatabase.CreateAsset (material, "Assets/" + _materialsFolder + "/" + materialName + ".mat");
		return material;
	}

	void Recreate ()
	{
		for (int i = 0; i < _modelsNames.Count; i++) {
			if (!_modelSelections [i]) {
				continue;
			}
			string modelName = _modelsNames [i];
			GameObject model = (GameObject)MonoBehaviour.Instantiate (AssetDatabase.LoadAssetAtPath ("Assets/" + _modelsFolder + "/" + modelName, typeof(GameObject)), Vector3.zero, Quaternion.Euler (_rotation));
			string materialName = modelName.Replace (".FBX", ".mat");
			Material material = (Material)AssetDatabase.LoadAssetAtPath ("Assets/" + _materialsFolder + "/" + materialName, typeof(Material));
			if (material == null) {
				material = CreateMaterial (materialName);
			}
			FindProperRenderer (model).material = material;
			string prefabName = modelName.Replace (".FBX", ".prefab");
			UnityEngine.Object prefab = PrefabUtility.CreateEmptyPrefab ("Assets/" + _modelRepositoryFolder + "/" + prefabName);
			PrefabUtility.ReplacePrefab (model, prefab, ReplacePrefabOptions.ConnectToPrefab);
			DestroyImmediate (model);
		}
	}
	
	protected virtual void OnGUI ()
	{
		_modelsFolder = EditorGUILayout.TextField ("Models Folder: ", _modelsFolder);
		_materialsFolder = EditorGUILayout.TextField ("Materials Folder: ", _materialsFolder);
		_modelRepositoryFolder = EditorGUILayout.TextField ("Model Repository Folder: ", _modelRepositoryFolder);
		_rotation = EditorGUILayout.Vector3Field ("Rotation: ", _rotation);
		
		if (GUILayout.Button ("Refresh")) {
			_modelsNames.Clear ();
			string[] assetsPaths = AssetDatabase.GetAllAssetPaths ();
			foreach (string assetPath in assetsPaths) {
				if (!assetPath.Contains ("Assets/" + _modelsFolder)) {
					continue;
				}
				
				if (assetPath == "Assets/" + _modelsFolder) {
					continue;
				}
				
				string modelName = assetPath.Substring (assetPath.LastIndexOf ("/") + 1);
				_modelsNames.Add (modelName);
			}
			
			_modelSelections = new bool[_modelsNames.Count];
		}
		
		bool hasSelectedAModel = false;
		for (int i = 0; i < _modelsNames.Count; i++) {
			if ((_modelSelections [i] = EditorGUILayout.Toggle (_modelsNames [i], _modelSelections [i]))) {
				hasSelectedAModel = true;
			}
		}
		
		if (GUILayout.Button ("All")) {
			for (int i = 0; i < _modelSelections.Length; i++) {
				_modelSelections [i] = true;
			}
		}
		
		if (GUILayout.Button ("None")) {
			for (int i = 0; i < _modelSelections.Length; i++) {
				_modelSelections [i] = false;
			}
		}
		
		if (hasSelectedAModel) {
			if (GUILayout.Button ("Recreate")) {
				Recreate ();
			}
		}
		
		EditorGUIUtility.LookLikeControls ();
	}
	
}
