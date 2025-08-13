using TMPro;
using UnityEngine;

public class RoomInviteUI : MonoBehaviour
{
    [SerializeField] private TMP_Text _name;
    private string _roomID;

    public void SetName(string newName, string roomID)
    {
        _name.text = newName + " invite you into the room.";
        _roomID = roomID;
    }

    public void JoinRoom()
    {
        NetworkManager.Instance.SendMessage(new PlayerJoinRoomRequest
        {
            RoomId = _roomID,
            Password = ""
        });
    }
}
