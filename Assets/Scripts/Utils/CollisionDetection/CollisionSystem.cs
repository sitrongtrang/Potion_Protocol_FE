using System.Collections.Generic;
using UnityEngine;

public static class CollisionSystem
{
    public static QuadTree Tree;
    public static List<AABBCollider> DynamicColliders = new List<AABBCollider>();

    public static void Initialize(Rect worldBounds)
    {
        Tree = new QuadTree(0, worldBounds);
    }

    public static void InsertStaticCollider(AABBCollider collider)
    {
        Tree.Insert(collider);
    }

    public static void RemoveStaticCollider(AABBCollider collider)
    {
        Tree.Remove(collider);
    }

    public static void InsertDynamicCollider(AABBCollider collider)
    {
        DynamicColliders.Add(collider);
    }

    public static void RemoveDynamicCollider(AABBCollider collider)
    {
        DynamicColliders.Remove(collider);
    }

    public static List<AABBCollider> RetrieveCollided(AABBCollider collider)
    {
        List<AABBCollider> colliders = new List<AABBCollider>();
        for (int i = 0; i < DynamicColliders.Count; i++)
        {
            AABBCollider dynamicCollider = DynamicColliders[i];
            if (dynamicCollider.Bounds.Overlaps(collider.Bounds) && collider.Mask.Contains(dynamicCollider.Layer))
            {
                colliders.Add(dynamicCollider);
            }
        }
        return Tree.RetrieveCollided(collider, colliders);
    }

    public static List<AABBCollider> RayCast(Vector2 origin, Vector2 dir, float maxReach = Mathf.Infinity, EntityLayer layer = EntityLayer.Default)
    {
        Vector2 end = origin + dir.normalized * maxReach;
        float minX = Mathf.Min(origin.x, end.x);
        float minY = Mathf.Min(origin.y, end.y);
        float maxX = Mathf.Max(origin.x, end.x);
        float maxY = Mathf.Max(origin.y, end.y);

        AABBCollider rayCollider = new AABBCollider(new Vector2(minX, minY), new Vector2(maxX - minX, maxY - minY));
        rayCollider.Mask.SetLayer((int)layer);

        return RetrieveCollided(rayCollider);
    }

    public static void OnDrawGizmos()
    {
        if (Tree != null)
        {
            Tree.OnDrawGizmos();
        }

        foreach (var collider in DynamicColliders)
        {
            if (collider != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireCube(collider.Bounds.center, collider.Bounds.size);
            }
        }
    }
}
