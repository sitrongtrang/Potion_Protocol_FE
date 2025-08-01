using System;

[Serializable]
public class AcceptRequestClientMessage : ClientMessage
{
    public string Id;
    public AcceptRequestClientMessage(string id)
        : base(NetworkMessageTypes.Client.FriendSystem.RemoveFriend)
    {
        Id = id;
    }
}