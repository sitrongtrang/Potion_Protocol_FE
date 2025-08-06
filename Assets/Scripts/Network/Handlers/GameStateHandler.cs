using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameStateHandler : MonoBehaviour
{
    [Header("Prefabs")]
    private ScriptableObjectMapping _prefabsMap;

    private GameStateNetworkInterpolator _interpolator = new(NetworkConstants.NET_INTERPOLATION_BUFFER_SIZE);

    private Dictionary<string, TrackedObject> _enemyMap = new();
    private Dictionary<string, TrackedObject> _itemSourceMap = new();
    private Dictionary<string, TrackedObject> _itemMap = new();
    private Dictionary<string, TrackedObject> _stationMap = new();
    private List<RecipeConfig> _requiredRecipes = new();

    public ScriptableObjectMapping PrefabsMap => _prefabsMap;

    void Start()
    {
        _prefabsMap = (ScriptableObjectMapping)ScriptableObject.CreateInstance(typeof(ScriptableObjectMapping));
    }

    void OnEnable()
    {
        NetworkEvents.OnMessageReceived += HandleNetworkMessage;
        LevelManager.Instance.OnLevelInitialized += PrepareConfigs;
    }

    void OnDisable()
    {
        NetworkEvents.OnMessageReceived -= HandleNetworkMessage;
        LevelManager.Instance.OnLevelInitialized -= PrepareConfigs;
    }

    void FixedUpdate()
    {
        _interpolator.IncrementAndInterpolate(
            (gameState) =>
            {
                HandleSyncing(gameState.EnemyIds, _enemyMap, _prefabsMap.EnemyPrefab);
                HandleSyncing(gameState.ItemSourceIds, _itemSourceMap, _prefabsMap.ItemSourcePrefab);
                HandleSyncing(gameState.ItemIds, _itemMap, _prefabsMap.ItemPrefab);
                HandleSyncing(gameState.StationIds, _stationMap, _prefabsMap.StationPrefab);
                SyncRecipes(gameState.RequiredRecipeIds);
            }
        );
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
                keysToRemove.Add(kvp.Key);
        }
        foreach (var key in keysToRemove)
        {
            Destroy(current[key]);
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
        StationController[] stationControllers = map.GetComponentsInChildren<StationController>();
        for (int i = 0; i < stationControllers.Length; i++)
        {
            scriptableObjects.Add(stationControllers[i].Config);
        }

        _prefabsMap.InitializeMapping(scriptableObjects.ToArray());
    }
}