using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }
    [SerializeField] LevelConfig _config;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    void Start()
    {
        LoadLevel(_config);
    }

    private void LoadLevel(LevelConfig config)
    {
        GameObject map = MapLoader.Instance.RenderMap(config.MapPrefab, Vector3.zero);
        
        StationSpawner[] stationSpawners = map.GetComponentsInChildren<StationSpawner>(true);
        for (int i = 0; i < stationSpawners.Length; i++)
        {
            if (stationSpawners[i].Config.Type == StationType.Furnace)
            {
                stationSpawners[i].Spawn(config.IngotRecipes);
            }
            else if (stationSpawners[i].Config.Type == StationType.AlchemyStation)
            {
                stationSpawners[i].Spawn(config.FinalRecipes);
            }
        }
    }
}