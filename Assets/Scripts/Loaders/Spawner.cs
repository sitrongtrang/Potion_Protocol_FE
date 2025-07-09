using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] private BaseSpawnConfig _config;

    void Awake()
    {
        _config.Spawn(transform.position); 
    }
}