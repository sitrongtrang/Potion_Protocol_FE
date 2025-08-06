using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameStateHandler : MonoBehaviour
{
    [Header("Prefabs")]
    private ScriptableObjectMapping _prafabsMap;

    private GameStateNetworkInterpolator _interpolator = new(NetworkConstants.NET_INTERPOLATION_BUFFER_SIZE);

    private Dictionary<string, TrackedObject> _enemyMap = new();
    private Dictionary<string, TrackedObject> _itemSourceMap = new();
    private Dictionary<string, TrackedObject> _itemMap = new();
    private Dictionary<string, TrackedObject> _stationMap = new();

    void Start()
    {
        _prafabsMap = (ScriptableObjectMapping)ScriptableObject.CreateInstance(typeof(ScriptableObjectMapping));
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
                HandleSyncing(gameState.EnemyIds, _enemyMap, _prafabsMap.EnemyPrefab);
                HandleSyncing(gameState.ItemSourceIds, _itemSourceMap, _prafabsMap.ItemSourcePrefab);
                HandleSyncing(gameState.ItemIds, _itemMap, _prafabsMap.ItemPrefab);
                HandleSyncing(gameState.StationIds, _stationMap, _prafabsMap.StationPrefab);
            }
        );
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
                obj.Initialize(id, _prafabsMap.GetSO(entityInfo.TypeId));
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

        _prafabsMap.InitializeMapping(scriptableObjects.ToArray());
    }
}