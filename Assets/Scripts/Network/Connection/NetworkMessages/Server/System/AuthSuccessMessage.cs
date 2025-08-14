using System;

[Serializable]
public class AuthSuccessMessage : ServerMessage
{
    [FieldOrder(0)] public string ReconnectToken;
    public AuthSuccessMessage() : base(NetworkMessageTypes.Server.System.AuthSuccess) { }
}