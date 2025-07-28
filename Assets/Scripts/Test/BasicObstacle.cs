using UnityEngine;
using System.Collections.Generic;

public class BasicObstacle : MonoBehaviour
{
    private AABBCollider _collider;
    private SpriteRenderer _spriteRenderer;
    private Vector2 _size = Vector2.zero;
    private void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        if (_spriteRenderer) _size = _spriteRenderer.bounds.size;
        _collider = new AABBCollider(new Vector2(transform.position.x - _size.x / 2, transform.position.y - _size.y / 2), _size);
        _collider.Layer = (int)EntityLayer.Obstacle;
        CollisionSystem.InsertCollider(_collider);
    }
}