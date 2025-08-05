using UnityEngine;

[SerializeField]
public class ServerPlayerLeft : ServerMessage
{
    [FieldOrder(0)]
    public string UserID;
    [FieldOrder(1)]
    public string userDisplayName;
    public ServerPlayerLeft() : base(NetworkMessageTypes.Server.Room.PlayerLeft) { }
}
