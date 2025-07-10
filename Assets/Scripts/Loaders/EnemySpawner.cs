using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    private float _currentCooldown;
    [SerializeField] private EnemySpawnerConfig _enemySpawnerConfig;
    private List<Transform> _positionsToSpawn;
    private List<EnemyConfig> _enemiesToSpawn;
    private List<int> _unspawnedEnemyIndices = new();
    private List<int> _unoccupiedPositionIndices = new();

    public void Initialize(List<EnemyConfig> enemiesToSpawn, List<Transform> positionsToSpawn)
    {
        _enemiesToSpawn = enemiesToSpawn;
        _positionsToSpawn = positionsToSpawn;

        for (int i = 0; i < enemiesToSpawn.Count; i++)
        {
            _unspawnedEnemyIndices.Add(i);
        }

        for (int i = 0; i < positionsToSpawn.Count; i++)
        {
            _unoccupiedPositionIndices.Add(i);
        }

        _currentCooldown = Random.Range(_enemySpawnerConfig.MinSpawnInterval, _enemySpawnerConfig.MaxSpawnInterval);
    }

    void Update()
    {
        Spawn();
    }

    void Spawn()
    {
        if (_currentCooldown > 0)
        {
            _currentCooldown -= Time.deltaTime;
            return;
        }

        if (_unoccupiedPositionIndices.Count > 0 && _unspawnedEnemyIndices.Count > 0)
        {
            int randomPositionIndex = Random.Range(0, _unoccupiedPositionIndices.Count);
            int selectedPosition = _unoccupiedPositionIndices[randomPositionIndex];
            _unoccupiedPositionIndices.RemoveAt(randomPositionIndex);

            int randomEnemyIndex = Random.Range(0, _unspawnedEnemyIndices.Count);
            int selectedEnemy = _unspawnedEnemyIndices[randomEnemyIndex];
            _unspawnedEnemyIndices.RemoveAt(randomEnemyIndex);

            _enemySpawnerConfig.Spawn(_enemiesToSpawn[selectedEnemy], this, _positionsToSpawn[selectedPosition].position, selectedPosition, selectedEnemy);
            _currentCooldown = Random.Range(_enemySpawnerConfig.MinSpawnInterval, _enemySpawnerConfig.MaxSpawnInterval);
        }

    }
    public void UnoccupiedSpace(int index)
    {
        _unoccupiedPositionIndices.Add(index);
    }

    public void UnspawnedEnemy(int index)
    {
        _unspawnedEnemyIndices.Add(index);
    }
}