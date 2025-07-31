using UnityEngine;

[SerializeField]
public class ServerGetAllRoom : ServerMessage
{
    RoomInfo []Room;
    public ServerGetAllRoom() : base(NetworkMessageTypes.Server.Room.GetAllRoom) { }
}
