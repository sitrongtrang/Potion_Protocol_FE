using System;

[Serializable]
public class DeclineRequestServerMessage : ServerMessage
{
    public DeclineRequestServerMessage()
        : base(NetworkMessageTypes.Client.FriendSystem.RemoveFriend)
    {
        
    }
}