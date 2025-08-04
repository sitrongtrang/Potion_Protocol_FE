using UnityEngine;

[SerializeField]
public class ServerGetRoomByID : ServerMessage
{
    public RoomInfo Room;
    public ServerGetRoomByID() : base(NetworkMessageTypes.Server.Room.GetRoomByID) { }
}