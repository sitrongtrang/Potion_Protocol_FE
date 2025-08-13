using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStateHandler : MonoBehaviour
{
    [SerializeField] private StartGameHandler _startGameHandler;
    [Header("Prefabs")]
    [SerializeField] private ScriptableObjectMapping _prefabMapTemplate;
    private ScriptableObjectMapping _prefabsMap;

    private GameStateNetworkInterpolator _interpolator = new(NetworkConstants.NET_INTERPOLATION_BUFFER_SIZE);

    private Dictionary<string, TrackedObject> _enemyMap = new();
    private Dictionary<string, TrackedObject> _itemSourceMap = new();
    private Dictionary<string, TrackedObject> _itemMap = new();
    // private Dictionary<string, TrackedObject> _stationMap = new();
    private List<RecipeConfig> _requiredRecipes = new();
    private int[] _scoreThresholds;
    private int _totalScore;
    private int _stars;
    private float _timeLeft;
    private float _maxTime;

    public event Action<string[], int[]> OnInventorySynced;
    public event Action<int> OnScoreChanged;
    public event Action<List<RecipeConfig>> OnRecipesSynced;
    public event Action<float> OnTimeChanged;
    public ScriptableObjectMapping PrefabsMap => _prefabsMap;
    
    void Awake()
    {
        // _prefabsMap = (ScriptableObjectMapping)ScriptableObject.CreateInstance(typeof(ScriptableObjectMapping));
        _prefabsMap = Instantiate(_prefabMapTemplate);
    }

    void OnEnable()
    {
        NetworkEvents.OnMessageReceived += HandleNetworkMessage;
        _startGameHandler.OnLevelInitialized += PrepareConfigs;
    }

    void OnDisable()
    {
        NetworkEvents.OnMessageReceived -= HandleNetworkMessage;
        _startGameHandler.OnLevelInitialized -= PrepareConfigs;
    }

    void FixedUpdate()
    {
        _interpolator.IncrementAndInterpolate(
            (gameState) =>
            {
                HandleSyncing(gameState.EnemyIds, _enemyMap, _prefabsMap.EnemyPrefab);
                HandleSyncing(gameState.ItemSourceIds, _itemSourceMap, _prefabsMap.ItemSourcePrefab);
                HandleSyncing(gameState.ItemIds, _itemMap, _prefabsMap.ItemPrefab);
                // HandleSyncing(gameState.StationIds, _stationMap, _prefabsMap.StationPrefab);

                // Syncing UI
                SyncRecipes(gameState.RequiredRecipeIds);
                SyncScore(gameState.PlayerScores);
                SyncInventory(gameState.PlayerInventories, gameState.PlayerInventoriesIndices);
                SyncTime(gameState.TimeLeft);
            }
        );

        // _timeLeft -= Time.fixedDeltaTime;
    }

    private void SyncRecipes(List<string> data)
    {
        for (int i = 0; i < data.Count; i++)
        {
            ScriptableObject scriptableObject = PrefabsMap.GetSO(data[i]);
            if (scriptableObject is RecipeConfig recipeConfig)
            {
                if (i >= _requiredRecipes.Count)
                {
                    _requiredRecipes.Add(recipeConfig);
                }
                else
                {
                    _requiredRecipes[i] = recipeConfig;
                }
            }
        }

        for (int i = _requiredRecipes.Count - 1; i >= data.Count; i--)
        {
            _requiredRecipes.RemoveAt(i);
        }
        OnRecipesSynced?.Invoke(_requiredRecipes);
    }

    private void SyncScore(Dictionary<string, int> scores)
    {
        _totalScore = 0;
        foreach (var item in scores)
        {
            _totalScore += item.Value;
            string localId = _startGameHandler.LocalPlayer?.Identity.PlayerId;
            if (localId == null) continue;
            if (scores.ContainsKey(localId))
            {
                OnScoreChanged?.Invoke(scores[localId]);
                break;
            }
        }

        _stars = 3;
        for (int i = _scoreThresholds.Length - 1; i >= 0; i--)
        {
            if (_totalScore >= _scoreThresholds[i]) break;
                _stars--;
        }
    }

    private void SyncInventory(Dictionary<string, string[]> inventories, Dictionary<string, int[]> inventoryIndecies)
    {
        foreach (var item in inventories)
        {
            string localId = _startGameHandler.LocalPlayer?.Identity.PlayerId;
            if (localId == null) continue;
            if (inventories.ContainsKey(localId) && inventoryIndecies.ContainsKey(localId))
            {
                OnInventorySynced?.Invoke(inventories[localId], inventoryIndecies[localId]);
                break;
            }
        }
    }

    private void SyncTime(float timeLeft)
    {
        _timeLeft = timeLeft;
        OnTimeChanged?.Invoke(_timeLeft);
        if (_timeLeft >= _maxTime)
        {
            StartCoroutine(LoadLevelResult(_totalScore, _stars));
        }
    }

    private void HandleSyncing(
        Dictionary<string, GameStateInterpolateData.EntityInfo> data,
        Dictionary<string, TrackedObject> current,
        NetworkBehaviour prefab
    )
    {
        List<string> keysToRemove = new();
        foreach (var kvp in current)
        {
            if (!data.ContainsKey(kvp.Key))
            {
                keysToRemove.Add(kvp.Key);
            }
        }
        foreach (var key in keysToRemove)
        {
            Destroy(current[key].gameObject);
        }

        foreach (var kvp in data)
        {
            string id = kvp.Key;
            GameStateInterpolateData.EntityInfo entityInfo = kvp.Value;

            if (!current.ContainsKey(id))
            {
                NetworkBehaviour obj = Instantiate(prefab, entityInfo.Position, Quaternion.identity);
                obj.Initialize(id, _prefabsMap.GetSO(entityInfo.TypeId));
                TrackedObject trackedObject = obj.AddComponent<TrackedObject>();
                current.Add(id, trackedObject);

                trackedObject.Id = id;

                trackedObject.OnDestroyed += (id) =>
                {
                    if (current.ContainsKey(id))
                    {
                        current.Remove(id);
                    }
                };
            }
        }
    }

    private void HandleNetworkMessage(ServerMessage message)
    {
        switch (message.MessageType)
        {
            case NetworkMessageTypes.Server.GameState.StateUpdate:
                HandleGameStates((GameStatesUpdate)message);
                break;

            default:
                break;
        }
    }
    private void HandleGameStates(GameStatesUpdate gameStates)
    {
        _interpolator.Store(gameStates.GameStates, null);
        // if (gameStates.GameStates[0] != null)
        // {
        //     for (int i = 0; i < gameStates.GameStates[0].ItemStates.Length; i++)
        //     {
        //         Debug.Log(gameStates.GameStates[0].ItemStates[i].ItemType);
        //     }
        // }
    }

    private void PrepareConfigs(LevelConfig levelConfig, GameObject map)
    {
        List<ScriptableObject> scriptableObjects = new();
        for (int i = 0; i < levelConfig.Enemies.Count; i++)
        {
            scriptableObjects.Add(levelConfig.Enemies[i]);
            scriptableObjects.Add(levelConfig.Enemies[i].Item);
        }
        for (int i = 0; i < levelConfig.ItemSources.Count; i++)
        {
            scriptableObjects.Add(levelConfig.ItemSources[i].Config);
            scriptableObjects.Add(levelConfig.ItemSources[i].Config.DroppedItem);
        }
        for (int i = 0; i < levelConfig.IngotRecipes.Count; i++)
        {
            scriptableObjects.Add(levelConfig.IngotRecipes[i]);
            scriptableObjects.Add(levelConfig.IngotRecipes[i].Product);
        }
        for (int i = 0; i < levelConfig.FinalRecipes.Count; i++)
        {
            scriptableObjects.Add(levelConfig.FinalRecipes[i]);
            scriptableObjects.Add(levelConfig.FinalRecipes[i].Product);
        }

        _prefabsMap.InitializeMapping(scriptableObjects.ToArray());
        _maxTime = levelConfig.LevelTime;
        _scoreThresholds = levelConfig.ScoreThresholds;

    }
    
    private IEnumerator LoadLevelResult(int score, int star)
    {
        LoadingScreenUI.Instance.OnSceneExit += () =>
        {
            LoadingScreenUI.Instance.SetData("Score", score);
            LoadingScreenUI.Instance.SetData("Stars", star);
        };
        AsyncOperation request = SceneManager.LoadSceneAsync("LevelResultScene");
        request.completed += async (op) =>
        {
            await LoadingScreenUI.Instance.RenderFinish();
        };
        LoadingScreenUI.Instance.gameObject.SetActive(true);
        List<AsyncOperation> opList = new List<AsyncOperation>();
        opList.Add(request);

        yield return StartCoroutine(LoadingScreenUI.Instance.RenderLoadingScene(opList));
    }
}