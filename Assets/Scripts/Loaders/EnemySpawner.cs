using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private EnemySpawnerConfig _enemySpawnerConfig;
    [SerializeField] private EnemyController _enemyPrefab;
    private float _currentCooldown;
    private List<Transform> _positionsToSpawn;
    private List<EnemyConfig> _enemiesToSpawn;
    private List<int> _unspawnedEnemyIndices = new();
    private List<int> _unoccupiedPositionIndices = new();

    public EnemySpawnerConfig Config => _enemySpawnerConfig;

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

            Spawn(this, _enemiesToSpawn[selectedEnemy], _positionsToSpawn[selectedPosition].position, selectedPosition, selectedEnemy);
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

    public void Spawn(EnemySpawner spawner, EnemyConfig config, Vector2 position, int positionIndex, int typeIndex)
    {
        EnemyController enemy = Instantiate(_enemyPrefab, spawner.transform.position, Quaternion.identity);
        enemy.Initialize(config, spawner, position, positionIndex, typeIndex);
    }
}