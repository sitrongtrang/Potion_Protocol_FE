using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using Newtonsoft.Json;


public static class Serialization
{
    #region Core

    /// <summary>
    /// Serialize a NetworkMessage into the binary format expected by the Java server.
    /// </summary>
    public static byte[] SerializeMessage(ClientMessage message)
    {
        try
        {
            byte[] payloadBytes = CreateByteFromType(message);

            short messageLength = (short)(2 + payloadBytes.Length);
            //Debug.Log(messageLength);
            using MemoryStream stream = new();
            using BinaryWriter writer = new(stream);

            BinarySerializer.WriteInt16BigEndian(writer, messageLength);
            BinarySerializer.WriteInt16BigEndian(writer, message.MessageType);
            writer.Write(payloadBytes);
            //Debug.Log(payloadBytes);
            return stream.ToArray();
        }
        catch (Exception e)
        {
            Debug.LogError($"[Serialization Error] {e.Message} {message.MessageType}");
            return null;
        }
    }

    private static byte[] CreateByteFromType(ClientMessage message)
    {
        return message.MessageType switch
        {
            NetworkMessageTypes.Client.Authentication.TryAuth => BinarySerializer.SerializeToBytes((AuthMessage)message),
            NetworkMessageTypes.Client.Authentication.TryReconnect => BinarySerializer.SerializeToBytes((ReconnectMessage)message),
            
            NetworkMessageTypes.Client.Pregame.CreateRoom => BinarySerializer.SerializeToBytes((PlayerCreateRoomRequest)message),
            NetworkMessageTypes.Client.Pregame.JoinRoom => BinarySerializer.SerializeToBytes((PlayerJoinRoomRequest)message),
            NetworkMessageTypes.Client.Pregame.LeaveRoom => BinarySerializer.SerializeToBytes((PlayerLeaveRoom)message),
            NetworkMessageTypes.Client.FriendSystem.GetFriendList => BinarySerializer.SerializeToBytes((FriendListClientMessage)message),
            NetworkMessageTypes.Client.FriendSystem.RemoveFriend => BinarySerializer.SerializeToBytes((FriendRemoveClientMessage)message),
            NetworkMessageTypes.Client.FriendSystem.SendFriendRequest => BinarySerializer.SerializeToBytes((FriendRequestClientMessage)message),
            NetworkMessageTypes.Client.FriendSystem.AcceptFriendRequest => BinarySerializer.SerializeToBytes((AcceptRequestClientMessage)message),
            NetworkMessageTypes.Client.FriendSystem.DeclineFriendRequest => BinarySerializer.SerializeToBytes((DeclineRequestClientMessage)message),
            NetworkMessageTypes.Client.FriendSystem.GetFriendRequests => BinarySerializer.SerializeToBytes((GetRequestsClientMessage)message),
            NetworkMessageTypes.Client.FriendSystem.GetMyRequests => BinarySerializer.SerializeToBytes((GetMyRequestsClientMessage)message),
            // NetworkMessageTypes.Client.FriendSystem.InviteFriend => BinarySerializer.SerializeToBytes((SendInviteClientMessage)message),
            NetworkMessageTypes.Client.Pregame.GetRoomInfo => BinarySerializer.SerializeToBytes((PlayerGetRoomInfoRequest)message),
            NetworkMessageTypes.Client.Pregame.GetRoomByName => BinarySerializer.SerializeToBytes((PlayerGetRoomByNameRequest)message),
            NetworkMessageTypes.Client.Pregame.GetAllRoom => BinarySerializer.SerializeToBytes((PlayerGetAllRoomRequest)message),

            NetworkMessageTypes.Client.Pregame.Ready => BinarySerializer.SerializeToBytes((PlayerReady)message),
            NetworkMessageTypes.Client.Pregame.Unready => BinarySerializer.SerializeToBytes((PlayerUnready)message),

            NetworkMessageTypes.Client.Pregame.StartGame => BinarySerializer.SerializeToBytes((PlayerStartGameRequest)message),

            NetworkMessageTypes.Client.Ingame.Input => BinarySerializer.SerializeToBytes((BatchPlayerInputMessage)message),

            NetworkMessageTypes.Client.System.Ping => BinarySerializer.SerializeToBytes((PingMessage)message),
            NetworkMessageTypes.Client.System.GetUserInfo => BinarySerializer.SerializeToBytes((GetUserInfoClient)message),

            _ => null
        };
    }

    /// <summary>
    /// Deserialize a server response (byte array) into a message structure.
    /// </summary>
    public static ServerMessage DeserializeMessage(byte[] rawData)
    {
        try
        {
            using MemoryStream stream = new(rawData);
            using BinaryReader reader = new(stream);

            short messageLength = BinarySerializer.ReadInt16BigEndian(reader);

            short messageType = BinarySerializer.ReadInt16BigEndian(reader);
            Debug.Log(messageType);
            short statusCode = BinarySerializer.ReadInt16BigEndian(reader);
            Debug.Log(statusCode);
            byte[] payloadBytes = reader.ReadBytes(messageLength - (2 + 2));

            return CreateMessageFromType(messageType, payloadBytes);
        }
        catch (Exception e)
        {
            Debug.LogError($"[Deserialization Error] {e.Message}");
            return null;
        }
    }


    /// <summary>
    /// Maps short messageType codes to actual message types and deserializes from JSON.
    /// </summary>
    private static ServerMessage CreateMessageFromType(short messageType, byte[] payloadBytes)
    {
        return messageType switch
        {
            // Only have cases for server broadcast json
            NetworkMessageTypes.Server.System.AuthSuccess => BinarySerializer.DeserializeFromBytes<AuthSuccessMessage>(payloadBytes),
            NetworkMessageTypes.Server.System.Pong => BinarySerializer.DeserializeFromBytes<PongMessage>(payloadBytes),
            NetworkMessageTypes.Server.System.GetUserInfo => BinarySerializer.DeserializeFromBytes<GetUserInfoServer>(payloadBytes),
            // NetworkMessageTypes.System.Kick => BinarySerializer.DeserializeFromBytes<KickMessage>(payloadBytes)

            NetworkMessageTypes.Server.Room.CreateRoom => BinarySerializer.DeserializeFromBytes<ServerCreateRoom>(payloadBytes),
            NetworkMessageTypes.Server.Room.PlayerJoined => BinarySerializer.DeserializeFromBytes<ServerJoinRoom>(payloadBytes),
            NetworkMessageTypes.Server.Room.PlayerLeft => BinarySerializer.DeserializeFromBytes<ServerPlayerLeft>(payloadBytes),
            NetworkMessageTypes.Server.Room.Ready => BinarySerializer.DeserializeFromBytes<ServerPlayerReady>(payloadBytes),
            NetworkMessageTypes.Server.Room.UnReady => BinarySerializer.DeserializeFromBytes<ServerPlayerUnReady>(payloadBytes),
            NetworkMessageTypes.Server.Room.ACK => BinarySerializer.DeserializeFromBytes<ServerACK>(payloadBytes),

            NetworkMessageTypes.Server.FriendSystem.GetFriendList =>  BinarySerializer.DeserializeFromBytes<FriendListServerMessage>(payloadBytes),
            NetworkMessageTypes.Server.FriendSystem.RemoveFriend =>  BinarySerializer.DeserializeFromBytes<FriendRemoveServerMessage>(payloadBytes),
            NetworkMessageTypes.Server.FriendSystem.SendFriendRequest => BinarySerializer.DeserializeFromBytes<FriendRequestServerMessage>(payloadBytes),
            NetworkMessageTypes.Server.FriendSystem.AcceptFriendRequest =>  BinarySerializer.DeserializeFromBytes<AcceptRequestServerMessage>(payloadBytes),
            NetworkMessageTypes.Server.FriendSystem.DeclineFriendRequest =>  BinarySerializer.DeserializeFromBytes<DeclineRequestServerMessage>(payloadBytes),
            NetworkMessageTypes.Server.FriendSystem.GetFriendRequests =>  BinarySerializer.DeserializeFromBytes<GetRequestsServerMessage>(payloadBytes),
            NetworkMessageTypes.Server.FriendSystem.GetMyRequests =>  BinarySerializer.DeserializeFromBytes<GetMyRequestsServerMessage>(payloadBytes),
            // NetworkMessageTypes.Server.FriendSystem.InviteFriend =>  BinarySerializer.DeserializeFromBytes<SendInviteServerMessage>(payloadBytes),
            NetworkMessageTypes.Server.Room.GetRoomInfo => BinarySerializer.DeserializeFromBytes<ServerGetRoomInfo>(payloadBytes),
            NetworkMessageTypes.Server.Room.GetRoomByName => BinarySerializer.DeserializeFromBytes<ServerGetRoomByName>(payloadBytes),
            NetworkMessageTypes.Server.Room.GetAllRoom => BinarySerializer.DeserializeFromBytes<ServerGetAllRoom>(payloadBytes),

            NetworkMessageTypes.Server.Pregame.StartGame => BinarySerializer.DeserializeFromBytes<ServerStartGame>(payloadBytes),

            NetworkMessageTypes.Server.Player.Connected => BinarySerializer.DeserializeFromBytes<PlayerConnectedMessage>(payloadBytes),
            NetworkMessageTypes.Server.Player.Disconnected => BinarySerializer.DeserializeFromBytes<PlayerDisconnectedMessage>(payloadBytes),

            NetworkMessageTypes.Server.GameState.StateUpdate => BinarySerializer.DeserializeFromBytes<GameStatesUpdate>(payloadBytes),
            _ => null
        };
    }
    #endregion
}
