using UnityEngine;

[SerializeField]
public class ServerPlayerReady : ServerMessage
{
    [FieldOrder(0)]
    public string UserID;
    [FieldOrder(1)]
    public string UserDisplayName;
    public ServerPlayerReady() : base(NetworkMessageTypes.Server.Room.Ready) { }
}
