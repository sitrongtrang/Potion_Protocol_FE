// using System;

// [Serializable]
// public class SendInviteClientMessage : ClientMessage
// {
//     [FieldOrder(0)] public string id;
//     public SendInviteClientMessage()
//         : base(NetworkMessageTypes.Client.FriendSystem.InviteFriend)
//     {
//         // Convert GUID thành 16 byte, rồi base64 để giữ đúng byte khi serialize

//     }
// }