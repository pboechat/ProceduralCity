using UnityEngine;
using UnityEditor;
using System.IO;

public class NormalmapMacro
{
	private NormalmapMacro ()
	{
	}
	
	static void EnableReading (Texture2D texture)
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
	
	[MenuItem("Normalmap/Generate %#n")]
	public static void GenerateNormalmap ()
	{
		if (!(Selection.activeObject is Texture2D)) {
			Debug.LogError ("selection is not texture");
		}
		
		Texture2D src = (Texture2D)Selection.activeObject;
		EnableReading (src);
		Texture2D dst = Normalmap.Generate (src, 1.0f);
		
		string srcPath = AssetDatabase.GetAssetPath (src);
		File.Delete (srcPath);
		
		int end = srcPath.LastIndexOf (".");
		string dstPath = srcPath.Substring (0, end) + ".png";
		File.WriteAllBytes (dstPath, dst.EncodeToPNG ());
		
		AssetDatabase.Refresh ();
	}
	
}
