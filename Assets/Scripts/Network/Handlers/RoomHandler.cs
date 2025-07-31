using NUnit.Framework;
using System.Collections.Generic;
using UnityEditor.XR;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomHandler : MonoBehaviour
{
    private void OnEnable()
    {
        NetworkEvents.OnMessageReceived += HandleNetworkMessage;
    }

    private void OnDisable()
    {
        NetworkEvents.OnMessageReceived -= HandleNetworkMessage;
    }

    private void HandleNetworkMessage(ServerMessage message)
    {
        switch (message.MessageType)
        {
            case NetworkMessageTypes.Server.Room.CreateRoom:
                OnCreateRoomResponse((ServerCreateRoom)message);
                break;
            case NetworkMessageTypes.Server.Room.RoomFull:
                Debug.Log("RoomFull");
                break;
            case NetworkMessageTypes.Server.Room.InRoom:
                Debug.Log("In Room");
                break;
            case NetworkMessageTypes.Server.Room.NotInRoom:
                Debug.Log("Not In Room");
                break;
            case NetworkMessageTypes.Server.Room.PlayerJoined:
                OnJoinRoomResponse((ServerInRoom)message);
                break;
            case NetworkMessageTypes.Server.Room.RoomInvalidPassword:
                Debug.Log("Invalid Password");
                break;
            case NetworkMessageTypes.Server.Room.PlayerLeft:
                OnLeftRoomResponse((ServerPlayerLeft)message);
                break;
            case NetworkMessageTypes.Server.Room.Ready:
                OnPlayerReadyResponse((ServerPlayerReady)message);
                break;
            case NetworkMessageTypes.Server.Room.UnReady:
                OnPlayerUnReadyResponse((ServerPlayerUnReady)message);
                break;
            case NetworkMessageTypes.Server.Room.GetRoomInfo:
                OnGetRoomInfoResponse((ServerGetRoomInfo)message);
                break;
            case NetworkMessageTypes.Server.Room.GetRoomByID:
                OnGetRoomByIDResponse((ServerGetRoomByID)message);
                break;
            case NetworkMessageTypes.Server.Room.GetAllRoom:
                OnGetAllRoomResponse((ServerGetAllRoom)message);
                break;
            default:
                break;
        }
    }

    private void OnCreateRoomResponse(ServerCreateRoom msg)
    {
        Debug.Log($"Server đã tạo phòng: {msg.RoomID}");
        // Load create room scene
        SceneManager.LoadSceneAsync("RoomScene");
    }

    private void OnJoinRoomResponse(ServerInRoom msg)
    {
        Debug.Log("Player Join Room");

    }

    private void OnLeftRoomResponse(ServerPlayerLeft msg)
    {
        Debug.Log("Leave Room");

    }

    private void OnPlayerReadyResponse(ServerPlayerReady msg)
    {
        Debug.Log("Player Ready");

    }

    private void OnPlayerUnReadyResponse(ServerPlayerUnReady msg)
    {
        Debug.Log("Player UnReady");

    }

    private void OnGetRoomInfoResponse(ServerGetRoomInfo msg)
    {
        Debug.Log("Get Room Info");

    }

    private void OnGetRoomByIDResponse(ServerGetRoomByID msg)
    {
        Debug.Log("Get Room By ID");

    }

    private void OnGetAllRoomResponse(ServerGetAllRoom msg)
    {
        Debug.Log("Get All Room");

    }
}
