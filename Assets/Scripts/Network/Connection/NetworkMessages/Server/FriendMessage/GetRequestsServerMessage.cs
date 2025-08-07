using System;
using System.Collections.Generic;

[Serializable]
public class GetRequestsServerMessage : ServerMessage
{
    [FieldOrder(0)]
    public List<Friend> FriendList;
    
    public GetRequestsServerMessage()
        : base(NetworkMessageTypes.Server.FriendSystem.GetFriendRequests)
    {

    }
}