using System;

[Serializable]
public class RemoveMyRequestMessage : ClientMessage
{
    [FieldOrder(0)] public string Id;
    public RemoveMyRequestMessage(string id)
        : base(NetworkMessageTypes.Client.FriendSystem.RemoveMyRequest)
    {
        Id = id;
    }
}