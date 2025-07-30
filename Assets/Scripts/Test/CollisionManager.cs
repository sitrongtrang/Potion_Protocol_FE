using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;

public class CollisionManager : MonoBehaviour
{
    public static CollisionManager Instance { get; private set; }
    private ColliderSaver.ColliderDataList mapColliderList = new();
    private ColliderSaver.ColliderDataList stationProtectionList = new();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        CollisionSystem.Initialize(new Rect(-960, -540, 1920, 1080));
        mapColliderList = new ColliderSaver.ColliderDataList { colliders = new List<ColliderSaver.RectangleData>() };
        stationProtectionList = new ColliderSaver.ColliderDataList { colliders = new List<ColliderSaver.RectangleData>() };
    }

    void Start()
    {
        LoadColliders();
    }

    public void LoadColliders()
    {
        string basePath = Application.persistentDataPath;

        // Load map colliders
        string path = Path.Combine(basePath, "colliders_map.json");
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            mapColliderList = JsonUtility.FromJson<ColliderSaver.ColliderDataList>(json);
            foreach (var colData in mapColliderList.colliders)
            {
                AABBCollider collider = new AABBCollider(
                    new Vector2(colData.centerX - colData.width / 2, colData.centerY - colData.height / 2),
                    new Vector2(colData.width, colData.height)
                )
                { Layer = (int)EntityLayer.Obstacle };
                CollisionSystem.InsertStaticCollider(collider);
            }

        }
        else
        {
            Debug.LogWarning($"Map collider file not found at {path}");
        }

        // Load station protections
        path = Path.Combine(basePath, "colliders_station.json");
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            stationProtectionList = JsonUtility.FromJson<ColliderSaver.ColliderDataList>(json);
            foreach (var colData in stationProtectionList.colliders)
            {
                AABBCollider collider = new AABBCollider(
                    new Vector2(colData.centerX - colData.width / 2, colData.centerY - colData.height / 2),
                    new Vector2(colData.width, colData.height)
                )
                { Layer = (int)EntityLayer.StationProtection };
                CollisionSystem.InsertStaticCollider(collider);
            }

        }
        else
        {
            Debug.LogWarning($"Map collider file not found at {path}");
        } 

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        foreach (var c in mapColliderList.colliders)
        {
            Gizmos.DrawWireCube(new Vector2(c.centerX, c.centerY), new Vector2(c.width, c.height));
        }

        Gizmos.color = Color.yellow;
        foreach (var c in stationProtectionList.colliders)
        {
            Gizmos.DrawWireCube(new Vector2(c.centerX, c.centerY), new Vector2(c.width, c.height));
        }
        
        Gizmos.color = Color.green;
        CollisionSystem.OnDrawGizmos();
    }

}