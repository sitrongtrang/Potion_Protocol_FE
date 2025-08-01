using System;

[Serializable]
public class FriendRequestClientMessage : ClientMessage
{
    public string Id;
    public FriendRequestClientMessage(string id)
        : base(NetworkMessageTypes.Client.FriendSystem.RemoveFriend)
    {
        Id = id;
    }
}