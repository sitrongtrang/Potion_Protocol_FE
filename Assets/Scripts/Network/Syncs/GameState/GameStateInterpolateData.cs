using System;
using System.Collections.Generic;

public class GameStateInterpolateData : IServerStateSnapshot, IComparable<GameStateInterpolateData>
{
    public int ServerSequence;
    int IServerStateSnapshot.ServerSequence => ServerSequence;
    public Dictionary<string, string> PlayerIds; // Ingame Id -> Type Id
    public Dictionary<string, string> ItemIds;
    public Dictionary<string, string> EnemyIds;
    public Dictionary<string, string> ItemSourceIds;
    public Dictionary<string, string> StationIds;

    public int CompareTo(GameStateInterpolateData other)
    {
        if (other == null) return 1;
        return ServerSequence.CompareTo(other.ServerSequence);
    }
}