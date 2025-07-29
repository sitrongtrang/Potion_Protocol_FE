using System;

public class GameStateInterpolateData : IServerStateSnapshot, IComparable<PlayerStateInterpolateData>
{
    public int ServerSequence;
    int IServerStateSnapshot.ServerSequence => ServerSequence;
    public string[] PlayerIds;
    public string[] ItemIds;
    public string[] EnemyIds;
    public string[] ItemSourceIds;
    public string[] StationIds;

    public int CompareTo(PlayerStateInterpolateData other)
    {
        if (other == null) return 1;
        return ServerSequence.CompareTo(other.ServerSequence);
    }
}