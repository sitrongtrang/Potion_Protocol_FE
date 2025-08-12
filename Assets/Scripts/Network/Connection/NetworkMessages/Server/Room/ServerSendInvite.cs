using UnityEngine;

[SerializeField]
public class ServerSendInvite : ServerMessage
{
    [FieldOrder(0)]
    public string RoomId;
    [FieldOrder(1)]
    public string RequesterDisplayName;
    public ServerSendInvite() : base(NetworkMessageTypes.Server.Room.SendRoomInvite) { }
}
