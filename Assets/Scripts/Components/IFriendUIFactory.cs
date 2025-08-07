using UnityEngine;

public interface IFriendUIFactory
{
    GameObject CreateItem(Transform parent);
    void SetupItem(GameObject item, Friend friend);
}
