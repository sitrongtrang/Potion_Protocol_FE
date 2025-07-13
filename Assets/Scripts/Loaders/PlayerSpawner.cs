using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField] private PlayerConfig _config;

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

        _config.Spawn(transform.position, _inputActions);
    }
}