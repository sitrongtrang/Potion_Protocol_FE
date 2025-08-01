using System;
using UnityEngine;

public abstract class NetworkBehaviour : MonoBehaviour
{
    public abstract void Initialize(ScriptableObject scriptableObject);
}