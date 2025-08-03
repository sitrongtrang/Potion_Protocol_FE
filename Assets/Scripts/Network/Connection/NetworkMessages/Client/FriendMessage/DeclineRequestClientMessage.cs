using System;

[Serializable]
public class DeclineRequestClientMessage : ClientMessage
{
    [FieldOrder(0)] public string Id;
    public DeclineRequestClientMessage(string id)
        : base(NetworkMessageTypes.Client.FriendSystem.RemoveFriend)
    {
        Id = id;
    }
}