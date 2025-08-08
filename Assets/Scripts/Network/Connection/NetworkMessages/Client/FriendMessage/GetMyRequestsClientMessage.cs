using System;

[Serializable]
public class GetMyRequestsClientMessage : ClientMessage
{
    public GetMyRequestsClientMessage()
        : base(NetworkMessageTypes.Client.FriendSystem.GetMyRequests)
    {
        
    }
}