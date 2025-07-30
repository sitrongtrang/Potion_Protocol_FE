using System;
using System.Collections.Generic;
using UnityEngine;

public class GameStateHandler : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject _playerPrefab;
    [SerializeField] private GameObject _enemyPrefab;
    [SerializeField] private GameObject _itemSourcePrefab;
    [SerializeField] private GameObject _itemPrefab;
    [SerializeField] private GameObject _stationPrefab;

    private GameStateNetworkInterpolator _interpolator = new(NetworkConstants.NET_INTERPOLATION_BUFFER_SIZE);
    
    private Dictionary<string, GameObject> _enemyMap = new();
    private Dictionary<string, GameObject> _itemSourceMap = new();
    private Dictionary<string, GameObject> _itemMap = new();
    private Dictionary<string, GameObject> _playerMap = new();
    private Dictionary<string, GameObject> _stationMap = new();
    void FixedUpdate()
    {
        
    }

    void OnEnable()
    {
        NetworkEvents.OnMessageReceived += HandleNetworkMessage;
    }
    void OnDisable()
    {
        NetworkEvents.OnMessageReceived -= HandleNetworkMessage;
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

    }
}