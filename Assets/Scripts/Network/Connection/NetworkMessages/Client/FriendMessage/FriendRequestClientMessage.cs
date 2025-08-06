using System;

[Serializable]
public class FriendRequestClientMessage : ClientMessage
{
    [FieldOrder(0)] public string Id;
    public FriendRequestClientMessage(string id)
        : base(NetworkMessageTypes.Client.FriendSystem.SendFriendRequest)
    {
        Id = id;
    }
}