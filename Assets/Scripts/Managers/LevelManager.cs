using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }
    private LevelConfig _config;
    [SerializeField] LoadingScreenUI _loadingScreen;
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
                GameManager.Instance.Score = value;
                OnScoreChanged?.Invoke(value);
            }
            if (Stars >= _config.ScoreThresholds.Length) return;
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
            _stars = Mathf.Min(value, _config.ScoreThresholds.Length);
            if (value != oldStars)
            {
                OnStarGained?.Invoke();
            }
        }
    }

    private float _timeLeft;
    public bool IsPaused { get; private set; }
    public event Action<int> OnScoreChanged;
    public event Action<float> OnTimeChanged;
    public event Action<bool> OnPauseToggled;
    public event Action OnStarGained;
    [SerializeField] private ItemSourceSpawner _itemSourceSpawner;

    private List<RecipeConfig> _requiredRecipes = new List<RecipeConfig>();
    public event Action<RecipeConfig> OnRequiredRecipeAdded;
    public event Action<int> OnRequiredRecipeRemoved;
    [SerializeField] private RequiredRecipeListUI _requiredRecipeListUI;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // _config = Resources.Load<LevelConfig>($"ScriptableObjects/Levels/Level{GameManager.Instance.CurrentLevel + 1}");
    }

    void Start()
    {
        StartCoroutine(LoadLevelAsset());   
    }

    private IEnumerator LoadLevelAsset()
    {
        string levelPath = $"ScriptableObjects/Levels/Level{GameManager.Instance.CurrentLevel + 1}";
        ResourceRequest request = Resources.LoadAsync<LevelConfig>(levelPath);
        _loadingScreen.gameObject.SetActive(true);

        yield return StartCoroutine(_loadingScreen.RenderLoadingScene(request));
    
        _config = request.asset as LevelConfig;
        Initialize();
        _loadingScreen.gameObject.SetActive(false);
    }

    void Initialize()
    {
        _requiredRecipeListUI.Initialize(_requiredRecipes);
        _requiredRecipes.Clear();
        LoadLevel(_config);
        StartCoroutine(LevelTimer());
        StartCoroutine(AddNewRequiredRecipe());
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
            new string[] { "Obstacle", "ItemSource" },
            LayerMask.GetMask("Obstacle"),
            (x, y, isoverlap) =>
            {
                PathNode pathNode = Pathfinding.Instance.GetNode(x, y);
                pathNode.IsWalkable = !isoverlap;
            },
            map.transform
        );

        GameObject spawnItemSourceBounds = GameObject.Find("Spawn ItemSource Bounds");

        BoxCollider2D collider2D = spawnItemSourceBounds.GetComponent<BoxCollider2D>();
        float widthInF = collider2D.bounds.size.x;
        float heightInF = collider2D.bounds.size.y;

        GridBuilderFactory.Instance.BuildGrid(
            GridBuilderFactory.BuilderNames[1],
            Mathf.CeilToInt(widthInF / cellSize),
            Mathf.CeilToInt(heightInF / cellSize),
            cellSize,
            origin,
            new string[] { "Obstacle", "ItemSource", "Enemy", "Player" },
            LayerMask.GetMask("Obstacle"),
            (x, y, isoverlap) =>
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

        // Initialize item source spawner
        _itemSourceSpawner.Initialize(_config.ItemSources);
    }

    private IEnumerator LevelTimer()
    {
        while (_timeLeft > 0)
        {
            yield return new WaitForSeconds(1);
            _timeLeft -= 1;
            OnTimeChanged?.Invoke(_timeLeft);
        }
        if (_timeLeft <= 0)
        {
            GameManager.Instance.Star = _stars;
            SceneManager.LoadScene("LevelResultScene");
        }
    }

    public void TogglePause()
    {
        IsPaused = !IsPaused;
        Time.timeScale = IsPaused ? 0f : 1f;
        OnPauseToggled?.Invoke(IsPaused);
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

    private RecipeConfig GetNewRequiredRecipe()
    {
        int idx = _config.FinalRecipes.Count > 0 ? UnityEngine.Random.Range(0, _config.FinalRecipes.Count) : -1;
        if (idx == -1) return null;
        return _config.FinalRecipes[idx];
    }

    private IEnumerator AddNewRequiredRecipe()
    {
        while (true)
        {
            RecipeConfig newRecipe = GetNewRequiredRecipe();
            if (newRecipe != null)
            {
                _requiredRecipes.Add(newRecipe);
                OnRequiredRecipeAdded?.Invoke(newRecipe);
                yield return new WaitForSeconds(_config.RecipeAddInterval);
                yield return new WaitUntil(() => _requiredRecipes.Count < GameConstants.MaxRequiredRecipes);
            }
        }
    }

    public bool OnProductSubmitted(FinalProductConfig product)
    {
        for (int i = 0; i < _requiredRecipes.Count; i++)
        {
            if (_requiredRecipes[i].Product == product)
            {
                _requiredRecipes.RemoveAt(i);
                OnRequiredRecipeRemoved?.Invoke(i);
                Score += product.Score;
                return true;
            }
        }
        Debug.Log($"Submitted product {product.Name} is not in the required recipes list.");
        return false;
    }
}