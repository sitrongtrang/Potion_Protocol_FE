using UnityEngine;

[CreateAssetMenu(fileName = "EnemySpawnerConfig", menuName = "Scriptable Objects/Enemy/Spawner/Config")]
public class EnemySpawnerConfig : ScriptableObject
{
    [field: SerializeField] public float MinSpawnInterval { get; private set; }
    [field: SerializeField] public float MaxSpawnInterval { get; private set; }
}