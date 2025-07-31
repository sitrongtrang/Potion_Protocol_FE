using UnityEngine;

[SerializeField]
public class ServerRoomFull : ServerMessage
{
    public ServerRoomFull() : base(NetworkMessageTypes.Server.Room.RoomFull) { }
}
