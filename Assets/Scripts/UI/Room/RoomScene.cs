using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RoomScene : MonoBehaviour
{
    [SerializeField] private Image TargetImage;
    [SerializeField] private Sprite[] NewSprite;
    [SerializeField] private TMP_Text RoomID;

    private bool _ready = false;
    public static int img = 0;
    public bool Leader = false;

    public void ChooseImage(int image)
    {
        TargetImage.sprite = NewSprite[image];
        Debug.Log("Img: " + image);
    }

    public void SetPersonRoom(string newText, TMP_Text Person)
    {
        Person.text = newText;
    }

    public void SetRoomName(string newName)
    {
        RoomID.text = "Room Name: " + newName;
    }

    public void OnLeaveRoom()
    {
        NetworkManager.Instance.SendMessage(new PlayerUnready());
        NetworkManager.Instance.SendMessage(new PlayerLeaveRoom());
        CreateRoomUI.Instance.ShowRoomListCanvas();
        CreateRoomUI.Instance.OnRefreshButtonClicked();
    }

    public void OnReadyRoom()
    {
        if (Leader)
        {
            NetworkManager.Instance.SendMessage(new PlayerStartGameRequest());
            return;
        }
        _ready = !_ready;
        if (_ready)
        {
            NetworkManager.Instance.SendMessage(new PlayerReady());
        }
        else
        {
            NetworkManager.Instance.SendMessage(new PlayerUnready());
        }
    }
}
