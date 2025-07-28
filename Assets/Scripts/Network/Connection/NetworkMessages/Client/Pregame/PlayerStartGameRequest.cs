using System;
using UnityEngine;

[Serializable]
public class PlayerStartGameRequest : ClientMessage
{
    public PlayerStartGameRequest() : base(NetworkMessageTypes.Client.Pregame.StartGame) { }
}