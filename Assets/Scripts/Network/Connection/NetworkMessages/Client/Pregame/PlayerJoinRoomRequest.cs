using System;
using UnityEngine;

[Serializable]
public class PlayerJoinRoomRequest : ClientMessage
{
    [FieldOrder(0)]
    public string RoomId;
    public PlayerJoinRoomRequest() : base(NetworkMessageTypes.Client.Pregame.JoinRoom) { }
}