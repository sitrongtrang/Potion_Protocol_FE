using System;

[Serializable]
public class PlayerSpawnMessage : ServerMessage
{
    public string PlayerId;
    public float PositionX;
    public float PositionY;
    
    public PlayerSpawnMessage() : base(NetworkMessageTypes.Server.Player.Spawn) { }
}