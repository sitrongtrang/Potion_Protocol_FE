using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField] private PlayerConfig _config;

    void Start()
    {
        _config.Spawn(transform.position); 
    }
}