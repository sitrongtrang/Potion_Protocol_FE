using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StationConfig", menuName = "Scriptable Objects/StationConfig")]
public class StationConfig : BaseSpawnConfig
{
    [SerializeField] private StationController _prefab;
    public StationController Prefab => _prefab;

    [SerializeField] private List<RecipeConfig> _recipes;
    public List<RecipeConfig> Recipes => _recipes;

    public override void Spawn(Vector3 position)
    {
        StationController station = Instantiate(_prefab, position, Quaternion.identity);
        station.Initialize(this);
    }
}