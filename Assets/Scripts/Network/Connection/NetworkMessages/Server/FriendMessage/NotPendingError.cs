using System;

[Serializable]
public class NotPendingRequest : ServerMessage
{
    public NotPendingRequest()
        : base(NetworkMessageTypes.Server.FriendSystem.NotPendingRequest)
    {

    }
}