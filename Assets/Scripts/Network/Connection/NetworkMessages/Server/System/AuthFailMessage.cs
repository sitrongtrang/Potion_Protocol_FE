using System;

[Serializable]
public class AuthFailMessage : ServerMessage
{
    [FieldOrder(0)] public string Response;
    public AuthFailMessage() : base(NetworkMessageTypes.Server.System.AuthFail) { }
}