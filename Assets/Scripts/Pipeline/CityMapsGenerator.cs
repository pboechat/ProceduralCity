using UnityEngine;

public abstract class CityMapsGenerator : MonoBehaviour
{
    protected float[] _elevationMap;
    protected float[] _populationMap;
    protected int[] _architecturalStyleMap;

    public int[] architecturalStylesMap
    {
        get
        {
            return this._architecturalStyleMap;
        }
    }

    public float[] populationMap
    {
        get
        {
            return this._populationMap;
        }
    }

    public float[] elevationMap
    {
        get
        {
            return this._elevationMap;
        }
    }

    public abstract void Execute(RoadNetworkParameters roadNetworkParameters, ArchitectureStyle[] allArchitectureStyles);

}
