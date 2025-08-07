using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[System.Serializable]
public class Person
{
    public TMP_Text Name;
    public TMP_Text ID;
    public Image ReadyIcon;
}

public class RoomHandler : MonoBehaviour
{
    [Header("Room Reference")]
    [SerializeField] private RoomListRenderer _roomListRenderer;
    [SerializeField] private RoomScene _roomScene;
    [SerializeField] private CreateRoom _createRoom;
    [Header("Person")]
    [SerializeField] private Person[] Person;
    [SerializeField] private Animator[] Avt;
    [Header("Button")]
    [SerializeField] private TMP_Text _startButton;

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
            case NetworkMessageTypes.Server.Pregame.StartGame:
                SceneManager.LoadSceneAsync("GameScene");
                break;
            case NetworkMessageTypes.Server.Room.ACK:
                OnSelfLeaveRoom();
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
        if (msg.LeaderID == NetworkManager.Instance.ClientId)
        {
            _roomScene.Leader = true;
            NetworkManager.Instance.SendMessage(new PlayerReady());
        }
        for (int i = 0; i < Person.Length; i++)
        {
            if (string.Compare(msg.LeaderID, Person[i].ID.text) == 0)
            {
                Person[i].Name.color = Color.yellow;
                _roomScene.SetPersonRoom("Start", _startButton);
            }

            if (string.Compare(msg.UserID, Person[i].ID.text) == 0)
            {
                _roomScene.SetPersonRoom(null, Person[i].Name);
                _roomScene.SetPersonRoom(null, Person[i].ID);
                _roomScene.RunAnim(Avt[i], false);
            }
        }
    }

    public void OnSelfLeaveRoom()
    {
        Debug.Log("Leave Room");
        CreateRoomUI.Instance.ShowRoomListCanvas();
        if (_roomScene.Leader == true)
        {
            _roomScene.Leader = false;
            _roomScene.SetPersonRoom("Ready", _startButton);
        }

        ResetRoom();
                
        _inRoom = !_inRoom;
        CreateRoomUI.Instance.OnRefreshButtonClicked();
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
        SetLeader(msg.Room.PlayerList[0]);
        _roomScene.ChooseImage(msg.Room.MapID);
        _roomScene.SetRoomName(msg.Room.RoomName);
        RenderRoom(msg.Room.PlayerList);
    }

    private void OnGetRoomByNameResponse(ServerGetRoomByName msg)
    {
        Debug.Log("Get Room By Name");
        CreateRoomUI.Instance.Refreshed();
        _roomListRenderer.RenderRooms(msg.Room);
    }

    private void OnGetAllRoomResponse(ServerGetAllRoom msg)
    {
        Debug.Log("Get All Room");
        if (msg.Room != null && msg.Room.Length > 0) Debug.Log(msg.Room[0].RoomName);
        CreateRoomUI.Instance.Refreshed();
        _roomListRenderer.RenderRooms(msg.Room);
    }

    private void SetLeader(PlayerInfo Leader)
    {
        if (NetworkManager.Instance.ClientId == Leader.PlayerID)
        {
            _roomScene.Leader = true;
            NetworkManager.Instance.SendMessage(new PlayerReady());
        }
    }

    private void RenderRoom(PlayerInfo[] PlayerList)
    {
        if (_inRoom == false)
        {
            _inRoom = !_inRoom;
            CreateRoomUI.Instance.ShowPvPCanvas();
            _createRoom.ResetRoom();
        }
        for (int i = 0; i < PlayerList.Length; i++)
        {
            
            if (IsExistPerson(PlayerList[i])) continue;

            int slot = FindEmptySlot();

            _roomScene.SetPersonRoom(PlayerList[i].PlayerDisPlayName, Person[slot].Name);
            _roomScene.SetPersonRoom(PlayerList[i].PlayerID, Person[slot].ID);
            _roomScene.RunAnim(Avt[slot], true);
            if (PlayerList[i].PlayerRole == (short)PlayerRole.Leader)
            {
                Person[slot].Name.color = Color.yellow;
            }
            else Person[slot].Name.color = Color.white;
        }
        if (_roomScene.Leader == true) _roomScene.SetPersonRoom("Start", _startButton);
        else _roomScene.SetPersonRoom("Ready", _startButton);
    }

    private int FindEmptySlot()
    {
        for (int i = 0; i < Person.Length; i++)
        {
            if (string.IsNullOrEmpty(Person[i].ID.text)) return i;
        }
        return -1;
    }

    private void ResetRoom()
    {
        for (int i = 0; i < Person.Length; i++)
        {
            _roomScene.SetPersonRoom(null, Person[i].Name);
            _roomScene.SetPersonRoom(null, Person[i].ID);
            _roomScene.RunAnim(Avt[i], false);
        }
    }

    private bool IsExistPerson(PlayerInfo Player)
    {
        for (int i = 0; i < Person.Length; i++)
        {
            if (Player.PlayerID == Person[i].ID.text)
            {
                return true;
            }
        }
        return false;
    }
}
