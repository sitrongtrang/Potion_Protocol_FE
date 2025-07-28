using System.Collections.Generic;
using UnityEngine;

public class BasicController : MonoBehaviour
{
    private AABBCollider _collider;
    private SpriteRenderer _spriteRenderer;
    private Vector2 _size = Vector2.zero;
    private void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        if (_spriteRenderer) _size = _spriteRenderer.bounds.size;
        _collider = new AABBCollider(new Vector2(transform.position.x - _size.x / 2, transform.position.y - _size.y / 2), _size);
        _collider.Layer = (int)EntityLayer.Player;
        _collider.Mask.SetLayer((int)EntityLayer.Obstacle);
        _collider.Mask.SetLayer((int)EntityLayer.Player);
    }
    void Update()
    {
        Move();
    }

    void Move()
    {
        Vector2 input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        if (input == Vector2.zero) return;

        Vector2 moveDirection = input.normalized;
        float speed = 5f;
        float deltaTime = Time.deltaTime;
        Vector2 desiredMove = moveDirection * speed * deltaTime;

        // Step 1: Predict future position
        Vector2 currentPos = new Vector2(transform.position.x, transform.position.y);
        Vector2 newPos = currentPos + desiredMove;

        // Step 2: Create a temp collider at the new position
        AABBCollider tempCollider = new AABBCollider(newPos - _size / 2, _size);
        tempCollider.Layer = _collider.Layer;
        tempCollider.Mask = _collider.Mask;

        // Step 3: Check collisions at the new position
        List<AABBCollider> collided = CollisionSystem.RetrieveCollided(tempCollider);

        // Step 4: If no collision, move freely
        if (collided.Count == 0)
        {
            transform.position = newPos;
            _collider.SetBottomLeft(newPos - _size / 2);
        }
        else
        {
            Debug.Log("Collision detected, attempting to resolve...");
            // Optional: slide along walls
            // Try moving along X axis only
            Vector2 xMove = new Vector2(desiredMove.x, 0);
            Vector2 xPos = currentPos + xMove;
            AABBCollider xCollider = new AABBCollider(xPos - _size / 2, _size);
            xCollider.Layer = _collider.Layer;
            xCollider.Mask = _collider.Mask;
            if (CollisionSystem.RetrieveCollided(xCollider).Count == 0)
            {
                transform.position = xPos;
                _collider.SetBottomLeft(xPos - _size / 2);
                return;
            }

            // Try moving along Y axis only
            Vector2 yMove = new Vector2(0, desiredMove.y);
            Vector2 yPos = currentPos + yMove;
            AABBCollider yCollider = new AABBCollider(yPos - _size / 2, _size);
            yCollider.Layer = _collider.Layer;
            yCollider.Mask = _collider.Mask;
            if (CollisionSystem.RetrieveCollided(yCollider).Count == 0)
            {
                transform.position = yPos;
                _collider.SetBottomLeft(xPos - _size / 2);
                return;
            }

            // No movement if both X and Y are blocked
        }
    }

}