using UnityEngine;

public class ServerCreateRoom : ServerMessage
{
    [FieldOrder(0)]
    public string RoomID;
    [FieldOrder(1)]
    public string UserID;
    public ServerCreateRoom() : base(NetworkMessageTypes.Server.Room.CreateRoom) { }
}
