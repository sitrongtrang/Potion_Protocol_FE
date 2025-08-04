using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FriendListRequestHandler : MonoBehaviour
{
    [Header("FriendItem")]
    [SerializeField] GameObject _friendItemPrefab; // format:  nameText, inviteButton, removeButton, avatarObj (1 ring img and avatar child)
    [SerializeField] TMP_Text _paging; // format: xx/yy
    [SerializeField] int _numberPerPage;
    GameObject[] _friendItem;

    [Header("FriendListContainer")]
    List<Friend> _friendList = new List<Friend>();

    private void OnEnable()
    {
        _friendItem = new GameObject[_numberPerPage];
        for (int i = 0; i < _numberPerPage; i++)
        {
            _friendItem[i] = Instantiate(_friendItemPrefab, transform);
            _friendItem[i].SetActive(false);
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
        SendGetFriendList();
    }

    void SendGetFriendList()
    {
        FriendListClientMessage friendListMsg = new FriendListClientMessage();
        NetworkManager.Instance.SendMessage(friendListMsg);
    }

    void DisplayFriend(int page)
    {
        for (int i = (page - 1) * _numberPerPage; i < page * _numberPerPage + _numberPerPage && i < _friendList.Count; i++)
        {
            _friendItem[i].SetActive(true);
            _friendItem[i].GetComponent<FriendItemUI>().nameText.GetComponent<TMP_Text>().text = _friendList[i].FriendDisplayName;
            _friendItem[i].GetComponent<FriendItemUI>().inviteButton.GetComponent<Button>().onClick.AddListener(() => SendInvite(_friendList[i].Id));
            _friendItem[i].GetComponent<FriendItemUI>().removeButton.GetComponent<Button>().onClick.AddListener(() =>
            {
                SendRemoveFriend(_friendList[i].Id);
                Destroy(_friendItem[i]);
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
                    int currentPage = 1;
                    DisplayFriend(currentPage);
                    int limitPage = _friendList.Count % _numberPerPage == 0 ? _friendList.Count / _numberPerPage : _friendList.Count / _numberPerPage + 1;
                    _paging.text = currentPage.ToString() + "/" + limitPage.ToString();
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
    