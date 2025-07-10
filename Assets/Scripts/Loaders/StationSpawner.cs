using System.Collections.Generic;
using UnityEngine;

public class StationSpawner : MonoBehaviour
{
    [SerializeField] private StationConfig _config;
    public StationConfig Config => _config;

    public void Spawn(List<RecipeConfig> recipes)
    {
        Config.Spawn(transform.position, recipes);
    }
}