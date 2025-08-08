using System;
using System.Collections.Generic;

[Serializable]
public class GetMyRequestsServerMessage : ServerMessage
{
    [FieldOrder(0)]
    public List<Friend> FriendList;
    
    public GetMyRequestsServerMessage()
        : base(NetworkMessageTypes.Server.FriendSystem.GetMyRequests)
    {

    }
}