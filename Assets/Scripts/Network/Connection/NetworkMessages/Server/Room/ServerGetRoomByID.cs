using UnityEngine;

[SerializeField]
public class ServerGetRoomByID : ServerMessage
{
    RoomInfo Room;
    public ServerGetRoomByID() : base(NetworkMessageTypes.Server.Room.GetRoomByID) { }
}