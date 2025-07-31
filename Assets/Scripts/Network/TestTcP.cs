using UnityEngine;

public class TestTcP : MonoBehaviour
{
    [Header("Create Room")]
    public string RoomName;
    public short RoomType;
    public short GameMode;
    [Header("Join Room")]
    public string JoinRoomName;

    [ContextMenu("Create Room")]
    public void CreateRoom()
    {
        NetworkManager.Instance.SendMessage(
            new PlayerCreateRoomRequest
            {
                RoomName = RoomName,
                RoomType = RoomType,
                GameMode = GameMode
            }
        );
    }

    [ContextMenu("Join Room")]
    public void JoinRoom()
    {
        NetworkManager.Instance.SendMessage(
            new PlayerJoinRoomRequest
            {

            }
        );
    }

    [ContextMenu("Start Game")]
    public void StartGame()
    {
        NetworkManager.Instance.SendMessage(
            new PlayerStartGameRequest
            {
                
            }
        );
    }

}
