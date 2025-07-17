using System;
using UnityEngine;

public static class EventBus
{
    public static Action<Transform> UpdateTarget;
    public static void AddTargetToCamera(Transform transform)
    {
        UpdateTarget?.Invoke(transform);
    }
    public static Action<int, GameObject> UpdateItem;
    public static void UpdateInventoryUI(int id, GameObject item)
    {
        UpdateItem?.Invoke(id, item);
    }
    public static Action<int> UpdateChoosingSlot;
    public static void OnSlotChanged(int idx)
    {
        UpdateChoosingSlot?.Invoke(idx);
    }
    

};