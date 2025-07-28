using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[ExecuteInEditMode]
public class ColliderSaver : MonoBehaviour
{
    [Serializable]
    public class ColliderData
    {
        public float centerX, centerY;
        public float width, height;
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
                width = b.size.x,
                height = b.size.y
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

                var min = pts[0];
                var max = pts[0];
                float sumX = 0, sumY = 0;

                foreach (var p in pts)
                {
                    min = Vector2.Min(min, p);
                    max = Vector2.Max(max, p);
                    sumX += p.x;
                    sumY += p.y;
                }

                targetList.colliders.Add(new ColliderData
                {
                    centerX = sumX / pointCount,
                    centerY = sumY / pointCount,
                    width = max.x - min.x,
                    height = max.y - min.y
                });
            }
        }

        string basePath = Application.persistentDataPath;
        string pathNon = Path.Combine(basePath, "colliders_nonTrigger.json");
        string pathTrig = Path.Combine(basePath, "colliders_trigger.json");

        File.WriteAllText(pathNon, JsonUtility.ToJson(nonTriggerList, true));
        File.WriteAllText(pathTrig, JsonUtility.ToJson(triggerList, true));

        Debug.Log($"Saved {nonTriggerList.colliders.Count} non-trigger colliders to: {pathNon}");
        Debug.Log($"Saved {triggerList.colliders.Count} trigger colliders to: {pathTrig}");
    }

    private void OnDrawGizmos()
    {
        if (nonTriggerList == null && triggerList == null) return;

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
    }
}
