using System;

[Serializable]
public class FriendRemoveServerMessage : ServerMessage
{
    [FieldOrder(0)] public string id;
    public FriendRemoveServerMessage()
        : base(NetworkMessageTypes.Client.FriendSystem.RemoveFriend)
    {
        
    }
}