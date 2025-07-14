using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }
    [SerializeField] LevelConfig _config;
    [SerializeField] private TextMeshProUGUI _scoreText;
    private int _score = 0;
    public int Score
    {
        get => _score;
        set
        {
            int oldScore = _score;
            _score = value;
            if (value != oldScore)
            {
                _scoreText.text = value.ToString();
                OnScoreChanged?.Invoke();
            } 
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

    private float _timeLeft;
    [SerializeField] private TextMeshProUGUI _timeText;
    [SerializeField] private OreSpawner _oreSpawner;
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
        StartCoroutine(LevelTimer());
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
        _timeLeft = config.LevelTime;

        GameObject map = MapLoader.Instance.RenderMap(config.MapPrefab, Vector2.zero);

        (int width, int height, float cellSize, Vector2 origin) = GetMapParameters(map);
        cellSize *= 0.5f;

        Pathfinding.Instance.InitializeGrid(width * 2, height * 2, cellSize, origin);
        GridBuilderFactory.Instance.BuildGrid(
            GridBuilderFactory.BuilderNames[0],
            width * 2,
            height * 2,
            cellSize,
            origin,
            new string[] { "Obstacle" , "Ore"},
            LayerMask.GetMask("Obstacle"),
            (x, y, isoverlap) =>
            {
                PathNode pathNode = Pathfinding.Instance.GetNode(x, y);
                pathNode.IsWalkable = !isoverlap;
            },
            map.transform
        );

        GameObject spawnOreBounds = GameObject.Find("Spawn Ore Bounds");

        BoxCollider2D collider2D = spawnOreBounds.GetComponent<BoxCollider2D>();
        float widthInF = collider2D.bounds.size.x;
        float heightInF = collider2D.bounds.size.y;

        GridBuilderFactory.Instance.BuildGrid(
            GridBuilderFactory.BuilderNames[1],
            Mathf.CeilToInt(widthInF / cellSize),
            Mathf.CeilToInt(heightInF / cellSize),
            cellSize,
            origin,
            new string[]{"Obstacle", "Ore", "Enemy", "Player"},
            LayerMask.GetMask("Obstacle"),
            (x,y,isoverlap) =>
            {

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

        // Initialize ore spawner
        _oreSpawner.Initialize(_config.Ores);
    }

    private IEnumerator LevelTimer()
    {
        while (_timeLeft > 0)
        {
            yield return new WaitForSeconds(1);
            _timeLeft -= 1;
            TimeSpan timeSpan = TimeSpan.FromSeconds(_timeLeft);
            _timeText.text = string.Format("{0:mm}:{0:ss}", timeSpan);       
        }
        if (_timeLeft <= 0) TogglePause();
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

        float cellSize = Mathf.Min(tileSize.x, tileSize.y);

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

        Vector2 bottomLeftWorldPos = grid.GetCellCenterWorld(globalMinCell);

        return (maxXLength, maxYLength, cellSize, bottomLeftWorldPos);
    }
}