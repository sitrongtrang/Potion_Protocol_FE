using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;

public class CollisionManager : MonoBehaviour
{
    public static CollisionManager Instance { get; private set; }
    private ColliderSaver.ColliderDataList nonTriggerList = new();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        CollisionSystem.Initialize(new Rect(-960, -540, 1920, 1080));
        nonTriggerList = new ColliderSaver.ColliderDataList { colliders = new List<ColliderSaver.ColliderData>() };
    }

    void Start()
    {
        LoadColliders();
    }

    public void LoadColliders()
    {
        string basePath = Application.persistentDataPath;

        // Load non-trigger colliders
        string pathNon = Path.Combine(basePath, "colliders_nonTrigger.json");
        if (File.Exists(pathNon))
        {
            string json = File.ReadAllText(pathNon);
            nonTriggerList = JsonUtility.FromJson<ColliderSaver.ColliderDataList>(json);
            foreach (var colData in nonTriggerList.colliders)
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
            Debug.LogWarning($"Non-trigger collider file not found at {pathNon}");
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        foreach (var c in nonTriggerList.colliders)
        {
            float halfW = c.width / 2f;
            float halfH = c.height / 2f;

            Vector3 bottomLeft = new Vector3(c.centerX - halfW, c.centerY - halfH, 0);
            Vector3 bottomRight = new Vector3(c.centerX + halfW, c.centerY - halfH, 0);
            Vector3 topRight = new Vector3(c.centerX + halfW, c.centerY + halfH, 0);
            Vector3 topLeft = new Vector3(c.centerX - halfW, c.centerY + halfH, 0);

            Gizmos.DrawLine(bottomLeft, bottomRight);
            Gizmos.DrawLine(bottomRight, topRight);
            Gizmos.DrawLine(topRight, topLeft);
            Gizmos.DrawLine(topLeft, bottomLeft);
        }
        
        Gizmos.color = Color.green;
        CollisionSystem.OnDrawGizmos();
    }

}