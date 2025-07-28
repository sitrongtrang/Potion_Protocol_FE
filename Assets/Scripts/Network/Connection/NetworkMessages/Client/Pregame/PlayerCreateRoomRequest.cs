using System;
using UnityEngine;

[Serializable]
public class PlayerCreateRoomRequest : ClientMessage
{
    [FieldOrder(0)]
    public string RoomName;

    [FieldOrder(1)]
    public short RoomType;

    [FieldOrder(2)]
    public int MaxPlayers;
    public PlayerCreateRoomRequest() : base(NetworkMessageTypes.Client.Pregame.CreateRoom) { }
}