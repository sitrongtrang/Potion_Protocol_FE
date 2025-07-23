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
    public Color gizmoColor = Color.red;
    private ColliderDataList loadedList;

    private void Start()
    {
        SaveColliders();
    }

    public void SaveColliders()
    {
        var list = new ColliderDataList { colliders = new List<ColliderData>() };
        var all = mapGameObject.GetComponentsInChildren<Collider2D>();
        foreach (var col in all)
        {
            if (col is TilemapCollider2D || col is CompositeCollider2D)
                continue;

            var b = col.bounds;
            list.colliders.Add(new ColliderData
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

                list.colliders.Add(data);
            }
        }

        string json = JsonUtility.ToJson(list, prettyPrint: true);
        string path = Path.Combine(Application.persistentDataPath, "colliders.json");
        File.WriteAllText(path, json);
        Debug.Log($"Saved {list.colliders.Count} total colliders to:\n{path}");
        loadedList = list;
    }

    private void OnDrawGizmos()
    {
        if (loadedList == null) return;

        Gizmos.color = gizmoColor;
        foreach (var c in loadedList.colliders)
        {
            var verts = c.vertices;
            int n = verts.Count;
            if (n < 2) continue;
            for (int i = 0; i < n; i++)
            {
                var a = verts[i];
                var b = verts[(i + 1) % n];
                Gizmos.DrawLine(
                    new Vector3(a.x, a.y, 0f),
                    new Vector3(b.x, b.y, 0f)
                );
            }
        }
    }
}
