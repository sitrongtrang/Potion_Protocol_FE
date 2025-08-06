using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Person
{
    public TMP_Text Name;
    public TMP_Text ID;
    public Image ReadyIcon;
    public Button addButton;
}

public class RoomHandler : MonoBehaviour
{
    [SerializeField] private RoomListRenderer _roomListRenderer;
    [SerializeField] private RoomScene _roomScene;
    [SerializeField] private Person[] Person;

    private bool _inRoom = false;

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
                OnJoinRoomResponse((ServerJoinRoom)message);
                break;
            case NetworkMessageTypes.Server.Room.RoomInvalidPassword:
                Debug.Log("Invalid Password");
                break;
            case NetworkMessageTypes.Server.Room.RoomNotExist:
                Debug.Log("Room not exist");
                NetworkManager.Instance.SendMessage(new PlayerGetAllRoomRequest());
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
            case NetworkMessageTypes.Server.Room.GetRoomByName:
                OnGetRoomByNameResponse((ServerGetRoomByName)message);
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
        NetworkManager.Instance.SendMessage(new PlayerGetRoomInfoRequest());
    }

    private void OnJoinRoomResponse(ServerJoinRoom msg)
    {
        Debug.Log("Player Join Room");
        // pop-up thông báo người chơi mới xuất hiện

        NetworkManager.Instance.SendMessage(new PlayerGetRoomInfoRequest { });
    }

    private void OnLeftRoomResponse(ServerPlayerLeft msg)
    {
        Debug.Log("Leave Room");
        _inRoom = false;
        for (int i = 0; i < Person.Length; i++)
        {
            if (string.Compare(msg.UserID, Person[i].ID.text) == 0)
            {
                _roomScene.SetPersonRoom(null, Person[i].ID);
                _roomScene.SetPersonRoom(null, Person[i].Name);
                Person[i].addButton.gameObject.SetActive(false);
            }
        }
    }

    private void OnPlayerReadyResponse(ServerPlayerReady msg)
    {
        Debug.Log("Player Ready");
        for (int i = 0; i < Person.Length; i++)
        {
            if (string.Compare(msg.UserID, Person[i].ID.text) == 0)
            {
                CreateRoomUI.Instance.SetImageAlpha(255f, Person[i].ReadyIcon);
                break;
            }
        }
    }

    private void OnPlayerUnReadyResponse(ServerPlayerUnReady msg)
    {
        Debug.Log("Player UnReady");
        for (int i = 0; i < Person.Length; i++)
        {
            if (string.Compare(msg.UserID, Person[i].ID.text) == 0)
            {
                CreateRoomUI.Instance.SetImageAlpha(0f, Person[i].ReadyIcon);
                break;
            }
        }
    }

    private void OnGetRoomInfoResponse(ServerGetRoomInfo msg)
    {
        Debug.Log("Get Room Info");
        _roomScene.ChooseImage(msg.Room.MapID);
        for (int i = 0; i < msg.Room.PlayerList.Length; i++)
        {
            _roomScene.SetPersonRoom(msg.Room.PlayerList[i].PlayerDisPlayName, Person[i].Name);
            _roomScene.SetPersonRoom(msg.Room.PlayerList[i].PlayerID, Person[i].ID);
            if (msg.Room.PlayerList[i].PlayerID != NetworkManager.Instance.ClientId && !string.IsNullOrEmpty(msg.Room.PlayerList[i].PlayerDisPlayName))
            {
                Debug.Log(msg.Room.PlayerList[i].PlayerDisPlayName);
                Person[i].addButton.gameObject.SetActive(true);
            }
            else
            {
                Debug.Log(!string.IsNullOrEmpty(msg.Room.PlayerList[i].PlayerDisPlayName) ? msg.Room.PlayerList[i].PlayerDisPlayName : "NULLLLLLLLLLLLLL");
                Person[i].addButton.gameObject.SetActive(false);
            }
        }
        _roomScene.SetRoomName(msg.Room.RoomName);
        if (!_inRoom)
        {
            CreateRoomUI.Instance.ShowPvPCanvas();
            _inRoom = true;
        }
    }

    private void OnGetRoomByNameResponse(ServerGetRoomByName msg)
    {
        Debug.Log("Get Room By Name");
        _roomListRenderer.RenderRooms(msg.Room);
    }

    private void OnGetAllRoomResponse(ServerGetAllRoom msg)
    {
        Debug.Log("Get All Room");
        if (msg.Room != null && msg.Room.Length > 0) Debug.Log(msg.Room[0].RoomName);
        CreateRoomUI.Instance.Refreshed();
        _roomListRenderer.RenderRooms(msg.Room);
    }
}
