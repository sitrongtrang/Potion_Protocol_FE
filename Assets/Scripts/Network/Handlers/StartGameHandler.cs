using UnityEngine;
using UnityEngine.InputSystem;

public class StartGameHandler : MonoBehaviour
{
    [Header("Prefab")]
    [SerializeField] private GameObject _playerPrefab;
    [SerializeField] private InputActionAsset _inputActionAsset;
    private string[] _playerIds;
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

        localPlayerController.Initialize(_inputActionAsset, playerId, isLocal);

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
            case NetworkMessageTypes.Server.Pregame.GetPlayerId:
                HandleGetPlayerId((GetPlayerId)message);
                break;
        }
    }

    private void HandlePlayerSpawn(ServerStartGame message)
    {
        // _playerIds = new string[message.PlayerDTOs.Length];
        // for (int i = 0; i < _playerIds.Length; i++)
        // {
        //     _playerIds[i] = message.PlayerDTOs[i].PlayerId;
        // }
    }

    private void HandleGetPlayerId(GetPlayerId message)
    {
        string localPlayerId = message.PlayerId;
        for (int i = 0; i < message.AllPlayerIds.Length; i++)
        {
            TrySpawnPlayer(
                message.AllPlayerIds[i],
                Vector2.zero,
                message.AllPlayerIds[i] == localPlayerId
            );
        }
    }

}
