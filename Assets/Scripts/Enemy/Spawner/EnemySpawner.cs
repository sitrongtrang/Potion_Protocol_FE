using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [field: SerializeField] public EnemySpawnerConfig EnemySpawnerConf { get; private set; }
    [field: SerializeField] public Transform[] PositionsToSpawn { get; private set; }
}