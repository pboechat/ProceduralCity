using UnityEngine;

public class RadialDistributionCityMapsGenerator : CityMapsGenerator
{
    [SerializeField]
    private float _cityCenterDivergence;

    static Vector2 DivergeFromCenter(float divergence, Vector2 center)
    {
        float strength = UnityEngine.Random.Range(0, divergence);
        Vector2 direction = new Vector2(UnityEngine.Random.Range(0.0f, 1.0f), UnityEngine.Random.Range(0.0f, 1.0f));
        return center + new Vector2(Mathf.FloorToInt(strength * direction.x), Mathf.FloorToInt(strength * direction.y));
    }

    public override void Execute(RoadNetworkParameters roadNetworkParameters, ArchitectureStyle[] architectureStyles)
    {
        int worldWidth = roadNetworkParameters.Width;
        int worldHeight = roadNetworkParameters.Height;
        int halfWorldWidth = worldWidth / 2;
        int halfWorldHeight = roadNetworkParameters.Height / 2;

        Vector2 center = new Vector3(halfWorldWidth, halfWorldHeight);

        Vector2 cityCenter = DivergeFromCenter(_cityCenterDivergence, center);

        int mapSize = worldWidth * worldHeight;

        // TODO:
        _elevationMap = null;
        _populationMap = null;

        _architecturalStyleMap = new int[mapSize];

        float maxRadius = Mathf.Sqrt(Mathf.Pow(worldWidth, 2.0f) + Mathf.Pow(worldHeight, 2.0f)) / 4.0f;

        for (int y = 0, i = 0; y < worldHeight; y++)
        {
            for (int x = 0; x < worldWidth; x++, i++)
            {
                Vector2 position = new Vector2(x, y);
                _architecturalStyleMap[i] = Mathf.RoundToInt(Mathf.Lerp(0.0f, (float)architectureStyles.Length - 1.0f, Vector2.Distance(position, cityCenter) / maxRadius));
            }
        }
    }

}