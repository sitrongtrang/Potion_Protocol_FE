using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }
    [SerializeField] LevelConfig _config;
    private static Pathfinding _pathfinding;
    private int _score = 0;
    public int Score
    {
        get => _score;
        set
        {
            int oldScore = _score;
            _score = value;
            if (value != oldScore) OnScoreChanged?.Invoke();
            if (value >= _config.ScoreThresholds[_stars]) Stars++;
        }
    }
    private int _stars = 0;
    public int Stars
    {
        get => _stars;
        set
        {
            int oldStars = _stars;
            _stars = value;
            if (value != oldStars) OnStarGained?.Invoke();
        }
    }

    public event Action OnScoreChanged;
    public event Action OnStarGained;
    public bool IsPaused { get; private set; }
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

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    private void LoadLevel(LevelConfig config)
    {
        GameObject map = MapLoader.Instance.RenderMap(config.MapPrefab, Vector2.zero);

        (int width, int height, float cellSize, Vector2 origin) = GetMapParameters(map);

        Pathfinding.Instance.InitializeGrid(width, height, cellSize, origin);
        GridBuilderFactory.Instance.BuildGrid(
            "Pathfinding Grid",
            width,
            height,
            cellSize,
            origin,
            new string[]{"Obstacle"},
            LayerMask.GetMask("Obstacle"),
            (x,y,isoverlap) =>
            {
                PathNode pathNode = Pathfinding.Instance.GetNode(x, y);
                pathNode.IsWalkable = !isoverlap;
            },
            map.transform
        );

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
        GameObject[] patrolCenters = GameObject.FindGameObjectsWithTag("PatrolCenter");
        List<Transform> positionsToSpawn = new();
        for (int i = 0; i < patrolCenters.Length; i++)
        {
            positionsToSpawn.Add(patrolCenters[i].transform);
        }

        for (int i = 0; i < enemySpawners.Length; i++)
        {
            enemySpawners[i].Initialize(_config.Enemies, positionsToSpawn);
        }
    }

    private IEnumerator EndLevel()
    {
        yield return new WaitForSeconds(_config.LevelTime);
        TogglePause();
    }

    public void TogglePause()
    {
        IsPaused = !IsPaused;
        Time.timeScale = IsPaused ? 0f : 1f;
    }

    private (int, int, float, Vector2) GetMapParameters(GameObject mapGameobject)
    {
        Grid grid = mapGameobject.GetComponent<Grid>();
        Vector2 tileSize = grid.cellSize;

        float cellSize = Mathf.Min(tileSize.x, tileSize.y) * 0.5f;

        Tilemap[] tilemaps = mapGameobject.GetComponentsInChildren<Tilemap>();

        Vector3Int globalMinCell = new(int.MaxValue, int.MaxValue, 0);
        int maxXLength = 0, maxYLength = 0;
        foreach (Tilemap tilemap in tilemaps)
        {
            BoundsInt bounds = tilemap.cellBounds;
            Vector3Int min = bounds.min;

            maxXLength = Mathf.Max(maxXLength, bounds.size.x);
            maxYLength = Mathf.Max(maxYLength, bounds.size.y);

            if (min.x < globalMinCell.x) globalMinCell.x = min.x;
            if (min.y < globalMinCell.y) globalMinCell.y = min.y;
        }
        maxXLength = Mathf.CeilToInt((float)maxXLength / 0.5f);
        maxYLength = Mathf.CeilToInt((float)maxYLength / 0.5f);

        Vector2 bottomLeftWorldPos = grid.GetCellCenterWorld(globalMinCell);

        return (maxXLength, maxYLength, cellSize, bottomLeftWorldPos);
    }
}