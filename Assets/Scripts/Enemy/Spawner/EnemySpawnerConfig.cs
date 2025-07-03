using UnityEngine;

public abstract class EnemySpawnerConfig : ScriptableObject
{
    [field: SerializeField] public EnemyConfig[] EnemyConfigsToSpawn;
    public abstract void SpawnAt(Transform positionToSpawn);
}