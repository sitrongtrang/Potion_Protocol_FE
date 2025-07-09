using UnityEngine;

public abstract class BaseSpawnConfig : ScriptableObject
{
    public abstract void Spawn(Vector3 position);
}