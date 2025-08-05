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
    [FieldOrder(6)]
    public PlayerInfo[] PlayerList;
    [FieldOrder(7)]
    public int MapID;
}

public class PlayerInfo
{
    [FieldOrder(0)]
    public string PlayerID;
    [FieldOrder(1)]
    public string PlayerDisPlayName;
    [FieldOrder(2)]
    public int level;
    [FieldOrder(3)]
    public short PlayerRole;
    [FieldOrder(4)]
    public short PlayerStatus;
}