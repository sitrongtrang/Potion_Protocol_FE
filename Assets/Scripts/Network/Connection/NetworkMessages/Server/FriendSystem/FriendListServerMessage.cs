
using System;
using System.Collections.Generic;

[Serializable]
public class Friend
{
    [FieldOrder(0)] public string id;
    [FieldOrder(1)] public string friendId;
    [FieldOrder(2)] public string friendDisplayName;
}

[Serializable]
public class FriendListServerMessage : ServerMessage
{
    [FieldOrder(0)]
    public List<Friend> friendList;

    public FriendListServerMessage() : base(NetworkMessageTypes.Server.FriendSystem.GetFriendList) { }
}