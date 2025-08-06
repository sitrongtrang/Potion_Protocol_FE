using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "ScriptableObjectMapping", menuName = "Scriptable Objects/ScriptableObjectMapping")]
public class ScriptableObjectMapping : ScriptableObject
{
    [Serializable]
    public class SOMap
    {
        public string Id;
        public ScriptableObject SO;
    }

    [Header("Prefabs")]
    [SerializeField] private NetworkBehaviour _enemyPrefab;
    [SerializeField] private NetworkBehaviour _itemSourcePrefab;
    [SerializeField] private NetworkBehaviour _itemPrefab;
    [SerializeField] private NetworkBehaviour _stationPrefab;

    [Header("Mappings")]
    [SerializeField] private List<SOMap> _enemyMap = new();
    [SerializeField] private List<SOMap> _itemSourceMap = new();
    [SerializeField] private List<SOMap> _itemMap = new();
    [SerializeField] private List<SOMap> _recipeMap = new();
    [SerializeField] private List<SOMap> _stationMap = new();

    // Internal cache for fast lookup
    private Dictionary<string, ScriptableObject> _lookup = new();

    private void OnEnable()
    {
        _lookup.Clear();
    }

    public void InitializeMapping(ScriptableObject[] scriptableObjects)
    {
        for (int i = 0; i < scriptableObjects.Length; i++)
        {
            ScriptableObject config = scriptableObjects[i];
            if (config is EnemyConfig enemyConfig)
            {
                _enemyMap.Add(new SOMap()
                {
                    Id = enemyConfig.Id,
                    SO = config
                });
            }
            else if (config is ItemSourceConfig itemSourceConfig)
            {
                _itemSourceMap.Add(new SOMap()
                {
                    Id = itemSourceConfig.Id,
                    SO = config
                });
            }
            else if (config is ItemConfig itemConfig)
            {
                _itemMap.Add(new SOMap()
                {
                    Id = itemConfig.Id,
                    SO = config
                });
            }
            else if (config is RecipeConfig recipeConfig)
            {
                _recipeMap.Add(new SOMap()
                {
                    Id = recipeConfig.Id,
                    SO = config
                });
            }
            else if (config is StationConfig stationConfig)
            {
                _stationMap.Add(new SOMap()
                {
                    Id = stationConfig.Id,
                    SO = config
                });
            }
            else
            {
                continue;
            }
        }

        CacheMap(_enemyMap);
        CacheMap(_itemSourceMap);
        CacheMap(_itemMap);
        CacheMap(_recipeMap);
        CacheMap(_stationMap);
    }

    private void CacheMap(List<SOMap> map)
    {
        foreach (var entry in map)
        {
            if (!string.IsNullOrEmpty(entry.Id) && entry.SO != null)
            {
                if (!_lookup.ContainsKey(entry.Id))
                {
                    _lookup.Add(entry.Id, entry.SO);
                }
                else
                {
                    Debug.LogWarning($"Duplicate key '{entry.Id}' found in ScriptableObjectMapping.");
                }
            }
        }
    }

    public ScriptableObject GetSO(string id)
    {
        if (_lookup.TryGetValue(id, out var so))
        {
            return so;
        }

        Debug.LogWarning($"No ScriptableObject found with ID '{id}'.");
        return null;
    }

    // Access to specific prefabs
    public NetworkBehaviour EnemyPrefab => _enemyPrefab;
    public NetworkBehaviour ItemSourcePrefab => _itemSourcePrefab;
    public NetworkBehaviour ItemPrefab => _itemPrefab;
    public NetworkBehaviour StationPrefab => _stationPrefab;
}
