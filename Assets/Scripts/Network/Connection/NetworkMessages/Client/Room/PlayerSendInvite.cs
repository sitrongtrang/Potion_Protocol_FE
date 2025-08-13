using System;
using UnityEngine;

[Serializable]
public class PlayerSendInvite : ClientMessage
{
    public PlayerSendInvite() : base(NetworkMessageTypes.Client.Pregame.SendRoomInvite) { }
}
