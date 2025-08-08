using System;

[Serializable]
public class ServerStartGame : ServerMessage
{
    [FieldOrder(0)]
    public PlayerDTO[] PlayerDTOs;
    [FieldOrder(1)]
    public string PlayerId;
    [FieldOrder(2)]
    public string[] PlayerIds;
    [FieldOrder(3)]
    public int Level;
    [FieldOrder(4)]
    public float[] PositionX;
    [FieldOrder(5)]
    public float[] PositionY;
    public ServerStartGame() : base(NetworkMessageTypes.Server.Pregame.StartGame) { }
}

public class PlayerDTO
{
    [FieldOrder(0)]
    public string PlayerId;

    [FieldOrder(1)]
    public string PlayerDisplayName;

    [FieldOrder(2)]
    public int Level;

    [FieldOrder(3)]
    public short PlayerRole;

    [FieldOrder(4)]
    public short PlayerStatus;
}