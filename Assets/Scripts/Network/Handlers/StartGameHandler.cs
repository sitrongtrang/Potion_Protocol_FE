using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class StartGameHandler : MonoBehaviour
{
    [Header("Prefab")]
    [SerializeField] private GameObject _playerPrefab;
    [SerializeField] private PlayerConfig _playerConfig;
    [SerializeField] private InputActionAsset _inputActionAsset;

    public event Action<LevelConfig, GameObject> OnLevelInitialized;

    private void OnEnable()
    {
        NetworkEvents.OnMessageReceived += HandleNetworkMessage;
    }

    private void OnDisable()
    {
        NetworkEvents.OnMessageReceived -= HandleNetworkMessage;
    }

    private void TrySpawnPlayer(string playerId, Vector2 position, bool isLocal)
    {
        if (_playerPrefab == null) return;

        GameObject playerObj = Instantiate(_playerPrefab, position, Quaternion.identity);

        if (!playerObj.TryGetComponent<PlayerNetworkController>(out var localPlayerController))
        {
            Debug.LogError("Wrong player object");
            Destroy(playerObj);
            return;
        }

        localPlayerController.Initialize(_playerConfig, _inputActionAsset, playerId, isLocal);

        // Additional setup
        if (isLocal)
        {
            // Setup camera follow, input controls, etc.
            Debug.Log($"Spawned local player: {playerId}");
        }
        else
        {
            Debug.Log($"Spawned remote player: {playerId}");
        }
    }

    private void HandleNetworkMessage(ServerMessage message)
    {
        switch (message.MessageType)
        {
            case NetworkMessageTypes.Server.Pregame.StartGame:
                HandlePlayerSpawn((ServerStartGame)message);
                InitializeLevel((ServerStartGame)message);
                break;
            case NetworkMessageTypes.Server.Room.OnlyLeader:
                Debug.Log("Only Leader");
                break;
            case NetworkMessageTypes.Server.Room.PlayerNotReady:
                Debug.Log("Only Leader");
                break;
            case NetworkMessageTypes.Server.Pregame.MatchMaking:
                Debug.Log("Match Making");
                break;
            default:
                break;
        }
    }

    private void HandlePlayerSpawn(ServerStartGame message)
    {
        string thisPlayerId = message.PlayerId;
        for (int i = 0; i < message.PlayerIds.Length; i++)
        {
            TrySpawnPlayer(
                message.PlayerIds[i],
                Vector2.zero,
                thisPlayerId == message.PlayerIds[i]
            );
        }
    }

    private void InitializeLevel(ServerStartGame message)
    {
        int level = message.Level;

        string levelPath = $"ScriptableObjects/Levels/Level{level}";
        LevelConfig config = Resources.Load<LevelConfig>(levelPath);

        GameObject map = Instantiate(config.MapPrefab, Vector2.zero, Quaternion.identity);

        OnLevelInitialized?.Invoke(config, map);
    }
}
