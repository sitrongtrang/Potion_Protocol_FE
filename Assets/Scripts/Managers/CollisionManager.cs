using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class CollisionManager : MonoBehaviour
{
    private ColliderSaver.ColliderDataList mapColliderList = new();
    private ColliderSaver.ColliderDataList stationProtectionList = new();

    public static CollisionManager Instance { get; private set; }

    [Serializable]
    private class MapColliderData
    {
        public ColliderSaver.ColliderDataList MapColliders;
    }

    [Serializable]
    private class StationProtectionData
    {
        public ColliderSaver.ColliderDataList StationProtections;
    }

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

    private void OnEnable()
    {
        if (SceneManager.GetActiveScene().name == "OnlineGameScene")
        {
            NetworkEvents.OnMessageReceived += HandleNetworkMessage;
        }
    }

    private void OnDisable()
    {
        if (SceneManager.GetActiveScene().name == "OnlineGameScene")
        {
            NetworkEvents.OnMessageReceived -= HandleNetworkMessage;
        }
    }

    public void LoadColliders(int level)
    {
        string basePath = Application.persistentDataPath;
        string folderPath = Path.Combine(basePath, "Levels");
        string filePath = Path.Combine(folderPath, $"level{level}.json");

        if (File.Exists(filePath))
        {
            // Load map colliders
            string json = File.ReadAllText(filePath);
            mapColliderList = JsonUtility.FromJson<MapColliderData>(json).MapColliders;
            foreach (var colData in mapColliderList.colliders)
            {
                AABBCollider collider = new AABBCollider(
                    new Vector2(colData.centerX - colData.width / 2, colData.centerY - colData.height / 2),
                    new Vector2(colData.width, colData.height)
                )
                { Layer = (int)EntityLayer.Obstacle };
                CollisionSystem.InsertStaticCollider(collider);
            }

            // Load station protections
            stationProtectionList = JsonUtility.FromJson<StationProtectionData>(json).StationProtections;
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
            Debug.LogWarning($"Map collider file not found at {filePath}");
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

    private void HandleNetworkMessage(ServerMessage message)
    {
        switch (message.MessageType)
        {
            case NetworkMessageTypes.Server.Pregame.StartGame:
                LoadColliders(((ServerStartGame)message).Level);
                break;
            default:
                break;
        }
    }
}