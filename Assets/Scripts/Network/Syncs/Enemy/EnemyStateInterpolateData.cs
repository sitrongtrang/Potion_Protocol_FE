using System;
using UnityEngine;

public class EnemyStateInterpolateData : IServerStateSnapshot, IComparable<EnemyStateInterpolateData>
{
    public int ServerSequence;
    public float PositionX;
    public float PositionY;
    public float Health;

    int IServerStateSnapshot.ServerSequence => ServerSequence;
    
    public int CompareTo(EnemyStateInterpolateData other)
    {
        if (other == null) return 1;
        return ServerSequence.CompareTo(other.ServerSequence);
    }
}