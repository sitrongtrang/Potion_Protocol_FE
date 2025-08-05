using UnityEngine;

[SerializeField]
public class ServerGetRoomByName : ServerMessage
{
    public RoomInfo[] Room;
    public ServerGetRoomByName() : base(NetworkMessageTypes.Server.Room.GetRoomByName) { }
}