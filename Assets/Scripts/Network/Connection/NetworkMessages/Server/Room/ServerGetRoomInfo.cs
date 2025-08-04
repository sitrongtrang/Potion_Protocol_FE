using UnityEngine;

[SerializeField]
public class ServerGetRoomInfo : ServerMessage
{
    [FieldOrder(0)]
    public RoomInfo Room;
    public ServerGetRoomInfo() : base(NetworkMessageTypes.Server.Room.GetRoomInfo) { }
}
