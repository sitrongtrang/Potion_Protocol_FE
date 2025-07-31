
using System;

[Serializable]
public class FriendListClientMessage : ClientMessage
{
    [FieldOrder(0)]
    public string UserId; // vẫn là string để BinarySerializer xử lý đúng

    public FriendListClientMessage(Guid userId)
        : base(NetworkMessageTypes.Client.FriendSystem.GetFriendList)
    {
        // Convert GUID thành 16 byte, rồi base64 để giữ đúng byte khi serialize
        UserId = Convert.ToBase64String(userId.ToByteArray());
    }
}