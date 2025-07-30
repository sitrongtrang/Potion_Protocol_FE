using System;

[Serializable]
public class GameStatesUpdate : ServerMessage
{
    public GameStateUpdate[] GameStates;
    public GameStatesUpdate() : base(NetworkMessageTypes.Server.GameState.StateUpdate) { }

}

[Serializable]
public class GameStateUpdate : IServerStateSnapshot
{
    [FieldOrder(0)]
    public int ServerSequence;
    [FieldOrder(1)]
    public PlayerState[] PlayerStates;
    [FieldOrder(2)]
    public StationState[] StationStates;
    [FieldOrder(3)]
    public AlchemyState AlchemyStte;
    [FieldOrder(4)]
    public EnemyState[] EnemyStates;
    [FieldOrder(5)]
    public PlantState[] PlantStates;
    [FieldOrder(6)]
    public OreState[] OreStates;
    // [FieldOrder(7)]
    // public ItemState[] ItemStates;
    [FieldOrder(7)]
    public int CurrentServerTime;
    //     public int CurrentScore;
    int IServerStateSnapshot.ServerSequence => ServerSequence;
}

