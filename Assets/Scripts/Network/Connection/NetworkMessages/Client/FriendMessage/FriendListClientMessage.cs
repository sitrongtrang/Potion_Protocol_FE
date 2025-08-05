using System;

[Serializable]
public class FriendListClientMessage : ClientMessage
{
    
    public FriendListClientMessage()
        : base(NetworkMessageTypes.Client.FriendSystem.GetFriendList)
    {
        // Convert GUID thành 16 byte, rồi base64 để giữ đúng byte khi serialize
        
    }
}