using System;
using UnityEngine;

[Serializable]
public class PlayerGetRoomInfo : ClientMessage
{
    public PlayerGetRoomInfo() : base(NetworkMessageTypes.Client.Pregame.GetRoomInfo) { }
}
