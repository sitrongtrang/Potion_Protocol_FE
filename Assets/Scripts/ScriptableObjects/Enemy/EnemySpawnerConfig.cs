using UnityEngine;

[CreateAssetMenu(fileName = "EnemySpawnerConfig", menuName = "Scriptable Objects/Enemy/Spawner/Config")]
public class EnemySpawnerConfig : ScriptableObject
{
    [field: SerializeField] public float MinSpawnInterval { get; private set; }
    [field: SerializeField] public float MaxSpawnInterval { get; private set; }

    public void Spawn(EnemySpawner spawner, EnemyConfig config, Vector2 position, int positionIndex, int typeIndex)
    {
        EnemyController enemy = Instantiate(config.Prefab, spawner.transform.position, Quaternion.identity);
        enemy.Initialize(config, spawner, position, positionIndex, typeIndex);
    }
}