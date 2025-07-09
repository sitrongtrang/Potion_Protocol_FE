using UnityEngine;

[CreateAssetMenu(fileName = "EnemySpawnerConfig", menuName = "Scriptable Objects/Enemy/Spawner/Config")]
public class EnemySpawnerConfig : ScriptableObject
{
    [field: SerializeField] public float MinSpawnInterval { get; private set; }
    [field: SerializeField] public float MaxSpawnInterval { get; private set; }
    [field: SerializeField] public EnemyConfig[] EnemyConfigsToSpawn { get; private set; }
    public void Spawn(EnemySpawner spawner, Vector2 position, int indexPosition)
    {
        int index = Random.Range(0, EnemyConfigsToSpawn.Length);
        GameObject enemy = Instantiate(EnemyConfigsToSpawn[index].Prefab.gameObject, spawner.transform.position, Quaternion.identity);
        enemy.GetComponent<EnemyController>().Initialize(EnemyConfigsToSpawn[index], spawner, position, indexPosition);
    }
}