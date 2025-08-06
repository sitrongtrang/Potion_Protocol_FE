using System;
using System.Collections.Generic;
using UnityEngine;

public class GameStateInterpolateData : IServerStateSnapshot, IComparable<GameStateInterpolateData>
{
    public int ServerSequence;
    int IServerStateSnapshot.ServerSequence => ServerSequence;

    [Serializable]
    public class EntityInfo
    {
        public string TypeId;
        public Vector2 Position;
    }

    // Ingame Id -> Type Id
    public Dictionary<string, EntityInfo> ItemIds;
    public Dictionary<string, EntityInfo> EnemyIds;
    public Dictionary<string, EntityInfo> ItemSourceIds;
    public Dictionary<string, EntityInfo> StationIds;

    public int CompareTo(GameStateInterpolateData other)
    {
        if (other == null) return 1;
        return ServerSequence.CompareTo(other.ServerSequence);
    }
}