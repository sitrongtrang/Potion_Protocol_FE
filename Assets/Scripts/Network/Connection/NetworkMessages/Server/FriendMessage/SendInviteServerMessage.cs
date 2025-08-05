// using System;

// [Serializable]
// public class SendInviteServerMessage : ServerMessage
// {
//     [FieldOrder(0)] public string id;
//     public SendInviteServerMessage()
//         : base(NetworkMessageTypes.Client.FriendSystem.InviteFriend)
//     {
//         // Convert GUID thành 16 byte, rồi base64 để giữ đúng byte khi serialize

//     }
// }