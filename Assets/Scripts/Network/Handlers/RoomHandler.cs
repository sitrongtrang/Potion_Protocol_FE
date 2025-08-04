using TMPro;
using UnityEngine;

public class RoomHandler : MonoBehaviour
{
    [SerializeField] private RoomListRenderer _roomListRenderer;
    [SerializeField] private RoomScene _roomScene;
    public TMP_Text[] Person;
    private void OnEnable()
    {
        NetworkEvents.OnMessageReceived += HandleNetworkMessage;
        LoadingScreenUI.Instance.OnSceneEnter += HandleAllRoom;
    }

    private void OnDisable()
    {
        NetworkEvents.OnMessageReceived -= HandleNetworkMessage;
        LoadingScreenUI.Instance.OnSceneEnter -= HandleAllRoom;
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

    private void HandleAllRoom()
    {
        Debug.Log("Request get all room");
        NetworkManager.Instance.SendMessage(new PlayerGetAllRoomRequest());
    }

    private void OnCreateRoomResponse(ServerCreateRoom msg)
    {
        Debug.Log($"Server đã tạo phòng: {msg.RoomID}");
        _roomScene.ChooseImage();
        CreateRoomUI.Instance.ShowPvPCanvas();
        NetworkManager.Instance.SendMessage(new PlayerGetRoomInfoRequest{});
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
        Debug.Log(msg.Room.PlayerList[0]);
        for (int i = 0; i < msg.Room.PlayerList.Length; i++)
        {
            _roomScene.SetPersonRoomName(msg.Room.PlayerList[i].PlayerDisPlayName, Person[i]);
        }
        _roomScene.SetRoomID(msg.Room.RoomID);
    }

    private void OnGetRoomByIDResponse(ServerGetRoomByID msg)
    {
        Debug.Log("Get Room By ID");
        _roomListRenderer.RenderRooms(new RoomInfo[] { msg.Room });
    }

    private void OnGetAllRoomResponse(ServerGetAllRoom msg)
    {
        Debug.Log("Get All Room");
        CreateRoomUI.Instance.Refreshed();
        _roomListRenderer.RenderRooms(msg.Room);
    }
}
