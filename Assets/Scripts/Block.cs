using UnityEngine;
using System;
using System.Collections.Generic;

public struct Block
{
    public Vector2 center;
    public int width;
    public int depth;
    private Dictionary<ArchitectureStyle, float> _architectureStylesProbabilities;

    public Block(Vector2 center, int width, int depth, List<ArchitectureStyle> architectureStylesSamples)
    {
        this.center = center;
        this.width = width;
        this.depth = depth;

        _architectureStylesProbabilities = new Dictionary<ArchitectureStyle, float>();
        float u = 1.0f / architectureStylesSamples.Count;
        foreach (ArchitectureStyle architectureStyleSample in architectureStylesSamples)
        {
            if (_architectureStylesProbabilities.ContainsKey(architectureStyleSample))
            {
                _architectureStylesProbabilities[architectureStyleSample] += u;
            }
            else {
                _architectureStylesProbabilities.Add(architectureStyleSample, u);
            }
        }
    }

    public ICollection<ArchitectureStyle> possibleArchitectureStyles
    {
        get
        {
            return _architectureStylesProbabilities.Keys;
        }
    }

    public Dictionary<ArchitectureStyle, float> architectureStylesProbabilities
    {
        get
        {
            return _architectureStylesProbabilities;
        }
    }

    public ArchitectureStyle randomArchitectureStyle
    {
        get
        {
            float chance = UnityEngine.Random.value;
            float accumulatedChance = 0.0f;
            foreach (KeyValuePair<ArchitectureStyle, float> architectureStylePercentage in _architectureStylesProbabilities)
            {
                accumulatedChance += architectureStylePercentage.Value;
                if (chance < accumulatedChance)
                {
                    return architectureStylePercentage.Key;
                }
            }
            // FIXME:
            throw new Exception("error");
        }
    }
}
