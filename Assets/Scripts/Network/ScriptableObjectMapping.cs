using System;
using System.Collections.Generic;
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
    [SerializeField] private SOMap[] _enemyMap;
    [SerializeField] private SOMap[] _itemSourceMap;
    [SerializeField] private SOMap[] _itemMap;
    [SerializeField] private SOMap[] _recipeMap;
    [SerializeField] private SOMap[] _stationMap;

    // Internal cache for fast lookup
    private Dictionary<string, ScriptableObject> _lookup = new();

    private void OnEnable()
    {
        _lookup.Clear();
        CacheMap(_enemyMap);
        CacheMap(_itemSourceMap);
        CacheMap(_itemMap);
        CacheMap(_recipeMap);
        CacheMap(_stationMap);
    }

    private void CacheMap(SOMap[] map)
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
