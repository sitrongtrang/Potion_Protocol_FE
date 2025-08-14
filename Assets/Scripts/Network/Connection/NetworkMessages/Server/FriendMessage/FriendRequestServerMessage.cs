using System;
using System.Collections.Generic;

[Serializable]
public class FriendRequestServerMessage : ServerMessage
{
    public FriendRequestServerMessage()
        : base(NetworkMessageTypes.Server.FriendSystem.SendFriendRequest)
    {

    }
}