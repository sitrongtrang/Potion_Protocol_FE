using UnityEngine;

public abstract class EnemySpawnerConfig : ScriptableObject
{
    [field: SerializeField] public float MinSpawnInterval { get; private set; }
    [field: SerializeField] public float MaxSpawnInterval { get; private set; }
    [field: SerializeField] public EnemyConfig[] EnemyConfigsToSpawn { get; private set; }
}