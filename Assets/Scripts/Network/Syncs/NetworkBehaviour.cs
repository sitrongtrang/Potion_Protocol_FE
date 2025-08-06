using System;
using UnityEngine;

public abstract class NetworkBehaviour : MonoBehaviour
{
    public string EntityId;

    public abstract void Initialize(string entityId, ScriptableObject scriptableObject);
}