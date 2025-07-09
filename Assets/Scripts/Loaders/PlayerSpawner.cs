using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField] private PlayerConfig _config;

    void Awake()
    {
        PlayerController player = Instantiate(_config.Prefab, transform.position, Quaternion.identity);
        player.Initialize(_config);
    }
}