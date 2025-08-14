using System;

[Serializable]
public class AcceptRequestServerMessage : ServerMessage
{
    public AcceptRequestServerMessage()
        : base(NetworkMessageTypes.Server.FriendSystem.AcceptFriendRequest)
    {

    }
}