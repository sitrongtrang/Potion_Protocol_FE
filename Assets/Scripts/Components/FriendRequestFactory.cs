using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FriendRequestFactory : IFriendUIFactory
{
    GameObject _prefab;
    System.Action<string> _onAccept;
    System.Action<string> _onDecline;

    public FriendRequestFactory(GameObject prefab, System.Action<string> onAccept, System.Action<string> onDecline)
    {
        _prefab = prefab;
        _onAccept = onAccept;
        _onDecline = onDecline;
    }

    public GameObject CreateItem(Transform parent)
    {
        return GameObject.Instantiate(_prefab, parent);
    }

    public void SetupItem(GameObject item, Friend friend)
    {
        var ui = item.GetComponent<FriendRequestItemUI>();
        ui.NameText.GetComponent<TMP_Text>().text = friend.FriendDisplayName;

        ui.AcceptButton.GetComponent<Button>().onClick.RemoveAllListeners();
        ui.AcceptButton.GetComponent<Button>().onClick.AddListener(() =>
        {
            _onAccept?.Invoke(friend.Id);
            Debug.Log("✅ Accept " + friend.Id);
        });

        ui.DeclineButton.GetComponent<Button>().onClick.RemoveAllListeners();
        ui.DeclineButton.GetComponent<Button>().onClick.AddListener(() =>
        {
            _onDecline?.Invoke(friend.Id);
            Debug.Log("❌ Decline " + friend.Id);
        });
    }
}
