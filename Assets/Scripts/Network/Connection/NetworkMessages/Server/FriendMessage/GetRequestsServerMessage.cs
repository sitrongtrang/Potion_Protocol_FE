using System;

[Serializable]
public class GetRequestsServerMessage : ServerMessage
{
    public GetRequestsServerMessage()
        : base(NetworkMessageTypes.Client.FriendSystem.RemoveFriend)
    {
        
    }
}