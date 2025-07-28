using System.Collections.Generic;
using UnityEngine;

public class StationSpawner : MonoBehaviour
{
    [SerializeField] private StationController _stationPrefab;
    public StationController Prefab => _stationPrefab;

    public StationController Spawn(List<RecipeConfig> recipes)
    {
        StationController station = Instantiate(_stationPrefab, transform.position, Quaternion.identity);
        station.Initialize(recipes);
        return station;
    }
}