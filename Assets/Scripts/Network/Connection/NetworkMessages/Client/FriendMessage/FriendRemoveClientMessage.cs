using System;

[Serializable]
public class FriendRemoveClientMessage : ClientMessage
{
    [FieldOrder(0)] public string Id;
    public FriendRemoveClientMessage(string id)
        : base(NetworkMessageTypes.Client.FriendSystem.RemoveFriend)
    {
        Id = id;
    }
}