using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

[ExecuteInEditMode]
public class ColliderSaver : MonoBehaviour
{
    [Serializable]
    public class RectangleData
    {
        public float centerX, centerY;
        public float width, height;
        
        public RectangleData(float cx, float cy, float w, float h)
        {
            centerX = cx;
            centerY = cy;
            width = w;
            height = h;
        }
    }

    [Serializable]
    public class ColliderDataList
    {
        public List<RectangleData> colliders;
    }

    public GameObject mapGameObject;
    public List<Tilemap> tilemaps;

    public ColliderDataList nonTriggerList;
    public ColliderDataList triggerList;

    public void SaveColliders()
    {
        nonTriggerList = new ColliderDataList { colliders = new List<RectangleData>() };
        triggerList = new ColliderDataList { colliders = new List<RectangleData>() };
        
        var all = mapGameObject.GetComponentsInChildren<Collider2D>(true);
        foreach (var col in all)
        {
            if (col is TilemapCollider2D)
                continue;

            var targetList = col.isTrigger ? triggerList : nonTriggerList;
            var b = col.bounds;
            targetList.colliders.Add(new RectangleData(
                b.center.x, b.center.y, 
                b.size.x, b.size.y
            ));
        }

        foreach (var tm in tilemaps)
        {
            var tilemapCollider = tm.GetComponent<TilemapCollider2D>();
            if (tilemapCollider == null) continue;

            ExtractTileColliders(tm, tilemapCollider);
        }

        // string basePath = Application.persistentDataPath;
        // string jsonNonTrigger = JsonUtility.ToJson(nonTriggerList, prettyPrint: true);
        // string pathNon = Path.Combine(basePath, "colliders_map.json");
        // File.WriteAllText(pathNon, jsonNonTrigger);

        // string jsonTrigger = JsonUtility.ToJson(triggerList, prettyPrint: true);
        // string pathTrig = Path.Combine(basePath, "colliders_station.json");
        // File.WriteAllText(pathTrig, jsonTrigger);

        // Debug.Log($"Saved {nonTriggerList.colliders.Count} colliders_map to: {pathNon}");
        // Debug.Log($"Saved {triggerList.colliders.Count} colliders_station to: {pathTrig}");
    }

    private void ExtractTileColliders(Tilemap tm, TilemapCollider2D tilemapCollider)
    {
        tm.RefreshAllTiles();

        Vector3 cellSize = tm.cellSize;
        Vector3 cellCenter = new Vector3(cellSize.x * 0.5f, cellSize.y * 0.5f, 0);

        foreach (Vector3Int pos in tm.cellBounds.allPositionsWithin)
        {
            if (!tm.HasTile(pos)) continue;
            Tile.ColliderType colliderType = tm.GetColliderType(pos);
            if (colliderType == Tile.ColliderType.None) continue;

            Vector3 worldPos = tm.CellToWorld(pos);
            Vector3 worldCenter = worldPos + cellCenter;

            if (colliderType == Tile.ColliderType.Sprite)
            {
                TileBase tile = tm.GetTile(pos);
                Sprite sprite = null;

                if (tile is Tile standardTile)
                {
                    sprite = standardTile.sprite;
                }
                else if (tile is RuleTile ruleTile)
                {
                    foreach (RuleTile.TilingRule rule in ruleTile.m_TilingRules)
                    {
                        if (rule.m_Sprites != null && rule.m_Sprites.Length > 0)
                        {
                            sprite = rule.m_Sprites[0];
                            break;
                        }
                    }
                }

                if (sprite != null)
                {
                    ProcessSpriteCollider(sprite, worldCenter, tm.transform, tilemapCollider.isTrigger);
                }
                else
                {
                    CreateRectangleCollider(worldCenter, cellSize, tm.transform, tilemapCollider.isTrigger);
                }
            }
            else
            {
                CreateRectangleCollider(worldCenter, cellSize, tm.transform, tilemapCollider.isTrigger);
            }
        }
    }

    private void ProcessSpriteCollider(Sprite sprite, Vector3 center, Transform transform, bool isTrigger)
    {
        List<Vector2> vertices = new List<Vector2>();
        int shapeCount = sprite.GetPhysicsShapeCount();

        if (shapeCount > 0)
        {
            sprite.GetPhysicsShape(0, vertices);

            Vector2 pivotOffset = sprite.pivot / sprite.pixelsPerUnit - new Vector2(0.25f, 0.25f);

            List<Vector2> worldVertices = new List<Vector2>();
            foreach (Vector2 vertex in vertices)
            {
                Vector2 adjustedVertex = (Vector2)center + vertex - pivotOffset;
                worldVertices.Add(transform.TransformPoint(adjustedVertex));
            }

            ProcessVertices(worldVertices, isTrigger);
        }
        else
        {
            CreateRectangleCollider(center, sprite.bounds.size, transform, isTrigger);
        }
    }

    private void CreateRectangleCollider(Vector3 center, Vector3 size, Transform transform, bool isTrigger)
    {
        Vector3 worldCenter = transform.TransformPoint(center);
        Vector3 worldSize = transform.TransformVector(size);
        
        var targetList = isTrigger ? triggerList : nonTriggerList;
        targetList.colliders.Add(new RectangleData(
            worldCenter.x, worldCenter.y,
            Mathf.Abs(worldSize.x), Mathf.Abs(worldSize.y)
        ));
    }

    private void ProcessVertices(List<Vector2> worldVertices, bool isTrigger)
    {
        // Kiểm tra xem có phải hình chữ nhật không
        if (IsRectangle(worldVertices))
        {
            CreateRectangleFromVertices(worldVertices, isTrigger);
        }
        else if (IsLShape(worldVertices))
        {
            // Tách hình chữ L thành hai hình chữ nhật
            var rectangles = SplitLShapeToRectangles(worldVertices);
            var targetList = isTrigger ? triggerList : nonTriggerList;
            
            foreach (var rect in rectangles)
            {
                targetList.colliders.Add(rect);
            }
        }
        else
        {
            // Fallback: tạo bounding box
            CreateBoundingBoxFromVertices(worldVertices, isTrigger);
        }
    }

    private bool IsRectangle(List<Vector2> vertices)
    {
        if (vertices.Count != 4) return false;
        
        // Sắp xếp vertices theo thứ tự
        var sorted = vertices.OrderBy(v => v.x).ThenBy(v => v.y).ToList();
        
        // Kiểm tra có 4 góc vuông không
        float tolerance = 0.01f;
        return Mathf.Abs(sorted[0].x - sorted[1].x) < tolerance &&
               Mathf.Abs(sorted[2].x - sorted[3].x) < tolerance &&
               Mathf.Abs(sorted[0].y - sorted[2].y) < tolerance &&
               Mathf.Abs(sorted[1].y - sorted[3].y) < tolerance;
    }

    private bool IsLShape(List<Vector2> vertices)
    {
        // Hình chữ L có 6 đỉnh
        return vertices.Count == 6;
    }

    private List<RectangleData> SplitLShapeToRectangles(List<Vector2> vertices)
    {
        var rectangles = new List<RectangleData>();
        
        // Sắp xếp vertices để xác định hướng của hình chữ L
        var minX = vertices.Min(v => v.x);
        var maxX = vertices.Max(v => v.x);
        var minY = vertices.Min(v => v.y);
        var maxY = vertices.Max(v => v.y);
        
        // Tìm điểm góc trong (điểm lõm của hình L)
        Vector2 innerCorner = FindInnerCorner(vertices);
        
        // Xác định hướng của hình L dựa vào vị trí góc trong
        LShapeType shapeType = DetermineLShapeType(innerCorner, minX, maxX, minY, maxY);
        
        switch (shapeType)
        {
            case LShapeType.TopLeft:
                // Hình L góc trên trái
                rectangles.Add(new RectangleData(
                    (minX + innerCorner.x) / 2f, (innerCorner.y + maxY) / 2f,
                    innerCorner.x - minX, maxY - innerCorner.y
                ));
                rectangles.Add(new RectangleData(
                    (minX + maxX) / 2f, (minY + innerCorner.y) / 2f,
                    maxX - minX, innerCorner.y - minY
                ));
                break;
                
            case LShapeType.TopRight:
                // Hình L góc trên phải
                rectangles.Add(new RectangleData(
                    (innerCorner.x + maxX) / 2f, (innerCorner.y + maxY) / 2f,
                    maxX - innerCorner.x, maxY - innerCorner.y
                ));
                rectangles.Add(new RectangleData(
                    (minX + maxX) / 2f, (minY + innerCorner.y) / 2f,
                    maxX - minX, innerCorner.y - minY
                ));
                break;
                
            case LShapeType.BottomLeft:
                // Hình L góc dưới trái
                rectangles.Add(new RectangleData(
                    (minX + innerCorner.x) / 2f, (minY + innerCorner.y) / 2f,
                    innerCorner.x - minX, innerCorner.y - minY
                ));
                rectangles.Add(new RectangleData(
                    (innerCorner.x + maxX) / 2f, (innerCorner.y + maxY) / 2f,
                    maxX - innerCorner.x, maxY - innerCorner.y
                ));
                break;
                
            case LShapeType.BottomRight:
                // Hình L góc dưới phải
                rectangles.Add(new RectangleData(
                    (innerCorner.x + maxX) / 2f, (minY + innerCorner.y) / 2f,
                    maxX - innerCorner.x, innerCorner.y - minY
                ));
                rectangles.Add(new RectangleData(
                    (minX + innerCorner.x) / 2f, (innerCorner.y + maxY) / 2f,
                    innerCorner.x - minX, maxY - innerCorner.y
                ));
                break;
        }
        
        return rectangles;
    }

    private enum LShapeType
    {
        TopLeft, TopRight, BottomLeft, BottomRight
    }

    private Vector2 FindInnerCorner(List<Vector2> vertices)
    {
        // Tìm điểm có góc lõm (góc trong > 180 độ)
        for (int i = 0; i < vertices.Count; i++)
        {
            Vector2 prev = vertices[(i - 1 + vertices.Count) % vertices.Count];
            Vector2 curr = vertices[i];
            Vector2 next = vertices[(i + 1) % vertices.Count];
            
            Vector2 v1 = prev - curr;
            Vector2 v2 = next - curr;
            
            float cross = v1.x * v2.y - v1.y * v2.x;
            if (cross > 0) // Góc lõm
            {
                return curr;
            }
        }
        
        return vertices[0]; // Fallback
    }

    private LShapeType DetermineLShapeType(Vector2 innerCorner, float minX, float maxX, float minY, float maxY)
    {
        float tolerance = 0.01f;
        
        bool nearLeft = Mathf.Abs(innerCorner.x - minX) < tolerance;
        bool nearRight = Mathf.Abs(innerCorner.x - maxX) < tolerance;
        bool nearBottom = Mathf.Abs(innerCorner.y - minY) < tolerance;
        bool nearTop = Mathf.Abs(innerCorner.y - maxY) < tolerance;
        
        if (nearLeft && nearTop) return LShapeType.TopLeft;
        if (nearRight && nearTop) return LShapeType.TopRight;
        if (nearLeft && nearBottom) return LShapeType.BottomLeft;
        if (nearRight && nearBottom) return LShapeType.BottomRight;
        
        // Fallback: xác định dựa vào vị trí tương đối
        bool leftHalf = innerCorner.x < (minX + maxX) / 2f;
        bool bottomHalf = innerCorner.y < (minY + maxY) / 2f;
        
        if (leftHalf && !bottomHalf) return LShapeType.TopLeft;
        if (!leftHalf && !bottomHalf) return LShapeType.TopRight;
        if (leftHalf && bottomHalf) return LShapeType.BottomLeft;
        return LShapeType.BottomRight;
    }

    private void CreateRectangleFromVertices(List<Vector2> vertices, bool isTrigger)
    {
        var bounds = GetBoundsFromVertices(vertices);
        var targetList = isTrigger ? triggerList : nonTriggerList;
        targetList.colliders.Add(new RectangleData(
            bounds.center.x, bounds.center.y,
            bounds.size.x, bounds.size.y
        ));
    }

    private void CreateBoundingBoxFromVertices(List<Vector2> vertices, bool isTrigger)
    {
        var bounds = GetBoundsFromVertices(vertices);
        var targetList = isTrigger ? triggerList : nonTriggerList;
        targetList.colliders.Add(new RectangleData(
            bounds.center.x, bounds.center.y,
            bounds.size.x, bounds.size.y
        ));
    }

    private Bounds GetBoundsFromVertices(List<Vector2> vertices)
    {
        Bounds bounds = new Bounds(vertices[0], Vector3.zero);
        foreach (Vector2 point in vertices)
        {
            bounds.Encapsulate(point);
        }
        return bounds;
    }

    private void OnDrawGizmos()
    {
        if (nonTriggerList == null && triggerList == null) return;

        if (nonTriggerList != null)
        {
            Gizmos.color = Color.red;
            DrawRectangleList(nonTriggerList);
        }

        if (triggerList != null)
        {
            Gizmos.color = Color.yellow;
            DrawRectangleList(triggerList);
        }
    }

    private void DrawRectangleList(ColliderDataList list)
    {
        foreach (var rect in list.colliders)
        {
            Vector3 center = new Vector3(rect.centerX, rect.centerY, 0f);
            Vector3 size = new Vector3(rect.width, rect.height, 0f);
            
            Vector3 min = center - size * 0.5f;
            Vector3 max = center + size * 0.5f;
            
            // Vẽ hình chữ nhật
            Gizmos.DrawLine(new Vector3(min.x, min.y, 0f), new Vector3(max.x, min.y, 0f));
            Gizmos.DrawLine(new Vector3(max.x, min.y, 0f), new Vector3(max.x, max.y, 0f));
            Gizmos.DrawLine(new Vector3(max.x, max.y, 0f), new Vector3(min.x, max.y, 0f));
            Gizmos.DrawLine(new Vector3(min.x, max.y, 0f), new Vector3(min.x, min.y, 0f));
        }
    }
}