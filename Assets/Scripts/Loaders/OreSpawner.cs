using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static LevelConfig;

/// <summary>
/// Handles spawning of ore based on configurable parameters with support for different spawn strategies
/// </summary>
public class OreSpawner : MonoBehaviour
{
    [SerializeField] private bool _debugLogsEnabled = false;
    
    private OreConfig[] _oreConfigs;
    private int[] _maxCapacities;
    private int[] _currentAmounts;

    [SerializeField] private float _maxSpawnInterval = 10f;
    [SerializeField] private float _minSpawnInterval = 5f;
    private float _spawnInterval = 5f;

    // Spawn strategy delegate - can be changed at runtime
    public delegate OreConfig GetOreConfig(OreConfig[] oreConfigs, int[] maxCapacities, int[] currentAmounts);
    private GetOreConfig _spawnStrategy;

    /// <summary>
    /// Initialize the spawner with configuration data
    /// </summary>
    /// <param name="oreSettings">List of ore configuration settings</param>
    /// <param name="initialStrategy">Optional initial spawn strategy</param>
    public void Initialize(IReadOnlyList<OreSetting> oreSettings, SpawnStrategy initialStrategy = SpawnStrategy.RandomAvailable)
    {
        ValidateOreSettings(oreSettings);

        int count = oreSettings.Count;
        _oreConfigs = new OreConfig[count];
        _maxCapacities = new int[count];
        _currentAmounts = new int[count];

        for (int i = 0; i < count; i++)
        {
            var setting = oreSettings[i];
            _oreConfigs[i] = setting.Config;
            _maxCapacities[i] = setting.MaxCapacity;
            _currentAmounts[i] = 0;

            if (_debugLogsEnabled)
            {
                Debug.Log($"Registered ore: {setting.Config.name} with max capacity {setting.MaxCapacity}");
            }
        }

        SetDefaultSpawnStrategy(initialStrategy);

        StartCoroutine(SpawnEnumerator());
    }

    private IEnumerator SpawnEnumerator()
    {
        while (true)
        {
            yield return new WaitForSeconds(_spawnInterval);

            OreConfig oreConfig = TrySpawnOre();
            if (oreConfig == null)
            {
                if (_debugLogsEnabled)
                    Debug.Log("Không có ore khả dụng để spawn");
                continue;
            }

            GridBuilder oreGrid = GridBuilderFactory.Instance.GetBuilder(GridBuilderFactory.BuilderNames[1]);
            if (oreGrid == null)
            {
                Debug.LogWarning("Không lấy được GridBuilder cho ore");
                continue;
            }

            GridCellObject gridCell = oreGrid.GetRandomNonoverlapCell();
            if (gridCell == null)
            {
                if (_debugLogsEnabled)
                    Debug.Log("Không có ô trống để spawn ore");
                continue;
            }

            Vector2 worldPosition = oreGrid.GetWorldPosition(gridCell);
            OreController oreController = Instantiate(oreConfig.Prefab, worldPosition, Quaternion.identity);
            oreController.Initialize(this, oreConfig);

            oreGrid.ReleaseCell(gridCell);
            _spawnInterval = UnityEngine.Random.Range(_minSpawnInterval, _maxSpawnInterval);
        }
    }
    
    /// <summary>
    /// Attempt to spawn an ore based on the current strategy
    /// </summary>
    /// <returns>The ore config to spawn, or null if no valid spawn</returns>
    public OreConfig TrySpawnOre()
    {
        if (_spawnStrategy == null)
        {
            Debug.LogWarning("Spawner not initialized or strategy not set");
            return null;
        }

        var oreToSpawn = _spawnStrategy.Invoke(_oreConfigs, _maxCapacities, _currentAmounts);
        if (oreToSpawn != null)
        {
            int index = Array.IndexOf(_oreConfigs, oreToSpawn);
            if (index >= 0)
            {
                _currentAmounts[index]++;

                if (_debugLogsEnabled)
                {
                    Debug.Log($"Spawning ore: {oreToSpawn.name}. Current count: {_currentAmounts[index]}/{_maxCapacities[index]}");
                }
            }
        }

        return oreToSpawn;
    }
    
    /// <summary>
    /// Notify the spawner that an ore was destroyed/removed
    /// </summary>
    /// <param name="oreConfig">Config of ore that was removed</param>
    public void NotifyOreRemoved(OreConfig oreConfig)
    {
        int index = Array.IndexOf(_oreConfigs, oreConfig);
        if (index >= 0)
        {
            _currentAmounts[index] = Mathf.Max(0, _currentAmounts[index] - 1);
            
            if (_debugLogsEnabled)
            {
                Debug.Log($"Ore removed: {oreConfig.name}. Current count: {_currentAmounts[index]}/{_maxCapacities[index]}");
            }
        }
        else
        {
            Debug.LogWarning($"Attempted to remove unregistered ore: {oreConfig?.name}");
        }
    }
    
    /// <summary>
    /// Change the spawn strategy at runtime
    /// </summary>
    /// <param name="strategy">The new strategy to use</param>
    public void SetDefaultSpawnStrategy(SpawnStrategy strategy)
    {
        switch (strategy)
        {
            case SpawnStrategy.RandomAvailable:
                _spawnStrategy = GetRandomAvailableOre;
                break;
            case SpawnStrategy.RoundRobin:
                _spawnStrategy = CreateRoundRobinStrategy();
                break;
            // case SpawnStrategy.WeightedRandom:
            //     _spawnStrategy = GetWeightedRandomOre;
            //     break;
            default:
                throw new ArgumentOutOfRangeException(nameof(strategy), strategy, null);
        }
        
        if (_debugLogsEnabled)
        {
            Debug.Log($"Spawn strategy changed to: {strategy}");
        }
    }
    
    /// <summary>
    /// Register a custom spawn strategy
    /// </summary>
    /// <param name="customStrategy">Custom strategy function</param>
    public void RegisterCustomStrategy(GetOreConfig customStrategy)
    {
        _spawnStrategy = customStrategy;
        
        if (_debugLogsEnabled)
        {
            Debug.Log("Custom spawn strategy registered");
        }
    }
    
    /// <summary>
    /// Get current count of a specific ore type
    /// </summary>
    public int GetOreCount(OreConfig oreConfig)
    {
        int index = Array.IndexOf(_oreConfigs, oreConfig);
        return index >= 0 ? _currentAmounts[index] : 0;
    }
    
    /// <summary>
    /// Get maximum capacity for a specific ore type
    /// </summary>
    public int GetOreCapacity(OreConfig oreConfig)
    {
        int index = Array.IndexOf(_oreConfigs, oreConfig);
        return index >= 0 ? _maxCapacities[index] : 0;
    }
    
    #region DEFAULT SPAWN STRATEGIES
    
    private OreConfig GetRandomAvailableOre(OreConfig[] oreConfigs, int[] maxCapacities, int[] currentAmounts)
    {
        int resultIndex = -1;
        int validOptionsCount = 0;
        
        for (int i = 0; i < currentAmounts.Length; i++)
        {
            if (currentAmounts[i] < maxCapacities[i])
            {
                validOptionsCount++;
                
                if (UnityEngine.Random.Range(0, validOptionsCount) == 0)
                {
                    resultIndex = i;
                }
            }
        }
        
        return resultIndex >= 0 ? oreConfigs[resultIndex] : null;
    }
    
    private GetOreConfig CreateRoundRobinStrategy()
    {
        int roundRobinIndex = 0;
        
        return (oreConfigs, maxCapacities, currentAmounts) =>
        {
            int initialIndex = roundRobinIndex;
            bool found = false;
            
            do
            {
                if (currentAmounts[roundRobinIndex] < maxCapacities[roundRobinIndex])
                {
                    found = true;
                    break;
                }
                
                roundRobinIndex = (roundRobinIndex + 1) % currentAmounts.Length;
            } 
            while (roundRobinIndex != initialIndex);
            
            if (!found) return null;
            
            var result = oreConfigs[roundRobinIndex];
            roundRobinIndex = (roundRobinIndex + 1) % currentAmounts.Length;
            
            return result;
        };
    }
    
    // private int _roundRobinIndex = 0;
    // private OreConfig GetRoundRobinOre(OreConfig[] oreConfigs, int[] maxCapacities, int[] currentAmounts)
    // {
    //     int initialIndex = _roundRobinIndex;
    //     bool found = false;

    //     do
    //     {
    //         if (currentAmounts[_roundRobinIndex] < maxCapacities[_roundRobinIndex])
    //         {
    //             found = true;
    //             break;
    //         }

    //         _roundRobinIndex = (_roundRobinIndex + 1) % currentAmounts.Length;
    //     }
    //     while (_roundRobinIndex != initialIndex);

    //     if (!found) return null;

    //     var result = oreConfigs[_roundRobinIndex];
    //     _roundRobinIndex = (_roundRobinIndex + 1) % currentAmounts.Length;

    //     return result;
    // }
    
    // private OreConfig GetWeightedRandomOre(OreConfig[] _oreConfigs, int[] _maxCapacities, int[] _currentAmounts)
    // {
    //     // Calculate total weight of available ores
    //     float totalWeight = 0;
    //     var availableOres = new List<(int index, float weight)>();
        
    //     for (int i = 0; i < _currentAmounts.Length; i++)
    //     {
    //         if (_currentAmounts[i] < _maxCapacities[i])
    //         {
    //             float weight = _oreConfigs[i].SpawnWeight;
    //             availableOres.Add((i, weight));
    //             totalWeight += weight;
    //         }
    //     }
        
    //     if (availableOres.Count == 0) return null;
        
    //     // Select based on weight
    //     float randomValue = UnityEngine.Random.Range(0f, totalWeight);
    //     float cumulativeWeight = 0;
        
    //     foreach (var ore in availableOres)
    //     {
    //         cumulativeWeight += ore.weight;
    //         if (randomValue <= cumulativeWeight)
    //         {
    //             return _oreConfigs[ore.index];
    //         }
    //     }
        
    //     return _oreConfigs[availableOres[^1].index]; // Fallback to last element
    // }
    
    #endregion
    
    #region VALIDATION
    
    private void ValidateOreSettings(IReadOnlyList<OreSetting> oreSettings)
    {
        if (oreSettings == null || oreSettings.Count == 0)
        {
            throw new ArgumentException("Ore settings cannot be null or empty", nameof(oreSettings));
        }
        
        var uniqueConfigs = new HashSet<OreConfig>();
        foreach (var setting in oreSettings)
        {
            if (setting.Config == null)
            {
                throw new ArgumentException("Ore config cannot be null");
            }
            
            if (!uniqueConfigs.Add(setting.Config))
            {
                throw new ArgumentException($"Duplicate ore config detected: {setting.Config.name}");
            }
            
            if (setting.MaxCapacity <= 0)
            {
                throw new ArgumentException($"Max capacity must be positive for ore: {setting.Config.name}");
            }
        }
    }
    
    #endregion
    
    #region SPAWN STRATEGY ENUM
    
    /// <summary>
    /// Available spawn strategies
    /// </summary>
    public enum SpawnStrategy
    {
        RandomAvailable,  // Random selection from available ores
        RoundRobin,       // Cycle through ores in order
        // WeightedRandom    // Random selection weighted by ore spawn weights
    }
    
    #endregion
}