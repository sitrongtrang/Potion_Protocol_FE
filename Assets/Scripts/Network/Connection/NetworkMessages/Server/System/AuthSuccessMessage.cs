using System;

[Serializable]
public class AuthSuccessMessage : ServerMessage
{
    // public string ClientId;
    // public string Response;
    [FieldOrder(0)]
    public string ReconnectToken;
    public AuthSuccessMessage() : base(NetworkMessageTypes.Server.System.AuthSuccess) { }
}