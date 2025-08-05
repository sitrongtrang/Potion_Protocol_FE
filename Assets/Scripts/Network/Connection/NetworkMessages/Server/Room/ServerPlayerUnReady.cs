using UnityEngine;

[SerializeField]
public class ServerPlayerUnReady : ServerMessage
{
    [FieldOrder(0)]
    public string UserID;
    [FieldOrder(1)]
    public string UserDisplayName;
    public ServerPlayerUnReady() : base(NetworkMessageTypes.Server.Room.UnReady) { }
}
