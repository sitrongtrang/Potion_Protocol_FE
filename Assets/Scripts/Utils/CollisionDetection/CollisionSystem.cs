using System.Collections.Generic;
using UnityEngine;

public static class CollisionSystem
{
    public static QuadTree Tree;

    public static void Initialize(Rect worldBounds)
    {
        Tree = new QuadTree(0, worldBounds);
    }

    public static void InsertCollider(AABBCollider collider)
    {
        Tree.Insert(collider);
    }

    public static List<AABBCollider> RetrieveCollided(AABBCollider collider)
    {
        return Tree.RetrieveCollided(collider, new List<AABBCollider>());
    }
}
