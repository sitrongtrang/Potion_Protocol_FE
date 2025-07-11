using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StationConfig", menuName = "Scriptable Objects/StationConfig")]
public class StationConfig : ScriptableObject
{
    [SerializeField] private StationType _type;
    public StationType Type => _type;

    [SerializeField] private StationController _prefab;
    public StationController Prefab => _prefab;

    public void Spawn(Vector2 position, List<RecipeConfig> recipes)
    {
        StationController station = Instantiate(_prefab, position, Quaternion.identity);
        station.Initialize(this, recipes);
    }
}