using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SearchRoomByName : MonoBehaviour
{
    public Transform contentParent;
    public TMP_InputField searchInput;

    void Start()
    {
        searchInput.onValueChanged.AddListener(FilterRooms);
    }

    public void FilterRooms(string keyword)
    {
        keyword = keyword.ToLower();

        foreach (Transform roomButton in contentParent)
        {
            TextMeshProUGUI roomName = roomButton.GetComponentInChildren<TextMeshProUGUI>();

            if (roomName != null)
            {
                bool isMatch = roomName.text.ToLower().Contains(keyword);
                roomButton.gameObject.SetActive(isMatch);
            }
        }
    }
}
