using UnityEngine;

public struct CustomRay
{
    public Vector2 Origin;
    public Vector2 Direction;

    public CustomRay(Vector2 origin, Vector2 direction)
    {
        Origin = origin;
        Direction = direction.normalized;
    }
}
