using UnityEngine;

[SerializeField]
public class ServerGetAllRoom : ServerMessage
{
    [FieldOrder(0)]
    public RoomInfo []Room;
    public ServerGetAllRoom() : base(NetworkMessageTypes.Server.Room.GetAllRoom) { }
}
