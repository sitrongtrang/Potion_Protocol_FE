
using System;
using System.Collections.Generic;

[Serializable]
public class Friend
{
    [FieldOrder(0)] public string Id;
    [FieldOrder(1)] public string FriendId;
    [FieldOrder(2)] public string FriendDisplayName;
    [FieldOrder(3)] public bool IsOnline;
}

[Serializable]
public class FriendListServerMessage : ServerMessage
{
    [FieldOrder(0)]
    public List<Friend> FriendList;

    public FriendListServerMessage() : base(NetworkMessageTypes.Server.FriendSystem.GetFriendList) { }
}