using System;

[Serializable]
public class GameStatesUpdate : ServerMessage
{
    public GameStateUpdate[] GameStates;
    public GameStatesUpdate() : base(NetworkMessageTypes.Server.GameState.StateUpdate) { }

}

[Serializable]
public class GameStateUpdate : IStateSnapshot, IServerStateSnapshot
{
    [FieldOrder(0)]
    public int ServerSequence;
    [FieldOrder(1)]
    public int ProcessedInputSequence;
    [FieldOrder(2)]
    public PlayerState[] PlayerStates;
    [FieldOrder(3)]
    public StationState[] StationStates;
    [FieldOrder(4)]
    public OreState[] OreStates;
    [FieldOrder(5)]
    public PlantState[] PlantStates;
    [FieldOrder(6)]
    public EnemyState[] EnemyStates;
    [FieldOrder(7)]
    public ItemState[] ItemStates;
    [FieldOrder(8)]
    public int CurrentGameTime;
    //     public int CurrentScore;

    int IStateSnapshot.ProcessedInputSequence => ProcessedInputSequence;
    int IServerStateSnapshot.ServerSequence => ServerSequence;
}

