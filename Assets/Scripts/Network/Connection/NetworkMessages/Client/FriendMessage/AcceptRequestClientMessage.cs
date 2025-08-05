using System;

[Serializable]
public class AcceptRequestClientMessage : ClientMessage
{
    [FieldOrder(0)] public string Id;
    public AcceptRequestClientMessage(string id)
        : base(NetworkMessageTypes.Client.FriendSystem.RemoveFriend)
    {
        Id = id;
    }
}