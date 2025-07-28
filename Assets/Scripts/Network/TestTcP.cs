using UnityEngine;

public class TestTcP : MonoBehaviour
{
    [Header("Create Room")]
    public string RoomName;
    public short RoomType;
    public int MaxPlayers;

    [ContextMenu("Create Room")]
    public void CreateRoom()
    {
        NetworkManager.Instance.SendMessage(
            new PlayerCreateRoomRequest
            {
                RoomName = RoomName,
                RoomType = RoomType,
                MaxPlayers = MaxPlayers
            }
        );
    }
}
