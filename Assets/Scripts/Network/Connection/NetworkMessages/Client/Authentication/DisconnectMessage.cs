using System;
using UnityEngine;

[Serializable]
public class DisconnectMessage : ClientMessage
{
    public DisconnectMessage() : base(NetworkMessageTypes.Client.Authentication.TryDisconnect) { }
}
