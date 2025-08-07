using UnityEngine;

[SerializeField]
public class ServerGetRoomByName : ServerMessage
{
    [FieldOrder(0)]
    public RoomInfo[] Room;
    public ServerGetRoomByName() : base(NetworkMessageTypes.Server.Room.GetRoomByName) { }
}