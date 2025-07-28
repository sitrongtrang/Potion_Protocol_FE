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
                float colWidth = colData.vertices[2].x - colData.vertices[0].x;
                float colHeight = colData.vertices[2].y - colData.vertices[0].y;
                AABBCollider collider = new AABBCollider(
                    new Vector2(colData.centerX - colWidth / 2, colData.centerY - colHeight / 2),
                    new Vector2(colWidth, colHeight)
                )
                { Layer = (int)EntityLayer.Obstacle };
                CollisionSystem.InsertCollider(collider);
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
            var verts = c.vertices;
            int n = verts.Count;
            if (n < 2) continue;
            for (int i = 0; i < n; i++)
            {
                var a = verts[i];
                var b = verts[(i + 1) % n];
                Gizmos.DrawLine(new Vector3(a.x, a.y, 0f), new Vector3(b.x, b.y, 0f));
            }
        }
    }

}