using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[ExecuteInEditMode]
public class LevelInfoSaver : MonoBehaviour
{
    [Serializable]
    public class ItemSourceData
    {
        public string Id;
        public int MaxCapacity;
    }

    [Serializable]
    public class ItemSourceDataList
    {
        public List<ItemSourceData> ItemSources;
    }

    [Serializable]
    public class Location
    {
        public float LocationX, LocationY;
    }

    [Serializable]
    public class StationData
    {
        public StationType Type;
        public Location StationLocation;
    }

    [Serializable]
    public class EnemySpawnerData
    {
        public float MinSpawnInterval, MaxSpawnInterval;
        public Location SpawnerLocation;
    }

    [Serializable]
    public class LevelData
    {
        public int LevelNumber;
        public List<string> EnemyIds;
        public ItemSourceDataList ItemSourceList;
        public List<string> RecipeIds;
        public float LevelTime;
        public List<int> ScoreThresholds;
        public Location PlayerSpawnLocation;
        public List<StationData> Stations;
        public List<EnemySpawnerData> EnemySpawners;
        public List<Location> PatrolCenters;
        public Location SubmissionPoint;
        public ColliderSaver.ColliderDataList MapColliders;
        public ColliderSaver.ColliderDataList StationProtections;
    }

    private LevelData _levelData;

    void OnEnable()
    {
        LevelManager.Instance.OnLevelInitialized += SaveData;
    }

    void OnDisable()
    {
        LevelManager.Instance.OnLevelInitialized -= SaveData;
    }

    public void SaveData(LevelConfig config, GameObject mapObject)
    {
        // Save level number
        int LevelNumber = GameManager.Instance.CurrentLevel + 1;

        // Save enemy data
        List<string> EnemyIds = new();
        for (int i = 0; i < config.Enemies.Count; i++)
        {
            EnemyIds.Add(config.Enemies[i].EnemyConf.Id);
        }

        // Save item source data
        ItemSourceDataList ItemSourceList = new ItemSourceDataList() { ItemSources = new() };
        for (int i = 0; i < config.ItemSources.Count; i++)
        {
            ItemSourceList.ItemSources.Add(new ItemSourceData()
            {
                Id = config.ItemSources[i].Prefab.Config.Id,
                MaxCapacity = config.ItemSources[i].MaxCapacity
            }
            );
        }

        // Save recipe data
        List<string> RecipeIds = new();
        for (int i = 0; i < config.IngotRecipes.Count; i++)
        {
            RecipeIds.Add(config.IngotRecipes[i].Id);
        }
        for (int i = 0; i < config.FinalRecipes.Count; i++)
        {
            RecipeIds.Add(config.FinalRecipes[i].Id);
        }

        // Save level time data
        float LevelTime = config.LevelTime;

        // Save score threshold data
        List<int> ScoreThresholds = new();
        for (int i = 0; i < config.ScoreThresholds.Length; i++)
        {
            ScoreThresholds.Add(config.ScoreThresholds[i]);
        }

        // Save player spawn data
        PlayerSpawner playerSpawner = mapObject.GetComponentInChildren<PlayerSpawner>();
        Location PlayerSpawnLocation = null;
        if (playerSpawner)
        {
            PlayerSpawnLocation = new Location()
            {
                LocationX = playerSpawner.transform.position.x,
                LocationY = playerSpawner.transform.position.y
            };
        }
        else
        {
            PlayerSpawnLocation = new Location()
            {
                LocationX = 0,
                LocationY = 0
            };
        }

        // Save station data 
        List<StationData> Stations = new();
        StationSpawner[] stationSpawners = mapObject.GetComponentsInChildren<StationSpawner>();
        for (int i = 0; i < stationSpawners.Length; i++)
        {
            Stations.Add(new StationData()
            {
                Type = stationSpawners[i].Prefab.Config.Type,
                StationLocation = new Location()
                {
                    LocationX = stationSpawners[i].transform.position.x,
                    LocationY = stationSpawners[i].transform.position.y
                }
            }
            );
        }

        // Save enemy spawner data 
        List<EnemySpawnerData> EnemySpawners = new();
        EnemySpawner[] enemySpawners = mapObject.GetComponentsInChildren<EnemySpawner>();
        for (int i = 0; i < enemySpawners.Length; i++)
        {
            EnemySpawners.Add(new EnemySpawnerData()
            {
                MinSpawnInterval = enemySpawners[i].Config.MinSpawnInterval,
                MaxSpawnInterval = enemySpawners[i].Config.MaxSpawnInterval,
                SpawnerLocation = new Location()
                {
                    LocationX = enemySpawners[i].transform.position.x,
                    LocationY = enemySpawners[i].transform.position.y
                }
            }
            );
        }

        // Save patrol center data
        List<Location> PatrolCenters = new();
        GameObject[] patrolCenters = GameObject.FindGameObjectsWithTag("PatrolCenter");
        for (int i = 0; i < patrolCenters.Length; i++)
        {
            PatrolCenters.Add(new Location()
            {
                LocationX = patrolCenters[i].transform.position.x,
                LocationY = patrolCenters[i].transform.position.y
            }
            );
        }

        // Save submission point data
        GameObject submissionPoint = GameObject.FindGameObjectWithTag("SubmissionPoint");
        Location SubmissionPoint = new Location()
        {
            LocationX = submissionPoint.transform.position.x,
            LocationY = submissionPoint.transform.position.y
        };

        // Save collider data
        ColliderSaver colliderSaver = mapObject.GetComponentInChildren<ColliderSaver>();
        colliderSaver.SaveColliders();
        ColliderSaver.ColliderDataList MapColliders = colliderSaver.nonTriggerList;
        ColliderSaver.ColliderDataList StationProtections = colliderSaver.triggerList;

        // Save whole level data
        _levelData = new LevelData()
        {
            LevelNumber = LevelNumber,
            EnemyIds = EnemyIds,
            ItemSourceList = ItemSourceList,
            RecipeIds = RecipeIds,
            LevelTime = LevelTime,
            ScoreThresholds = ScoreThresholds,
            PlayerSpawnLocation = PlayerSpawnLocation,
            Stations = Stations,
            EnemySpawners = EnemySpawners,
            PatrolCenters = PatrolCenters,
            SubmissionPoint = SubmissionPoint,
            MapColliders = MapColliders,
            StationProtections = StationProtections
        };

        string basePath = Application.persistentDataPath;
        string folderPath = Path.Combine(basePath, "Levels"); 
        string filePath = Path.Combine(folderPath, $"level{LevelNumber}.json");
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }
        string json = JsonUtility.ToJson(_levelData, prettyPrint: true);
        File.WriteAllText(filePath, json);
    } 

}