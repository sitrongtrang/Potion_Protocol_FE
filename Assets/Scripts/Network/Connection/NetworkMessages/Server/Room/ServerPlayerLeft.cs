using UnityEngine;

[SerializeField]
public class ServerPlayerLeft : ServerMessage
{
    public ServerPlayerLeft() : base(NetworkMessageTypes.Server.Room.PlayerLeft) { }
}
