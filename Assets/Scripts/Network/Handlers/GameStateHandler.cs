using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameStateHandler : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private ScriptableObjectMapping _prafabsMap;

    private GameStateNetworkInterpolator _interpolator = new(NetworkConstants.NET_INTERPOLATION_BUFFER_SIZE);
    
    private Dictionary<string, TrackedObject> _enemyMap = new();
    private Dictionary<string, TrackedObject> _itemSourceMap = new();
    private Dictionary<string, TrackedObject> _itemMap = new();
    private Dictionary<string, TrackedObject> _stationMap = new();
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

    void OnEnable()
    {
        NetworkEvents.OnMessageReceived += HandleNetworkMessage;
    }
    void OnDisable()
    {
        NetworkEvents.OnMessageReceived -= HandleNetworkMessage;
    }

    private void HandleSyncing(
        Dictionary<string, GameStateInterpolateData.EntityInfo> data,
        Dictionary<string, TrackedObject> current,
        NetworkBehaviour prefab
    ) {
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
}