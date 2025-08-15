using System;
using UnityEngine;

[Serializable]
public class ServerGetRoomByName : ServerMessage
{
    [FieldOrder(0)]
    public RoomInfo[] Room;
    public ServerGetRoomByName() : base(NetworkMessageTypes.Server.Room.GetRoomByName) { }
}