using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FriendListFactory : IFriendUIFactory
{
    GameObject _prefab;
    System.Action<string> _onInvite;
    System.Action<string> _onRemove;

    public FriendListFactory(GameObject prefab, System.Action<string> onInvite, System.Action<string> onRemove)
    {
        _prefab = prefab;
        _onInvite = onInvite;
        _onRemove = onRemove;
    }

    public GameObject CreateItem(Transform parent)
    {
        return GameObject.Instantiate(_prefab, parent);
    }

    public void SetupItem(GameObject item, Friend friend)
    {
        var ui = item.GetComponent<FriendItemUI>();
        ui.NameText.GetComponent<TMP_Text>().text = friend.FriendDisplayName;

        ui.InviteButton.GetComponent<Button>().onClick.RemoveAllListeners();
        ui.InviteButton.GetComponent<Button>().onClick.AddListener(() => _onInvite?.Invoke(friend.Id));

        ui.RemoveButton.GetComponent<Button>().onClick.RemoveAllListeners();
        ui.RemoveButton.GetComponent<Button>().onClick.AddListener(() => _onRemove?.Invoke(friend.Id));
    }
}
