using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class MapLoader : MonoBehaviour
{
    public static MapLoader Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public GameObject RenderMap(GameObject mapPrefab, Vector3 position)
    {
        GameObject map = Instantiate(mapPrefab, position, Quaternion.identity);
        return map;
    }
}
