using System;

[Serializable]
public class DeclineRequestClientMessage : ClientMessage
{
    public string Id;
    public DeclineRequestClientMessage(string id)
        : base(NetworkMessageTypes.Client.FriendSystem.RemoveFriend)
    {
        Id = id;
    }
}