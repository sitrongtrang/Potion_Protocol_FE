using System;
using UnityEngine;

public static class EventBus
{
    public static Action<Transform> UpdateTarget;
    public static void AddTargetToCamera(Transform transform)
    {
        UpdateTarget?.Invoke(transform);
    }
};