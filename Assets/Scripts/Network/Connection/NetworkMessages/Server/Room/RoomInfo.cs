public class RoomInfo
{
    [FieldOrder(0)]
    public string RoomID;
    [FieldOrder(1)]
    public string RoomName;
    [FieldOrder(2)]
    public short GameMode;
    [FieldOrder(3)]
    public short RoomType;
    [FieldOrder(4)]
    public int MaxPlayers;
    [FieldOrder(5)]
    public int CurrentPlayers;
    public PlayerInfo[] PlayerList;
}

public class PlayerInfo
{
    [FieldOrder(6)]
    public string PlayerID;
    [FieldOrder(7)]
    public string PlayerDisPlayName;
    [FieldOrder(8)]
    public int level;
    [FieldOrder(9)]
    public short PlayerRole;
    [FieldOrder(10)]
    public short PlayerStatus;
}