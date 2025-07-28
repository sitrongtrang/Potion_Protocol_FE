using UnityEngine;

public class AABBCollider : CustomCollider
{
    private Vector2 _bottomLeft;
    private Vector2 _size;
    public override ColliderType Type => ColliderType.AABB;

    public AABBCollider(Vector2 bottomLeft, Vector2 size)
    {
        _bottomLeft = bottomLeft;
        _size = size; 
    }
    
    public AABBCollider(AABBCollider other)
    {
        _bottomLeft = other._bottomLeft;
        _size = other._size;
        Layer = other.Layer;
        Mask = other.Mask;

    }
    public override Rect Bounds => new Rect(_bottomLeft, _size);

    public override bool IsColliding(CustomCollider other)
    {
        if (other.Type == ColliderType.AABB)
        {
            AABBCollider aabb = (AABBCollider)other;
            // AABB vs AABB
            return Bounds.Overlaps(aabb.Bounds);
        }
        else if (other.Type == ColliderType.Circle)
        {
            CircleCollider circle = (CircleCollider)other;
            // AABB vs Circle
            Vector2 closestPoint = new Vector2(
                Mathf.Clamp(circle.Center.x, Bounds.xMin, Bounds.xMax),
                Mathf.Clamp(circle.Center.y, Bounds.yMin, Bounds.yMax)
            );
            float distSq = (closestPoint - circle.Center).sqrMagnitude;
            return distSq <= circle.Radius * circle.Radius;
        }

        return false; // Unknown collider type
    }

    public void SetBottomLeft(Vector2 newBottomLeft)
    {
        _bottomLeft = newBottomLeft;
    }
}