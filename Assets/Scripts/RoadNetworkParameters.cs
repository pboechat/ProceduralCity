using UnityEngine;
using System;

[Serializable]
public class RoadNetworkParameters
{
	public int gridWidth;
	public int gridHeight;
	public int minCellWidth;
	public int maxCellWidth;
	public int minCellDepth;
	public int maxCellDepth;
	public int roadWidth;
	public int sidewalkWidth;

    public int Width
    {
        get
        {
            return (maxCellWidth + roadWidth) * gridWidth + roadWidth;
        }
    }

    public int Height
    {
        get
        {
            return (maxCellDepth + roadWidth) * gridHeight + roadWidth;
        }
    }

}