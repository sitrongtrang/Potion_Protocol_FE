using UnityEngine;

[SerializeField]
public class ServerGetRoomInfo : ServerMessage
{
    RoomInfo[] Room;
    public ServerGetRoomInfo() : base(NetworkMessageTypes.Server.Room.GetRoomInfo) { }
}
