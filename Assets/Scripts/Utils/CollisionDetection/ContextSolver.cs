using System.Collections.Generic;
using UnityEngine;

public class ContextSolver
{
    public static int MAX_ITERATION = 10;
    public static float PRECISION = 0.001f;

    public struct CollisionResult
    {
        public bool Hit;
        public float TOI; // Time of impact
        public Vector2 Normal;
    }

    public static Vector2 ResolveStatic(Vector2 from, Vector2 to, AABBCollider collider, QuadTree staticTree)
    {
        Vector2 result = from;

        // Step 1: resolve X axis
        Vector2 toX = new Vector2(to.x, result.y);
        result.x = BinaryResolveAxis(result, toX, collider, staticTree).x;

        // Step 2: resolve Y axis (using updated X)
        Vector2 toY = new Vector2(result.x, to.y);
        result.y = BinaryResolveAxis(result, toY, collider, staticTree).y;

        return result;
    }
    
    private static Vector2 BinaryResolveAxis(Vector2 from, Vector2 to, AABBCollider collider, QuadTree staticTree)
    {
        AABBCollider clone = new AABBCollider(collider);
        clone.SetBottomLeft(to - clone.Bounds.size / 2f);
        List<AABBCollider> collided = staticTree.RetrieveCollided(clone, new List<AABBCollider>());
        if (collided.Count == 0)
            return to;

        float moveDistance = Vector2.Distance(from, to);
        int maxIterations = Mathf.Min(MAX_ITERATION,
            (int)Mathf.CeilToInt(Mathf.Log(moveDistance / PRECISION) / Mathf.Log(2)));

        Vector2 start = from;
        Vector2 end = to;
        float precisionSqr = PRECISION * PRECISION;

        for (int i = 0; i < maxIterations; i++)
        {
            Vector2 mid = Vector2.Lerp(start, end, 0.5f);
            clone.SetBottomLeft(mid - clone.Bounds.size / 2f);

            collided = staticTree.RetrieveCollided(clone, new List<AABBCollider>());
            if (collided.Count != 0) end = mid;
            else start = mid;

            if (Vector2.SqrMagnitude(end - start) < precisionSqr) break;
        }
        return start;
    }

    public static Vector2 MoveWithSweep(Vector2 start, Vector2 target, AABBCollider collider, QuadTree tree)
    {
        Vector2 movement = target - start;
        float earliestTOI = 1.0f;

        Vector2 movementDelta = movement;
        Vector2 currentMin = collider.BottomLeft;
        Vector2 currentMax = collider.BottomLeft + collider.Size;
        Vector2 targetMin = currentMin + movement;
        Vector2 targetMax = currentMax + movement;

        Vector2 sweepMin = Vector2.Min(currentMin, targetMin);
        Vector2 sweepMax = Vector2.Max(currentMax, targetMax);

        AABBCollider sweepArea = new AABBCollider(sweepMin, sweepMax - sweepMin);
        sweepArea.Mask.SetLayer((int)EntityLayer.Obstacle);

        // Query nearby colliders
        List<AABBCollider> candidates = tree.RetrieveCollided(sweepArea, new List<AABBCollider>());

        foreach (var wall in candidates)
        {
            if (wall == collider) continue;
    
            var result = SweepAABB(collider, movement, wall);
            Debug.Log(wall.BottomLeft);
            Debug.Log(result.TOI);
            if (result.Hit && result.TOI < earliestTOI && result.TOI >= 0)
            {
                earliestTOI = result.TOI;
            }
        }

        float safeTOI = Mathf.Max(0, earliestTOI - 0.001f);
        return start + movement * safeTOI;
    }
    private static CollisionResult SweepAABB(AABBCollider moving, Vector2 movement, AABBCollider target)
    {
        Vector2 expandedMin = target.BottomLeft - moving.Size * 0.5f;
        Vector2 expandedSize = target.Size + moving.Size;
        AABBCollider expanded = new AABBCollider(expandedMin, expandedSize);
        expanded.Mask.SetLayer((int)EntityLayer.Obstacle);

        return RayVsAABB(moving.Bounds.center, movement, expanded);
    }

    private static CollisionResult RayVsAABB(Vector2 origin, Vector2 dir, AABBCollider box)
    {
        CollisionResult result = new CollisionResult { Hit = false, TOI = 1.0f };

        // Handle zero movement
        if (dir.sqrMagnitude < 0.0001f)
        {
            if (origin.x >= box.BottomLeft.x && origin.x <= (box.BottomLeft + box.Size).x &&
                origin.y >= box.BottomLeft.y && origin.y <= (box.BottomLeft + box.Size).y)
            {
                result.Hit = true;
                result.TOI = 0.0f;
            }
            return result;
        }

        Vector2 boxMin = box.BottomLeft;
        Vector2 boxMax = box.BottomLeft + box.Size;

        // Calculate intersection times for each axis
        Vector2 invDir = new Vector2(
            Mathf.Abs(dir.x) > 0.0001f ? 1.0f / dir.x : (dir.x > 0 ? float.MaxValue : float.MinValue),
            Mathf.Abs(dir.y) > 0.0001f ? 1.0f / dir.y : (dir.y > 0 ? float.MaxValue : float.MinValue)
        );

        Vector2 t1 = (boxMin - origin) * invDir;
        Vector2 t2 = (boxMax - origin) * invDir;

        Vector2 tmin = Vector2.Min(t1, t2);
        Vector2 tmax = Vector2.Max(t1, t2);

        float entry = Mathf.Max(tmin.x, tmin.y);
        float exit = Mathf.Min(tmax.x, tmax.y);

        // Check if intersection occurs within the movement range
        if (entry <= exit && entry >= 0.0f && entry <= 1.0f)
        {
            result.Hit = true;
            result.TOI = Mathf.Max(0, entry); // Ensure non-negative TOI

            // Calculate collision normal based on which axis was hit first
            if (Mathf.Abs(tmin.x - entry) < 0.0001f)
            {
                // Hit on X axis
                result.Normal = new Vector2(dir.x < 0 ? 1 : -1, 0);
            }
            else
            {
                // Hit on Y axis
                result.Normal = new Vector2(0, dir.y < 0 ? 1 : -1);
            }
        }

        return result;
    }
}