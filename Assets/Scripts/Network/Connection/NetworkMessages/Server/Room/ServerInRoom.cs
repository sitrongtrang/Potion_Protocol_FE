using UnityEngine;

[SerializeField]
public class ServerInRoom : ServerMessage
{
    public ServerInRoom() : base(NetworkMessageTypes.Server.Room.PlayerJoined) { }
}
