using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        StartCoroutine(EndLevel());
    }

    private void LoadLevel(LevelConfig config)
    {
        GameObject map = MapLoader.Instance.RenderMap(config.MapPrefab, Vector3.zero);

        // Spawn & initialize stations
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

        // Initialize enemy spawners
        EnemySpawner[] enemySpawners = map.GetComponentsInChildren<EnemySpawner>(true);
        for (int i = 0; i < enemySpawners.Length; i++)
        {
            enemySpawners[i].Initialize(_config.Enemies);
        }
    }

    private IEnumerator EndLevel()
    {
        yield return new WaitForSeconds(_config.LevelTime);
        SceneManager.LoadScene("LevelResultScene");
    }
}