using UnityEngine;

public class RandomCityMapsGenerator : CityMapsGenerator
{
    public override void Execute(RoadNetworkParameters roadNetworkParameters, ArchitectureStyle[] allArchitectureStyles)
    {
        int worldWidth = roadNetworkParameters.Width;
        int worldHeight = roadNetworkParameters.Height;
        int halfWorldWidth = worldWidth / 2;
        int halfWorldHeight = roadNetworkParameters.Height / 2;

        Vector2 center = new Vector3(halfWorldWidth, halfWorldHeight);

        int mapSize = worldWidth * worldHeight;

        // TODO:
        _elevationMap = null;
        _populationMap = null;

        _architecturalStyleMap = new int[mapSize];

        for (int y = 0, i = 0; y < worldHeight; y++)
            for (int x = 0; x < worldWidth; x++, i++)
                _architecturalStyleMap[i] = UnityEngine.Random.Range(0, allArchitectureStyles.Length);
    }

}