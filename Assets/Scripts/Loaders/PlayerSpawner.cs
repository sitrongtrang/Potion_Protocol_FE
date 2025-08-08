using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField] private PlayerController _playerPrefab;
    [SerializeField] private PlayerConfig _playerConfig;
    [SerializeField] private InputActionAsset _inputActions;

    void Start()
    {
        // (optional) Load override từ file nếu cần
        string path = Application.persistentDataPath + "/rebinds.json";
        if (System.IO.File.Exists(path))
        {
            string json = System.IO.File.ReadAllText(path);
            _inputActions.LoadBindingOverridesFromJson(json);
            Debug.Log("🔁 Loaded rebinds in PlayerSpawner");
        }

        SpawnPlayer(transform.position, _inputActions);
    }

    void SpawnPlayer(Vector2 position, InputActionAsset loadedInputAsset = null)
    {
        PlayerController player = Instantiate(_playerPrefab, position, Quaternion.identity);
        player.Initialize(_playerConfig, loadedInputAsset);
    }
}