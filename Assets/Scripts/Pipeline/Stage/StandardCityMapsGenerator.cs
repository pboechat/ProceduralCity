using UnityEngine;
using System;

public class StandardCityMapsGenerator : CityMapsGenerator
{
	[Serializable]
	public class ArchitectureStyleColorMapping
	{
		public string architectureStyleName;
		public Color color;
	}
	
	[SerializeField]
	private float _cityCenterDivergence;
	[SerializeField]
	private float _groundZeroDivergence;
	[SerializeField]
	private bool _saveMapsToDisk;
	[SerializeField]
	private string _mapsFolder = "Debug/CityMaps";
	[SerializeField]
	private ArchitectureStyleColorMapping[] _architectureStyleColorMappings;
	
	static Vector2 DivergeFromCenter (float divergence, Vector2 center)
	{
		float strength = UnityEngine.Random.Range (0, divergence);
		Vector2 direction = new Vector2 (UnityEngine.Random.Range (0.0f, 1.0f), UnityEngine.Random.Range (0.0f, 1.0f));
		return center + new Vector2 (Mathf.FloorToInt (strength * direction.x), Mathf.FloorToInt (strength * direction.y));
	}

	Color FindArchitectureStyleColor (ArchitectureStyle architectureStyle)
	{
		foreach (ArchitectureStyleColorMapping architectureStyleColorMapping in _architectureStyleColorMappings) {
			if (architectureStyleColorMapping.architectureStyleName == architectureStyle.name) {
				return architectureStyleColorMapping.color;
			}
		}
		
		// default color
		return Color.white;
	}
	
	/*void SaveArchitectureStyleMapToDisk (int width, int height)
	{
		Texture2D texture = new Texture2D (width, height, TextureFormat.RGBA32, false);
		int size = width * height;
		Color[] pixels = new Color[size];
		for (int i = 0; i < size; i++) {
			pixels [i] = FindArchitectureStyleColor (architecturalStyleMap [i]);
		}
		texture.SetPixels (pixels);
		texture.Apply ();
		System.IO.File.WriteAllBytes ("Assets/" + _mapsFolder + "/arch_styles.png", texture.EncodeToPNG ());
	}*/
	
	public override void Execute (RoadNetworkParameters roadNetworkParameters, ArchitectureStyle[] _architectureStyle)
	{
        int worldWidth = roadNetworkParameters.Width;
        int worldHeight = roadNetworkParameters.Height;
        int halfWorldWidth = worldWidth / 2;
		int halfWorldHeight = roadNetworkParameters.Height / 2;
		
		Vector2 center = new Vector3 (halfWorldWidth, halfWorldHeight);
		
		Vector2 cityCenter = DivergeFromCenter (_cityCenterDivergence, center);
		Vector2 groundZero = DivergeFromCenter (_groundZeroDivergence, center);

		int mapSize = worldWidth * worldHeight;

		// doesn't use
		_elevationMap = null;
		_destructionMap = new Vector2[mapSize];
		// doesn't use
		_populationMap = null;
		_architecturalStyleMap = new ArchitectureStyle[mapSize];
		
		// FIXME:
		float maxRadius = Mathf.Sqrt (Mathf.Pow (worldWidth, 2.0f) + Mathf.Pow (worldHeight, 2.0f)) / 2.0f;
		maxRadius *= 0.8f;
		
		//float maxDistance = Mathf.Sqrt (Mathf.Pow (halfWidth, 2.0f) + Mathf.Pow (halfHeight, 2.0f));
		for (int y = 0, i = 0; y < worldHeight; y++) {
			for (int x = 0; x < worldWidth; x++, i++) {
				Vector2 position = new Vector2 (x, y);
				_destructionMap [i] = groundZero - position;
				int architectureStyleIndex = Mathf.RoundToInt (Mathf.SmoothStep (0.0f, (float)_architectureStyle.Length - 1.0f, Vector2.Distance (position, cityCenter) / maxRadius));
				_architecturalStyleMap [i] = _architectureStyle [architectureStyleIndex];
			}
		}
		
		/*if (_saveMapsToDisk) {
			SaveArchitectureStyleMapToDisk (worldParameters.width, worldParameters.height);
		}*/
	}

}