using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FriendListRequestHandler : MonoBehaviour
{
    [SerializeField] GameObject _friendItemPrefab;
    void Start()
    {

    }

    private void OnEnable()
    {
        NetworkEvents.OnMessageReceived += HandleNetworkMessage;
    }

    private void OnDisable()
    {
        NetworkEvents.OnMessageReceived -= HandleNetworkMessage;
    }

    public void OnButtonClicked()
    {
        SendGetFriendList();
    }

    void SendGetFriendList()
    {
        FriendListClientMessage friendListMsg = new FriendListClientMessage();
        NetworkManager.Instance.SendMessage(friendListMsg);
    }
    private void HandleNetworkMessage(ServerMessage message)
    {
        switch (message.MessageType)
        {
            case NetworkMessageTypes.Server.FriendSystem.GetFriendList:
                if (message is FriendListServerMessage friendListMsg)
                {
                    Debug.Log($"👥 Received {friendListMsg.friendList.Count} friends:");
                    foreach (var friend in friendListMsg.friendList)
                    {
                        GameObject friendItem = Instantiate(_friendItemPrefab, transform);
                        friendItem.GetComponent<FriendItemUI>().nameText.GetComponent<TMP_Text>().text = friend.FriendDisplayName;
                        friendItem.GetComponent<FriendItemUI>().inviteButton.GetComponent<Button>().onClick.AddListener(() => SendInvite(friend.Id));
                        friendItem.GetComponent<FriendItemUI>().removeButton.GetComponent<Button>().onClick.AddListener(() => SendRemoveFriend(friend.Id));
                        Debug.Log($"👤 {friend.FriendDisplayName} ({friend.FriendId}, {friend.Id})");
                    }
                }
                else
                {
                    Debug.LogError("⚠️ Lỗi: Không thể cast message sang FriendListServerMessage.");
                }
                break;
            case NetworkMessageTypes.Server.FriendSystem.RemoveFriend:
                Debug.Log("Remove friend successfully");
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
    