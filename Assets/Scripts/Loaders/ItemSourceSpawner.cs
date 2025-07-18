using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static LevelConfig;

/// <summary>
/// Handles spawning of item source based on configurable parameters with support for different spawn strategies
/// </summary>
public class ItemSourceSpawner : MonoBehaviour
{
    [SerializeField] private bool _debugLogsEnabled = false;
    
    private ItemSourceConfig[] _itemSourceConfigs;
    private int[] _maxCapacities;
    private int[] _currentAmounts;

    [SerializeField] private float _maxSpawnInterval = 10f;
    [SerializeField] private float _minSpawnInterval = 5f;
    private float _spawnInterval = 5f;

    // Spawn strategy delegate - can be changed at runtime
    public delegate ItemSourceConfig GetItemSourceConfig(ItemSourceConfig[] itemSourceConfigs, int[] maxCapacities, int[] currentAmounts);
    private GetItemSourceConfig _spawnStrategy;

    /// <summary>
    /// Initialize the spawner with configuration data
    /// </summary>
    /// <param name="itemSourceSettings">List of item source configuration settings</param>
    /// <param name="initialStrategy">Optional initial spawn strategy</param>
    public void Initialize(IReadOnlyList<ItemSourceSetting> itemSourceSettings, SpawnStrategy initialStrategy = SpawnStrategy.RandomAvailable)
    {
        ValidateItemSourceSettings(itemSourceSettings);

        int count = itemSourceSettings.Count;
        _itemSourceConfigs = new ItemSourceConfig[count];
        _maxCapacities = new int[count];
        _currentAmounts = new int[count];

        for (int i = 0; i < count; i++)
        {
            var setting = itemSourceSettings[i];
            _itemSourceConfigs[i] = setting.Config;
            _maxCapacities[i] = setting.MaxCapacity;
            _currentAmounts[i] = 0;

            if (_debugLogsEnabled)
            {
                Debug.Log($"Registered item source: {setting.Config.name} with max capacity {setting.MaxCapacity}");
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

            ItemSourceConfig itemSourceConfig = TrySpawnItemSource();
            if (itemSourceConfig == null)
            {
                if (_debugLogsEnabled)
                    Debug.Log("Không có item source khả dụng để spawn");
                continue;
            }

            GridBuilder itemSourceGrid = GridBuilderFactory.Instance.GetBuilder(GridBuilderFactory.BuilderNames[1]);
            if (itemSourceGrid == null)
            {
                Debug.LogWarning("Không lấy được GridBuilder cho item source");
                continue;
            }

            GridCellObject gridCell = itemSourceGrid.GetRandomNonoverlapCell();
            if (gridCell == null)
            {
                if (_debugLogsEnabled)
                    Debug.Log("Không có ô trống để spawn item source");
                continue;
            }

            Vector2 worldPosition = itemSourceGrid.GetWorldPosition(gridCell);
            ItemSourceController itemSourceController = Instantiate(itemSourceConfig.Prefab, worldPosition, Quaternion.identity);
            itemSourceController.Initialize(this, itemSourceConfig);

            itemSourceGrid.ReleaseCell(gridCell);
            _spawnInterval = UnityEngine.Random.Range(_minSpawnInterval, _maxSpawnInterval);
        }
    }
    
    /// <summary>
    /// Attempt to spawn an item source based on the current strategy
    /// </summary>
    /// <returns>The item source config to spawn, or null if no valid spawn</returns>
    public ItemSourceConfig TrySpawnItemSource()
    {
        if (_spawnStrategy == null)
        {
            Debug.LogWarning("Spawner not initialized or strategy not set");
            return null;
        }

        var itemSourceToSpawn = _spawnStrategy.Invoke(_itemSourceConfigs, _maxCapacities, _currentAmounts);
        if (itemSourceToSpawn != null)
        {
            int index = Array.IndexOf(_itemSourceConfigs, itemSourceToSpawn);
            if (index >= 0)
            {
                _currentAmounts[index]++;

                if (_debugLogsEnabled)
                {
                    Debug.Log($"Spawning item source: {itemSourceToSpawn.name}. Current count: {_currentAmounts[index]}/{_maxCapacities[index]}");
                }
            }
        }

        return itemSourceToSpawn;
    }
    
    /// <summary>
    /// Notify the spawner that an item source was destroyed/removed
    /// </summary>
    /// <param name="itemSourceConfig">Config of item source that was removed</param>
    public void NotifyItemSourceRemoved(ItemSourceConfig itemSourceConfig)
    {
        int index = Array.IndexOf(_itemSourceConfigs, itemSourceConfig);
        if (index >= 0)
        {
            _currentAmounts[index] = Mathf.Max(0, _currentAmounts[index] - 1);
            
            if (_debugLogsEnabled)
            {
                Debug.Log($"Item Source removed: {itemSourceConfig.name}. Current count: {_currentAmounts[index]}/{_maxCapacities[index]}");
            }
        }
        else
        {
            Debug.LogWarning($"Attempted to remove unregistered item source: {itemSourceConfig?.name}");
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
                _spawnStrategy = GetRandomAvailableItemSource;
                break;
            case SpawnStrategy.RoundRobin:
                _spawnStrategy = CreateRoundRobinStrategy();
                break;
            // case SpawnStrategy.WeightedRandom:
            //     _spawnStrategy = GetWeightedRandomItemSource;
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
    public void RegisterCustomStrategy(GetItemSourceConfig customStrategy)
    {
        _spawnStrategy = customStrategy;
        
        if (_debugLogsEnabled)
        {
            Debug.Log("Custom spawn strategy registered");
        }
    }
    
    /// <summary>
    /// Get current count of a specific item source type
    /// </summary>
    public int GetItemSourceCount(ItemSourceConfig itemSourceConfig)
    {
        int index = Array.IndexOf(_itemSourceConfigs, itemSourceConfig);
        return index >= 0 ? _currentAmounts[index] : 0;
    }
    
    /// <summary>
    /// Get maximum capacity for a specific item source type
    /// </summary>
    public int GetItemSourceCapacity(ItemSourceConfig itemSourceConfig)
    {
        int index = Array.IndexOf(_itemSourceConfigs, itemSourceConfig);
        return index >= 0 ? _maxCapacities[index] : 0;
    }
    
    #region DEFAULT SPAWN STRATEGIES
    
    private ItemSourceConfig GetRandomAvailableItemSource(ItemSourceConfig[] itemSourceConfigs, int[] maxCapacities, int[] currentAmounts)
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
        
        return resultIndex >= 0 ? itemSourceConfigs[resultIndex] : null;
    }
    
    private GetItemSourceConfig CreateRoundRobinStrategy()
    {
        int roundRobinIndex = 0;
        
        return (itemSourceConfigs, maxCapacities, currentAmounts) =>
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
            
            var result = itemSourceConfigs[roundRobinIndex];
            roundRobinIndex = (roundRobinIndex + 1) % currentAmounts.Length;
            
            return result;
        };
    }
    
    // private int _roundRobinIndex = 0;
    // private ItemSourceConfig GetRoundRobinItemSource(ItemSourceConfig[] itemSourceConfigs, int[] maxCapacities, int[] currentAmounts)
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

    //     var result = itemSourceConfigs[_roundRobinIndex];
    //     _roundRobinIndex = (_roundRobinIndex + 1) % currentAmounts.Length;

    //     return result;
    // }
    
    // private ItemSourceConfig GetWeightedRandomItemSource(ItemSourceConfig[] _itemSourceConfigs, int[] _maxCapacities, int[] _currentAmounts)
    // {
    //     // Calculate total weight of available item sources
    //     float totalWeight = 0;
    //     var availableItemSources = new List<(int index, float weight)>();
        
    //     for (int i = 0; i < _currentAmounts.Length; i++)
    //     {
    //         if (_currentAmounts[i] < _maxCapacities[i])
    //         {
    //             float weight = _itemSourceConfigs[i].SpawnWeight;
    //             availableItemSources.Add((i, weight));
    //             totalWeight += weight;
    //         }
    //     }
        
    //     if (availableItemSources.Count == 0) return null;
        
    //     // Select based on weight
    //     float randomValue = UnityEngine.Random.Range(0f, totalWeight);
    //     float cumulativeWeight = 0;
        
    //     foreach (var item source in availableItemSources)
    //     {
    //         cumulativeWeight += item source.weight;
    //         if (randomValue <= cumulativeWeight)
    //         {
    //             return _itemSourceConfigs[item source.index];
    //         }
    //     }
        
    //     return _itemSourceConfigs[availableItemSources[^1].index]; // Fallback to last element
    // }
    
    #endregion
    
    #region VALIDATION
    
    private void ValidateItemSourceSettings(IReadOnlyList<ItemSourceSetting> itemSourceSettings)
    {
        if (itemSourceSettings == null || itemSourceSettings.Count == 0)
        {
            throw new ArgumentException("Item Source settings cannot be null or empty", nameof(itemSourceSettings));
        }
        
        var uniqueConfigs = new HashSet<ItemSourceConfig>();
        foreach (var setting in itemSourceSettings)
        {
            if (setting.Config == null)
            {
                throw new ArgumentException("Item Source config cannot be null");
            }
            
            if (!uniqueConfigs.Add(setting.Config))
            {
                throw new ArgumentException($"Duplicate item source config detected: {setting.Config.name}");
            }
            
            if (setting.MaxCapacity <= 0)
            {
                throw new ArgumentException($"Max capacity must be positive for item source: {setting.Config.name}");
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
        RandomAvailable,  // Random selection from available item sources
        RoundRobin,       // Cycle through item sources in order
        // WeightedRandom    // Random selection weighted by item source spawn weights
    }
    
    #endregion
}