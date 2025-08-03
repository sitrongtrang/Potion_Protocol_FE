using System;

[Serializable]
public class FriendRemoveServerMessage : ServerMessage
{
    public FriendRemoveServerMessage()
        : base(NetworkMessageTypes.Client.FriendSystem.RemoveFriend)
    {
        
    }
}