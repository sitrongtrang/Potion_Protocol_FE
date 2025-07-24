using System;

[Serializable]
public class PongMessage : ServerMessage
{
    public long ClientSendTime;
    public long ServerReceiveTime;

    public PongMessage() : base(NetworkMessageTypes.Server.System.Pong) { }
}