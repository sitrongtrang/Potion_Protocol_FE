using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class RoomListRenderer : MonoBehaviour
{
    [SerializeField] private Transform _contentParent;
    [SerializeField] private GameObject _roomPrefab;

    public void RenderRooms(RoomInfo[] roomList)
    {
        for (int i = _contentParent.childCount - 1; i >= 0; i--)
        {
            Destroy(_contentParent.GetChild(i).gameObject);
        }

        for (int i = 0; i < roomList.Length; i++)
        {
            RoomInfo room = roomList[i];
            GameObject button = Instantiate(_roomPrefab, _contentParent);

            var name = button.transform.Find("RoomName")?.GetComponent<TextMeshProUGUI>();
            if (name != null) name.text = room.RoomName;

            var capacity = button.transform.Find("Capacity")?.GetComponent<TextMeshProUGUI>();
            if (capacity != null) capacity.text = $"{room.CurrentPlayers}/{room.MaxPlayers}";

            string RoomID = room.RoomID;
        }
    }
}
