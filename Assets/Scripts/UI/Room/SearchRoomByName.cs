using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SearchRoomByName : MonoBehaviour
{
    public TMP_InputField searchInput;

    private Coroutine _searchRoom;

    void Start()
    {
        searchInput.onValueChanged.AddListener(FilterRooms);
    }

    public void FilterRooms(string keyword)
    {
        if (string.IsNullOrEmpty(keyword)) return;
        if (_searchRoom != null) StopCoroutine(_searchRoom);
        _searchRoom = StartCoroutine(CreateRoomUI.Instance.HandleRefresh());

        NetworkManager.Instance.SendMessage(new PlayerGetRoomByNameRequest
        {
            roomName = keyword,
        });

        CreateRoomUI.Instance.Refreshed();
    }
}
