using System;
using UnityEngine;

[Serializable]
public class PlayerReady : ClientMessage
{
    public PlayerReady() : base(NetworkMessageTypes.Client.Pregame.Ready) { }
}
