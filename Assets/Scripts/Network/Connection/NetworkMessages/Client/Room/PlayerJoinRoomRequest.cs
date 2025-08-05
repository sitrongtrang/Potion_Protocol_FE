using System;
using UnityEngine;

[Serializable]
public class PlayerJoinRoomRequest : ClientMessage
{
    [FieldOrder(0)]
    public string RoomId;
    [FieldOrder(1)]
    public string Password;
    public PlayerJoinRoomRequest() : base(NetworkMessageTypes.Client.Pregame.JoinRoom) { }
}