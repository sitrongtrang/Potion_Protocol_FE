using System;
using UnityEngine;

[Serializable]
public class PlayerUnready : ClientMessage
{
    public PlayerUnready() : base(NetworkMessageTypes.Client.Pregame.Unready) { }
}
