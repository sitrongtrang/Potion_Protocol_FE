using System;

[Serializable]
public class GetRequestsClientMessage : ClientMessage
{
    public GetRequestsClientMessage()
        : base(NetworkMessageTypes.Client.FriendSystem.RemoveFriend)
    {
        
    }
}