using UnityEngine;

[SerializeField]
public class ServerPlayerUnReady : ServerMessage
{
    public ServerPlayerUnReady() : base(NetworkMessageTypes.Server.Room.UnReady) { }
}
