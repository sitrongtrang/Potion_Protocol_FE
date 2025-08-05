using UnityEngine;

public class CircleCollider : CustomCollider
{
    private Vector2 _center;
    private float _radius;
    public override ColliderType Type => ColliderType.Circle;

    public CircleCollider(Vector2 center, float radius)
    {
        _center = center;
        _radius = radius;
    }

    public CircleCollider(CircleCollider other)
    {
        _center = other._center;
        _radius = other._radius;
        Layer = other.Layer;
        Mask = other.Mask;
    }

    public Vector2 Center => _center;
    public float Radius => _radius;

    public override Rect Bounds => new Rect(
        _center.x - _radius,
        _center.y - _radius,
        _radius * 2,
        _radius * 2
    );

    public void SetCenter(Vector2 newCenter)
    {
        _center = newCenter;
    }

    public override bool IsColliding(CustomCollider other)
    {
        if (other.Type == ColliderType.Circle)
        {
            CircleCollider circle = (CircleCollider)other;
            // Circle vs Circle
            float distSq = (circle._center - _center).sqrMagnitude;
            float combinedR = circle._radius + _radius;
            return distSq <= combinedR * combinedR;
        }
        else if (other.Type == ColliderType.AABB)
        {
            AABBCollider aabb = (AABBCollider)other;
            // Circle vs AABB
            Vector2 closestPoint = new Vector2(
                Mathf.Clamp(_center.x, aabb.Bounds.xMin, aabb.Bounds.xMax),
                Mathf.Clamp(_center.y, aabb.Bounds.yMin, aabb.Bounds.yMax)
            );
            float distSq = (closestPoint - _center).sqrMagnitude;
            return distSq <= _radius * _radius;
        }

        return false; // Unknown collider type
    }
}
