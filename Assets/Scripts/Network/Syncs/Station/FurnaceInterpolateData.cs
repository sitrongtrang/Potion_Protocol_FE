using System;
using UnityEngine;

public class FurnaceStateInterpolateData : IServerStateSnapshot, IComparable<FurnaceStateInterpolateData>
{
    public int ServerSequence;
    public float CraftTime;
    public float CraftMaxTime;
    public bool IsCrafting;

    int IServerStateSnapshot.ServerSequence => ServerSequence;
    
    public int CompareTo(FurnaceStateInterpolateData other)
    {
        if (other == null) return 1;
        return ServerSequence.CompareTo(other.ServerSequence);
    }
}