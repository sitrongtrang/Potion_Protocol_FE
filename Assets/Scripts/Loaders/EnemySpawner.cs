using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    private float _currentCooldown;
    [SerializeField] private EnemySpawnerConfig _enemySpawnerConfig;
    [SerializeField] private Transform[] _positionsToSpawn;
    private List<int> _unoccupiedIndices = new();
    
    void Start()
    {
        for (int i = 0; i < _positionsToSpawn.Length; i++)
        {
            _unoccupiedIndices.Add(i);
        }
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

        if (_unoccupiedIndices.Count > 0)
        {
            int randomIndex = Random.Range(0, _unoccupiedIndices.Count);
            int selected = _unoccupiedIndices[randomIndex];
            _unoccupiedIndices.RemoveAt(randomIndex);
            _enemySpawnerConfig.Spawn(this, _positionsToSpawn[selected].position, selected);

            _currentCooldown = Random.Range(_enemySpawnerConfig.MinSpawnInterval, _enemySpawnerConfig.MaxSpawnInterval);
        }

    }
    public void UnoccupiedSpace(int index)
    {
        _unoccupiedIndices.Add(index);
    }
}