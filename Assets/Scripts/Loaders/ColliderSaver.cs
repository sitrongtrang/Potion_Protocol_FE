using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[ExecuteInEditMode]
public class ColliderSaver : MonoBehaviour
{
    [Serializable]
    public class Point
    {
        public float x, y;
        public Point() { }
        public Point(Vector2 v) { x = v.x; y = v.y; }
    }

    [Serializable]
    public class ColliderData
    {
        public float centerX, centerY;
        public List<Point> vertices;
    }
    [Serializable]
    public class ColliderDataList
    {
        public List<ColliderData> colliders;
    }

    public GameObject mapGameObject;
    public List<Tilemap> tilemaps;

    private ColliderDataList nonTriggerList;
    private ColliderDataList triggerList;

    private void Start()
    {
        SaveColliders();
    }

    public void SaveColliders()
    {
        nonTriggerList = new ColliderDataList { colliders = new List<ColliderData>() };
        triggerList = new ColliderDataList { colliders = new List<ColliderData>() };
        var all = mapGameObject.GetComponentsInChildren<Collider2D>(true);
        foreach (var col in all)
        {
            if (col is TilemapCollider2D || col is CompositeCollider2D)
                continue;

            var targetList = col.isTrigger ? triggerList : nonTriggerList;
            var b = col.bounds;
            targetList.colliders.Add(new ColliderData
            {
                centerX = b.center.x,
                centerY = b.center.y,
                vertices = new List<Point> {
                    new Point(new Vector2(b.min.x, b.min.y)),
                    new Point(new Vector2(b.max.x, b.min.y)),
                    new Point(new Vector2(b.max.x, b.max.y)),
                    new Point(new Vector2(b.min.x, b.max.y)),
                }
            });
        }

        foreach (var tm in tilemaps)
        {
            var comp = tm.GetComponent<CompositeCollider2D>();
            if (comp == null) continue;

            var targetList = comp.isTrigger ? triggerList : nonTriggerList;
            for (int i = 0; i < comp.pathCount; i++)
            {
                int pointCount = comp.GetPathPointCount(i);
                var pts = new Vector2[pointCount];
                comp.GetPath(i, pts);

                var data = new ColliderData
                {
                    vertices = new List<Point>()
                };

                float sumX = 0, sumY = 0;
                foreach (var v in pts)
                {
                    data.vertices.Add(new Point(v));
                    sumX += v.x; sumY += v.y;
                }
                data.centerX = sumX / pointCount;
                data.centerY = sumY / pointCount;

                targetList.colliders.Add(data);
            }
        }

        string basePath = Application.persistentDataPath;
        string jsonNonTrigger = JsonUtility.ToJson(nonTriggerList, prettyPrint: true);
        string pathNon = Path.Combine(basePath, "colliders_nonTrigger.json");
        File.WriteAllText(pathNon, jsonNonTrigger);

        string jsonTrigger = JsonUtility.ToJson(triggerList, prettyPrint: true);
        string pathTrig = Path.Combine(basePath, "colliders_trigger.json");
        File.WriteAllText(pathTrig, jsonTrigger);

        Debug.Log($"Saved {nonTriggerList.colliders.Count} non-trigger colliders to: {pathNon}");
        Debug.Log($"Saved {triggerList.colliders.Count} trigger colliders to: {pathTrig}");
    }

    private void OnDrawGizmos()
    {
        if (nonTriggerList == null && triggerList == null) return;

        // draw non-trigger
        if (nonTriggerList != null)
        {
            Gizmos.color = Color.red;
            DrawList(nonTriggerList);
        }

        if (triggerList != null)
        {
            Gizmos.color = Color.yellow;
            DrawList(triggerList);
        }
    }

    private void DrawList(ColliderDataList list)
    {
        foreach (var c in list.colliders)
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
