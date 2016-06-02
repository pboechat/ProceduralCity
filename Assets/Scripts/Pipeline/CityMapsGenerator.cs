using UnityEngine;

public abstract class CityMapsGenerator : MonoBehaviour
{
	protected float[] _elevationMap;
	protected float[] _populationMap;
	protected ArchitectureStyle[] _architecturalStyleMap;
	protected Vector2[] _destructionMap;

	public ArchitectureStyle[] architecturalStyleMap {
		get {
			return this._architecturalStyleMap;
		}
	}

	public Vector2[] destructionMap {
		get {
			return this._destructionMap;
		}
	}

	public float[] populationMap {
		get {
			return this._populationMap;
		}
	}

	public float[] elevationMap {
		get {
			return this._elevationMap;
		}
	}
	
	public abstract void Execute (RoadNetworkParameters roadNetworkParameters, ArchitectureStyle[] _architectureStyle);
	
}
