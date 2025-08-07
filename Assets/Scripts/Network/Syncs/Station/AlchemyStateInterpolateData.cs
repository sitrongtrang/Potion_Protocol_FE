using System;
using UnityEngine;

public class AlchemyStateInterpolateData : IServerStateSnapshot, IComparable<AlchemyStateInterpolateData>
{
    public int ServerSequence;
    public float CraftTime;
    public float CraftMaxTime;
    public bool IsCrafting;
    public string[] ItemTypeIds;

    int IServerStateSnapshot.ServerSequence => ServerSequence;
    
    public int CompareTo(AlchemyStateInterpolateData other)
    {
        if (other == null) return 1;
        return ServerSequence.CompareTo(other.ServerSequence);
    }
}