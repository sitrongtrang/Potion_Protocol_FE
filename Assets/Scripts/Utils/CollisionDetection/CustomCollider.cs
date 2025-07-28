using UnityEngine;

public abstract class CustomCollider
{
    public CustomLayerMask Mask = new CustomLayerMask();
    public int Layer;
    public abstract ColliderType Type { get; }
    public abstract Rect Bounds { get; }

    public abstract bool IsColliding(CustomCollider other);
}