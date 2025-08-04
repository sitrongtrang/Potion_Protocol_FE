using UnityEngine;

[SerializeField]
public class ServerGetAllRoom : ServerMessage
{
    public RoomInfo []Room;
    public ServerGetAllRoom() : base(NetworkMessageTypes.Server.Room.GetAllRoom) { }
}
