using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SearchRoomByName : MonoBehaviour
{
    [SerializeField] private TMP_InputField _searchInput;
    public TMP_InputField SearchInput => _searchInput;
    private Coroutine _searchRoom;

    void Start()
    {
        _searchInput.onValueChanged.AddListener(FilterRooms);
    }

    public void FilterRooms(string keyword)
    {
        if (string.IsNullOrEmpty(keyword)) return;
        if (_searchRoom != null) StopCoroutine(_searchRoom);
        _searchRoom = StartCoroutine(CreateRoomUI.Instance.HandleRefresh());

        NetworkManager.Instance.SendMessage(new PlayerGetRoomByNameRequest
        {
            RoomName = keyword,
        });
    }

    public void ResetSearch()
    {
        _searchInput.text = string.Empty;
    }
}
