using System;
using UnityEngine;

public class TrackedObject : MonoBehaviour
{
    public string Id;
    public Action<string> OnDestroyed;

    void OnDestroy()
    {
        OnDestroyed?.Invoke(Id);
    }
}