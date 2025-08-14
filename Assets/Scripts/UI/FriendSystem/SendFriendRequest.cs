using TMPro;
using UnityEngine;

public class SendFriendRequest : MonoBehaviour
{
    [SerializeField] TMP_Text _userID;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnEnable()
    {
        NetworkEvents.OnMessageReceived += HandleNetworkMessage;
    }

    void HandleNetworkMessage(ServerMessage message)
    {
        Debug.Log(message.MessageType);
        switch (message.MessageType)
        {
            case NetworkMessageTypes.Server.FriendSystem.SendFriendRequest:
                Debug.Log("Send friend request successfully!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                break;
        }
    }

    public void OnButtonClicked()
    {
        FriendRequestClientMessage friendRequest = new FriendRequestClientMessage(_userID.text);
        NetworkManager.Instance.SendMessage(friendRequest);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
