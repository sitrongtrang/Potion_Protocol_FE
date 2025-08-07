using UnityEngine;

public class AABBCollider : CustomCollider
{
    private Vector2 _bottomLeft;
    private Vector2 _size;
    public override ColliderType Type => ColliderType.AABB;

    public Vector2 Size => _size;
    public Vector2 BottomLeft => _bottomLeft;

    public static AABBCollider MakeColliderBaseOnCenter(Vector2 center, Vector2 size)
    {
        Vector2 bottomLeft = center - size * 0.5f;
        return new AABBCollider(bottomLeft, size);
    }

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

    public static AABBCollider GetColliderBaseOnSprite(SpriteRenderer spriteRenderer, Transform transform)
    {
        Sprite sprite = spriteRenderer.sprite;
        float pivotY = sprite.pivot.y;

        float pivotToBottom = pivotY / sprite.rect.height * spriteRenderer.bounds.size.y;

        float colliderWidth = spriteRenderer.bounds.size.x;
        float colliderHeight = 2f * pivotToBottom;

        Vector2 colliderBottomLeft = new Vector2(
            transform.position.x - colliderWidth / 2f,
            transform.position.y - pivotToBottom
        );

        Vector2 colliderSize = new Vector2(colliderWidth, colliderHeight);
        return new AABBCollider(colliderBottomLeft, colliderSize);
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

    public void SetSize(Vector2 newSize)
    {
        _size = newSize;
    }
    
    public bool Raycast(CustomRay ray, out float distance)
    {
        distance = float.MaxValue;

        Vector2 invDir = new Vector2(1f / ray.Direction.x, 1f / ray.Direction.y);
        Vector2 tMin = (_bottomLeft - ray.Origin) * invDir;
        Vector2 tMax = (_bottomLeft + _size - ray.Origin) * invDir;

        Vector2 t1 = Vector2.Min(tMin, tMax);
        Vector2 t2 = Vector2.Max(tMin, tMax);

        float tNear = Mathf.Max(t1.x, t1.y);
        float tFar = Mathf.Min(t2.x, t2.y);

        if (tNear > tFar || tFar < 0)
            return false;

        distance = tNear > 0 ? tNear : tFar;
        return true;
    }
}