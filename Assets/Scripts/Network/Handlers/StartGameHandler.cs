using UnityEngine;
using UnityEngine.InputSystem;

public class StartGameHandler : MonoBehaviour
{
    [Header("Prefab")]
    [SerializeField] private GameObject _playerPrefab;
    [SerializeField] private PlayerConfig _playerConfig;
    [SerializeField] private InputActionAsset _inputActionAsset;
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

        if (!playerObj.TryGetComponent<TstController>(out var localPlayerController))
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

}
