using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum FriendViewMode
{
    FriendList,
    FriendRequest,
    MyRequest
}
public class FriendListHandler : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] GameObject _friendItemPrefab;
    [SerializeField] GameObject _friendRequestItemPrefab;
    [SerializeField] GameObject _myRequestItemPrefab;

    [Header("Paging")]
    [SerializeField] TMP_Text _friendListPagingText;
    [SerializeField] TMP_Text _friendRequestPagingText;
    [SerializeField] TMP_Text _myRequestPagingText;
    [SerializeField] int _numberPerPage = 5;

    [Header("Containers")]
    [SerializeField] GameObject _friendListContainer;
    [SerializeField] GameObject _friendRequestContainer;
    [SerializeField] GameObject _myRequestContainer;
    [SerializeField] Transform _friendListParent;
    [SerializeField] Transform _friendRequestParent;
    [SerializeField] Transform _myRequestParent;

    FriendViewMode _currentMode;
    
    Dictionary<FriendViewMode, int> _currentPages = new();
    int _currentPage = 1;

    List<Friend> _friendList = new List<Friend>();
    List<Friend> _friendRequestList = new List<Friend>();
    List<Friend> _myRequestList = new List<Friend>();

    Dictionary<FriendViewMode, GameObject[]> _uiPools = new();
    Dictionary<FriendViewMode, GameObject> _prefabs;
    Dictionary<FriendViewMode, Transform> _parents;

    void Awake()
    {
        _prefabs = new()
        {
            { FriendViewMode.FriendList, _friendItemPrefab },
            { FriendViewMode.FriendRequest, _friendRequestItemPrefab },
            { FriendViewMode.MyRequest, _myRequestItemPrefab },
        };

        _parents = new()
        {
            { FriendViewMode.FriendList, _friendListParent },
            { FriendViewMode.FriendRequest, _friendRequestParent },
            { FriendViewMode.MyRequest, _myRequestParent },
        };

        foreach (var mode in _prefabs.Keys)
        {
            GameObject[] pool = new GameObject[_numberPerPage];
            for (int i = 0; i < _numberPerPage; i++)
            {
                pool[i] = Instantiate(_prefabs[mode], _parents[mode]);
                pool[i].SetActive(false);
            }
            _uiPools[mode] = pool;
        }
    }

    void OnEnable()
    {
        NetworkEvents.OnMessageReceived += HandleNetworkMessage;
        GameManager.Instance.LoadFriendList += DisplayUIItems;
    }

    void OnDisable()
    {
        NetworkEvents.OnMessageReceived -= HandleNetworkMessage;
        GameManager.Instance.LoadFriendList -= DisplayUIItems;
    }

    public void ShowFriendList()
    {
        SwitchMode(FriendViewMode.FriendList);
        
    }
    public void ShowFriendRequest() => SwitchMode(FriendViewMode.FriendRequest);
    public void ShowMyRequest() => SwitchMode(FriendViewMode.MyRequest);

    void SwitchMode(FriendViewMode mode)
    {
        _currentMode = mode;
        _currentPage = 1;

        _friendListContainer.SetActive(mode == FriendViewMode.FriendList);
        _friendRequestContainer.SetActive(mode == FriendViewMode.FriendRequest);
        _myRequestContainer.SetActive(mode == FriendViewMode.MyRequest);

        switch (mode)
        {
            case FriendViewMode.FriendList:
                RequestFriendList(); break;
            case FriendViewMode.FriendRequest:
                RequestFriendRequests(); break;
            case FriendViewMode.MyRequest:
                RequestMyRequests(); break;
        }
    }

    void DisplayUIItems(FriendViewMode mode, int page)
    {
        var list = mode switch
        {
            FriendViewMode.FriendList => _friendList,
            FriendViewMode.FriendRequest => _friendRequestList,
            FriendViewMode.MyRequest => _myRequestList,
            _ => null
        };

        var pool = _uiPools[mode];
        int start = (page - 1) * _numberPerPage;

        for (int i = 0; i < pool.Length; i++)
        {
            if (start + i < list.Count)
            {
                SetupUI(pool[i], list[start + i], mode);

                pool[i].SetActive(true);
            }
            else
            {
                pool[i].SetActive(false);
            }
        }

        int maxPage = Mathf.CeilToInt((float)list.Count / _numberPerPage);
        string pageText = $"{_currentPage}/{Mathf.Max(1, maxPage)}";
        switch (mode)
        {
            case FriendViewMode.FriendList:
                _friendListPagingText.text = pageText;
                break;
            case FriendViewMode.FriendRequest:
                _friendRequestPagingText.text = pageText;
                break;
            case FriendViewMode.MyRequest:
                _myRequestPagingText.text = pageText;
                break;
        }
    }

    void SetupUI(GameObject go, Friend data, FriendViewMode mode)
    {
        switch (mode)
        {
            case FriendViewMode.FriendList:
                var ui = go.GetComponent<FriendItemUI>();
                ui.NameText.GetComponent<TMP_Text>().text = data.FriendDisplayName;
                ui.InviteButton.GetComponent<Button>().onClick.RemoveAllListeners();
                ui.InviteButton.GetComponent<Button>().onClick.AddListener(() => SendInvite(data.Id));

                ui.RemoveButton.GetComponent<Button>().onClick.RemoveAllListeners();
                ui.RemoveButton.GetComponent<Button>().onClick.AddListener(() =>
                {
                    SendRemoveFriend(data.Id);
                    go.SetActive(false);
                });
                break;
            case FriendViewMode.FriendRequest:
                var ui1 = go.GetComponent<FriendRequestItemUI>();
                ui1.NameText.GetComponent<TMP_Text>().text = data.FriendDisplayName;
                ui1.NameText.GetComponent<TMP_Text>().text = data.FriendDisplayName;
                ui1.AcceptButton.GetComponent<Button>().onClick.RemoveAllListeners();
                ui1.AcceptButton.GetComponent<Button>().onClick.AddListener(() =>
                {
                    SendAccept(data.Id);
                    go.SetActive(false);
                });

                ui1.DeclineButton.GetComponent<Button>().onClick.RemoveAllListeners();
                ui1.DeclineButton.GetComponent<Button>().onClick.AddListener(() =>
                {
                    SendDecline(data.Id);
                    go.SetActive(false);
                });
                break;
            case FriendViewMode.MyRequest:
                var ui2 = go.GetComponent<MyRequestItemUI>();
                ui2.NameText.GetComponent<TMP_Text>().text = data.FriendDisplayName;
                ui2.NameText.GetComponent<TMP_Text>().text = data.FriendDisplayName;
                ui2.CancelButton.GetComponent<Button>().onClick.RemoveAllListeners();
                ui2.CancelButton.GetComponent<Button>().onClick.AddListener(() =>
                {
                    // SendRemoveRequest(data.Id);
                    go.SetActive(false);
                });
                break;
            // TODO: Handle other modes if needed
        }
    }

    void SendAccept(string id)
    {
        NetworkManager.Instance.SendMessage(new AcceptRequestClientMessage(id));
    }

    void SendDecline(string id)
    {
        NetworkManager.Instance.SendMessage(new DeclineRequestClientMessage(id));
    }

    void RequestFriendList()
    {
        NetworkManager.Instance.SendMessage(new FriendListClientMessage());
        Debug.Log("Send friend list request");
    }

    void RequestFriendRequests()
    {
        NetworkManager.Instance.SendMessage(new GetRequestsClientMessage());
        Debug.Log("Send friend requests request");
    }

    void RequestMyRequests()
    {
        NetworkManager.Instance.SendMessage(new GetMyRequestsClientMessage());
        Debug.Log("Send my requests request");
        // Add request message
    }

    void HandleNetworkMessage(ServerMessage message)
    {
        Debug.Log("Message type: " + message.MessageType);
        switch (message.MessageType)
        {
            case NetworkMessageTypes.Server.FriendSystem.GetFriendList:
                if (message is FriendListServerMessage flMsg)
                {
                    _friendList = flMsg.FriendList;
                    if (_currentMode == FriendViewMode.FriendList)
                    {
                        DisplayUIItems(_currentMode, _currentPage);
                    }
                }
                break;

            case NetworkMessageTypes.Server.FriendSystem.GetFriendRequests:
                if (message is GetRequestsServerMessage frMsg)
                {
                    _friendRequestList = frMsg.FriendList;
                    for (int i = 0; i < _friendRequestList.Count; i++)
                    {
                        Debug.Log(_friendRequestList[i].FriendDisplayName);
                    }
                    if (_currentMode == FriendViewMode.FriendRequest)
                    {
                        DisplayUIItems(_currentMode, _currentPage);
                    }
                }
                break;

            case NetworkMessageTypes.Server.FriendSystem.RemoveFriend:
                Debug.Log("✅ Remove friend success.");
                break;
            case NetworkMessageTypes.Server.FriendSystem.GetMyRequests:
                if (message is GetMyRequestsServerMessage myRequest)
                {
                    _myRequestList = myRequest.FriendList;
                    for (int i = 0; i < _myRequestList.Count; i++)
                    {
                        Debug.Log(_myRequestList[i].FriendDisplayName);
                    }
                    Debug.Log("_currentMode: " + _currentMode);
                    if (_currentMode == FriendViewMode.MyRequest)
                    {
                        DisplayUIItems(_currentMode, _currentPage);
                    }
                }
                break;
        }
    }

    void SendRemoveFriend(string id)
    {
        NetworkManager.Instance.SendMessage(new FriendRemoveClientMessage(id));
    }

    void SendInvite(string id)
    {
        Debug.Log($"Sending invite to {id}");
        // Implement invite logic
    }
}
