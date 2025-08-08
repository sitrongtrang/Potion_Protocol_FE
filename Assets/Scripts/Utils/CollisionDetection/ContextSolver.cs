using System.Collections.Generic;
using UnityEngine;

public class ContextSolver
{
    public static int MAX_ITERATION = 10;
    public static float PRECISION = 0.001f;

    public static Vector2 ResolveStatic(Vector2 from, Vector2 to, AABBCollider collider, QuadTree staticTree)
    {
        AABBCollider clone = new AABBCollider(collider);
        clone.SetBottomLeft(to - clone.Bounds.size / 2f);
        List<AABBCollider> collided = staticTree.RetrieveCollided(clone, new List<AABBCollider>());
        if (collided.Count == 0)
        {
            return to;
        }

        Vector2 result = to;

        float moveDistance = Vector2.Distance(from, to);
        int maxIterations = Mathf.Min(MAX_ITERATION, (int)Mathf.CeilToInt(Mathf.Log(moveDistance / PRECISION) / Mathf.Log(2)));

        Vector2 start = from;
        Vector2 end = to;
        float precisionSqr = PRECISION * PRECISION;

        for (int i = 0; i < maxIterations; i++) {
            Vector2 mid = Vector2.Lerp(start, end, 0.5f);
            clone.SetBottomLeft(mid - clone.Bounds.size / 2f);

            collided = staticTree.RetrieveCollided(clone, new List<AABBCollider>());
            if (collided.Count != 0) end = mid;
            else start = mid;
            
            if (Vector2.SqrMagnitude(end - start) < precisionSqr) break;

        }
        result = start;
        return result;

    }
}