using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MyRequestFactory : IFriendUIFactory
{
    GameObject _prefab;

    public MyRequestFactory(GameObject prefab)
    {
        _prefab = prefab;
    }

    public GameObject CreateItem(Transform parent)
    {
        return GameObject.Instantiate(_prefab, parent);
    }

    public void SetupItem(GameObject item, Friend friend)
    {
        var ui = item.GetComponent<MyRequestItemUI>();
        ui.NameText.GetComponent<TMP_Text>().text = friend.FriendDisplayName;

        ui.CancelButton.GetComponent<Button>().onClick.RemoveAllListeners();
        ui.CancelButton.GetComponent<Button>().onClick.AddListener(() =>
        {
            Debug.Log("🚫 Cancel request to " + friend.Id);
        });
    }
}
