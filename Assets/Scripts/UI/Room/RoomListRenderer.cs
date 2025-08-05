using System;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomListRenderer : MonoBehaviour
{
    [Header("Scroll View")]
    [SerializeField] private Transform _contentParent;
    [SerializeField] private GameObject _roomPrefab;
    [Header("Paging")]
    [SerializeField] private int _itemsPerPage = 6;
    [SerializeField] private Button _prevButton;
    [SerializeField] private Button _nextButton;
    [SerializeField] private TMP_Text _pageIndicatorText;

    private RoomInfo[] _allRooms;
    private int _currentPage = 0;
    private int _totalPages = 0;

    private void Awake()
    {
        _prevButton.onClick.AddListener(OnPrevPage);
        _nextButton.onClick.AddListener(OnNextPage);
    }

    public void RenderRooms(RoomInfo[] roomList)
    {
        _allRooms = roomList ?? Array.Empty<RoomInfo>();
        _currentPage = 0;
        _totalPages = Mathf.CeilToInt((float)_allRooms.Length / _itemsPerPage);
        RefreshPage();
    }

    public void RefreshPage()
    {
        for (int i = _contentParent.childCount - 1; i >= 0; i--)
        {
            Destroy(_contentParent.GetChild(i).gameObject);
        }

        int startIdx = _currentPage * _itemsPerPage;
        int count = Mathf.Min(_itemsPerPage, _allRooms.Length - startIdx);

        for (int i = 0; i < count; i++)
        {
            var room = _allRooms[startIdx + i];
            GameObject button = Instantiate(_roomPrefab, _contentParent);

            var name = button.transform.Find("RoomName")?.GetComponent<TMP_Text>();
            if (name != null) name.text = room.RoomName;

            var capacity = button.transform.Find("Capacity")?.GetComponent<TextMeshProUGUI>();
            if (capacity != null) capacity.text = $"{room.CurrentPlayers}/{room.MaxPlayers}";

            var privateIcon = button.transform.Find("PrivateIcon").GetComponent<Image>();
            if (room.RoomType == (short)RoomType.Public) CreateRoomUI.Instance.SetImageAlpha(0f, privateIcon);
            else if (room.RoomType == (short)RoomType.Private) CreateRoomUI.Instance.SetImageAlpha(255f, privateIcon);

            var enterButton = button.transform.Find("Enter").GetComponent<Button>();
            enterButton.onClick.AddListener(() => OnJoinRoom(room.RoomID, ""));
        }

        _pageIndicatorText.text = $"{_currentPage + 1} / {_totalPages}";
        _prevButton.gameObject.SetActive(_currentPage > 0);
        _nextButton.gameObject.SetActive(_currentPage < _totalPages - 1);
    }

    private void OnPrevPage()
    {
        if (_currentPage > 0)
        {
            _currentPage--;
            RefreshPage();
        }
    }

    private void OnNextPage()
    {
        if (_currentPage < _totalPages - 1)
        {
            _currentPage++;
            RefreshPage();
        }
    }

    private void OnJoinRoom(string RoomID, string password)
    {
        NetworkManager.Instance.SendMessage(new PlayerJoinRoomRequest
        {
            RoomId = RoomID,
            Password = password
        });
    }
}
