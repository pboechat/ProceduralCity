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
        float baseChance = 1.0f / architectureStylesSamples.Count;
        foreach (ArchitectureStyle architectureStyleSample in architectureStylesSamples)
        {
            if (_architectureStylesProbabilities.ContainsKey(architectureStyleSample))
                _architectureStylesProbabilities[architectureStyleSample] += baseChance;
            else
                _architectureStylesProbabilities.Add(architectureStyleSample, baseChance);
        }
    }

    public ICollection<ArchitectureStyle> architectureStyles
    {
        get
        {
            return _architectureStylesProbabilities.Keys;
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
            // should never get here!
            return null;
        }
    }
}
