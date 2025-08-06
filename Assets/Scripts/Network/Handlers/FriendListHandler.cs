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
    FriendViewMode _currentMode;
    [Header("ItemPrefab")]
    [SerializeField] GameObject _friendItemPrefab; // format:  nameText, inviteButton, removeButton, avatarObj (1 ring img and avatar child)
    [SerializeField] GameObject _friendRequestItemPrefab; // format:  nameText, acceptButton, declineButton, avatarObj (1 ring img and avatar child)
    [SerializeField] GameObject _myRequestItemPrefab; // format:  nameText, cancelButton, avatarObj (1 ring img and avatar child)
    
    [Header("Paging")]
    [SerializeField] TMP_Text _paging; // format: xx/yy
    private int _currentPage;
    [SerializeField] int _numberPerPage;

    [Header("ListItemDisplayed")]
    GameObject[] _friendItemList;
    GameObject[] _friendRequestList;
    GameObject[] _myRequestList;
    
    [Header("PanelContainer")]
    [SerializeField] GameObject _friendListContainer;
    [SerializeField] GameObject _friendRequestContainer;
    [SerializeField] GameObject _myRequestContainer;
    [Header("ListDataContainer")]
    List<Friend> _friendList = new List<Friend>(); // item of friend list, friend request and my request has the same type


    private void OnEnable()
    {
        _friendItemList = new GameObject[_numberPerPage];
        for (int i = 0; i < _numberPerPage; i++)
        {
            _friendItemList[i] = Instantiate(_friendItemPrefab, transform);
            _friendItemList[i].SetActive(false);
        }
        NetworkEvents.OnMessageReceived += HandleNetworkMessage;
        GameManager.Instance.LoadFriendList += DisplayFriend;
    }

    private void OnDisable()
    {
        NetworkEvents.OnMessageReceived -= HandleNetworkMessage;
        GameManager.Instance.LoadFriendList -= DisplayFriend;
    }

    void Start()
    {
        DisplayFriendList();
    }

    void DisplayCurrentList(FriendViewMode currentMode)
    {
        // Ẩn hết
        _friendListContainer.SetActive(false);
        _friendRequestContainer.SetActive(false);
        _myRequestContainer.SetActive(false);

        // Hiện cái đang chọn
        switch (currentMode)
        {
            case FriendViewMode.FriendList:
                _friendListContainer.SetActive(true);
                DisplayFriendList();
                break;
            case FriendViewMode.FriendRequest:
                _friendRequestContainer.SetActive(true);
                DisplayFriendRequest();
                break;
            case FriendViewMode.MyRequest:
                _myRequestContainer.SetActive(true);
                DisplayMyRequest();
                break;
        }
    }

    public void ShowFriendList()
    {
        _currentMode = FriendViewMode.FriendList;
        _currentPage = 1;
        DisplayCurrentList(_currentMode);
    }

    public void ShowFriendRequest()
    {
        _currentMode = FriendViewMode.FriendRequest;
        _currentPage = 1;
        DisplayCurrentList(_currentMode);
    }

    public void ShowMyRequest()
    {
        _currentMode = FriendViewMode.MyRequest;
        _currentPage = 1;
        DisplayCurrentList(_currentMode);
    }

    void DisplayFriendList()
    {
        FriendListClientMessage friendListMsg = new FriendListClientMessage();
        NetworkManager.Instance.SendMessage(friendListMsg);
    }

    void DisplayFriendRequest()
    {
        GetRequestsClientMessage friendRequestList = new GetRequestsClientMessage();
        NetworkManager.Instance.SendMessage(friendRequestList);
    }

    void DisplayMyRequest()
    {
        
    }
    void DisplayFriend(int page)
    {
        for (int i = (page - 1) * _numberPerPage; i < page * _numberPerPage + _numberPerPage && i < _friendList.Count; i++)
        {
            _friendItemList[i].SetActive(true);
            _friendItemList[i].GetComponent<FriendItemUI>().NameText.GetComponent<TMP_Text>().text = _friendList[i].FriendDisplayName;
            _friendItemList[i].GetComponent<FriendItemUI>().InviteButton.GetComponent<Button>().onClick.AddListener(() => SendInvite(_friendList[i].Id));
            _friendItemList[i].GetComponent<FriendItemUI>().RemoveButton.GetComponent<Button>().onClick.RemoveAllListeners();
            _friendItemList[i].GetComponent<FriendItemUI>().RemoveButton.GetComponent<Button>().onClick.AddListener(() =>
            {
                SendRemoveFriend(_friendList[i].Id);
                Destroy(_friendItemList[i]);
            });
        }
    }

    private void HandleNetworkMessage(ServerMessage message)
    {
        Debug.Log(message.MessageType);
        switch (message.MessageType)
        {
            case NetworkMessageTypes.Server.FriendSystem.GetFriendList:
                if (message is FriendListServerMessage friendListMsg)
                {
                    Debug.Log($"👥 Received {friendListMsg.FriendList.Count} friends:");
                    foreach (var friend in friendListMsg.FriendList)
                    {
                        _friendList.Add(friend);
                        Debug.Log($"👤 {friend.FriendDisplayName} ({friend.FriendId}, {friend.Id})");
                    }
                    _currentPage = 1;
                    DisplayFriend(_currentPage);
                    int limitPage = _friendList.Count % _numberPerPage == 0 ? _friendList.Count / _numberPerPage : _friendList.Count / _numberPerPage + 1;
                    _paging.text = _currentPage.ToString() + "/" + limitPage.ToString();
                }
                else
                {
                    Debug.LogError("⚠️ Lỗi: Không thể cast message sang FriendListServerMessage.");
                }
                break;
            case NetworkMessageTypes.Server.FriendSystem.RemoveFriend:
                if (message is FriendRemoveServerMessage)
                {
                    Debug.Log("Remove friend successfully");
                }
                else
                {
                    Debug.LogError("⚠️ Lỗi: Không thể cast message sang FriendListServerMessage.");
                }
                break;
        }
    }

    private void SendRemoveFriend(string id)
    {
        FriendRemoveClientMessage removeFriend = new FriendRemoveClientMessage(id);
        Debug.Log(id);
        NetworkManager.Instance.SendMessage(removeFriend);
    }
    private void SendInvite(string id)
    {

    }
}
    