using System;
using UnityEngine;

[Serializable]
public class LogoutRequest : ClientMessage
{
    public LogoutRequest() : base(NetworkMessageTypes.Client.Authentication.LogOut) { }
}
