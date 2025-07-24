using System;

[Serializable]
public class ReconnectMessage : ClientMessage
{
    public string Token;
    public string SessionToken;
    public ReconnectMessage() : base(NetworkMessageTypes.Client.Authentication.TryReconnect) { }
}