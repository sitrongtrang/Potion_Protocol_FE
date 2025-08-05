using System;
using UnityEngine;

[Serializable]
public class PlayerCreateRoomRequest : ClientMessage
{
    [FieldOrder(0)]
    public string RoomName;

    [FieldOrder(1)]
    public short GameMode;

    [FieldOrder(2)]
    public short RoomType;
    [FieldOrder(3)]
    public string Password;
    [FieldOrder(4)]
    public int MapID;
    public PlayerCreateRoomRequest() : base(NetworkMessageTypes.Client.Pregame.CreateRoom) { }
}