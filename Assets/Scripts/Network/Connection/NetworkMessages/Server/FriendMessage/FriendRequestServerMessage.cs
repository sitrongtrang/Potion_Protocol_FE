using System;

[Serializable]
public class FriendRequestServerMessage : ServerMessage
{
    public FriendRequestServerMessage()
        : base(NetworkMessageTypes.Client.FriendSystem.RemoveFriend)
    {
        
    }
}