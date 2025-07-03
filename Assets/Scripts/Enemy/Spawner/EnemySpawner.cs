using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [field: SerializeField] public Transform[] PositionsToSpawn { get; private set; }
}