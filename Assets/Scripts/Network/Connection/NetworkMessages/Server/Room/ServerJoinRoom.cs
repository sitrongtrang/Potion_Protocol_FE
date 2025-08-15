using System;
using UnityEngine;

[Serializable]
public class ServerJoinRoom : ServerMessage
{
    [FieldOrder(0)]
    public string PlayerID;
    [FieldOrder(1)]
    public string PlayerDisplayName;

    public ServerJoinRoom() : base(NetworkMessageTypes.Server.Room.PlayerJoined) { }
}
