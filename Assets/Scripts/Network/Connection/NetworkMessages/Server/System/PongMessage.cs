using System;

[Serializable]
public class PongMessage : ServerMessage
{
    [FieldOrder(0)]
    public long ClientSendTime;
    [FieldOrder(1)]
    public long ServerReceiveTime;

    public PongMessage() : base(NetworkMessageTypes.Server.System.Pong) { }
}