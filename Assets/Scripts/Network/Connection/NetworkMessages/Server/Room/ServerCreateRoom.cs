using UnityEngine;

public class ServerCreateRoom : ServerMessage
{
    [FieldOrder(0)]
    public string RoomID;
    public ServerCreateRoom() : base(NetworkMessageTypes.Server.Room.CreateRoom) { }
}
