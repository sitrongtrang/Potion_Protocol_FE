using UnityEngine;

[SerializeField]
public class ServerPlayerReady : ServerMessage
{
    public ServerPlayerReady() : base(NetworkMessageTypes.Server.Room.Ready) { }
}
