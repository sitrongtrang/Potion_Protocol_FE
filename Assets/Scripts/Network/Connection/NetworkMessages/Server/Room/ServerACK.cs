using UnityEngine;

[SerializeField]
public class ServerACK : ServerMessage
{
    [FieldOrder(0)]
    public short clientMessageType;
    public ServerACK() : base(NetworkMessageTypes.Server.Room.ACK) { }
}
