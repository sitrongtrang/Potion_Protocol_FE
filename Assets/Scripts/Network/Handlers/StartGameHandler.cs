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
    private PlayerNetworkController _localPlayer;

    public event Action<LevelConfig, GameObject> OnLevelInitialized;
    public PlayerNetworkController LocalPlayer => _localPlayer;

    private void OnEnable()
    {
        LoadingScreenUI.Instance.OnSceneEnter += HandleOnSceneEnter;
    }

    private void OnDisable()
    {
        LoadingScreenUI.Instance.OnSceneEnter -= HandleOnSceneEnter;
    }

    private void TrySpawnPlayer(string playerId, Vector2 position, bool isLocal)
    {
        if (_playerPrefab == null) return;

        GameObject playerObj = Instantiate(_playerPrefab, position, Quaternion.identity);

        if (!playerObj.TryGetComponent<PlayerNetworkController>(out var playerController))
        {
            Debug.LogError("Wrong player object");
            Destroy(playerObj);
            return;
        }

        playerController.Initialize(_playerConfig, _inputActionAsset, playerId, isLocal);

        // Additional setup
        if (isLocal)
        {
            // Setup camera follow, input controls, etc.
            _localPlayer = playerController;
            Debug.Log($"Spawned local player: {playerId}");
        }
        else
        {
            Debug.Log($"Spawned remote player: {playerId}");
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
        // int level = 1;

        string levelPath = $"ScriptableObjects/Levels/Level{level}";
        LevelConfig config = Resources.Load<LevelConfig>(levelPath);

        GameObject map = Instantiate(config.MapPrefab, Vector2.zero, Quaternion.identity);

        OnLevelInitialized?.Invoke(config, map);
    }

    private void HandleOnSceneEnter()
    {
        ServerStartGame msg = LoadingScreenUI.Instance.GetData<ServerStartGame>("StartGameData");
        HandlePlayerSpawn(msg);
        InitializeLevel(msg);
    }
}
