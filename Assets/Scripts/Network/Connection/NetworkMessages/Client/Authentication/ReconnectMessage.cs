using System;

[Serializable]
public class ReconnectMessage : ClientMessage
{
    [FieldOrder(0)]
    public string Token;
    [FieldOrder(1)]
    public string SessionToken;
    public ReconnectMessage() : base(NetworkMessageTypes.Client.Authentication.TryReconnect) { }
}