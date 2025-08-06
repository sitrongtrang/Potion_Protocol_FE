using System;

[Serializable]
public class AcceptRequestServerMessage : ServerMessage
{
    public AcceptRequestServerMessage()
        : base(NetworkMessageTypes.Client.FriendSystem.RemoveFriend)
    {

    }
}