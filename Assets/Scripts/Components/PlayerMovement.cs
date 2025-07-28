using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;

public class PlayerMovement
{
    [SerializeField] private PlayerConfig _playerConfig;
    private PlayerController _player;
    private Transform _transform;
    private PlayerInputManager _inputManager;
    private Vector2 _moveDir;
    //private bool _isMoving = false;
    //public bool IsMoving => _isMoving;
    private Vector2 _playerDir;
    private bool _isDashing = false;
    private bool _canDash = true;
    private SpriteRenderer _spriteRenderer;
    private AABBCollider _collider;
    private Vector2 _size = Vector2.zero;

    public Vector2 MoveDir => _moveDir;
    public Vector2 PlayerDir => _playerDir;

    public void Initialize(PlayerController player, PlayerInputManager inputManager)
    {
        _player = player;
        _transform = player.transform;
        _playerConfig = player.Config;
        _inputManager = inputManager;

        _inputManager.controls.Player.Move.performed += ctx => Move(ctx);
        _inputManager.controls.Player.Move.canceled += ctx => CancelMove();
        _inputManager.controls.Player.Dash.performed += ctx => Dash();

        _spriteRenderer = _player.GetComponent<SpriteRenderer>();
        if (_spriteRenderer) _size = _spriteRenderer.bounds.size;
        _collider = new AABBCollider(new Vector2(_transform.position.x - _size.x / 2, _transform.position.y - _size.y / 2), _size);
        _collider.Layer = (int)EntityLayer.Player;
        _collider.Mask.SetLayer((int)EntityLayer.Obstacle);
        _collider.Mask.SetLayer((int)EntityLayer.Player);
    }

    public void Update()
    {
        if (_moveDir != Vector2.zero)
        {
            SimulateMove(_moveDir, _playerConfig.MoveSpeed);
        }
    }

    private void Move(CallbackContext ctx)
    {
        if (_isDashing) return;
        _moveDir = ctx.ReadValue<Vector2>().normalized;
        _playerDir = _moveDir;
        if (!_player) return;
        TriggerMoveAnimation(true);
    }

    private void CancelMove()
    {
        _moveDir = Vector2.zero;
        if (!_player) return;
        TriggerMoveAnimation(false);
    }

    private void Dash()
    {
        if (_canDash && _player) _player.StartCoroutine(DashCoroutine());
    }

    private IEnumerator DashCoroutine()
    {
        _isDashing = true;
        _canDash = false;

        float dashTime = 0f;
        while (dashTime < _playerConfig.DashTime)
        {
            SimulateMove(_playerDir, _playerConfig.DashSpeed);
            TriggerMoveAnimation(true);

            dashTime += Time.deltaTime;
            yield return null;
        }
        _isDashing = false;

        TriggerMoveAnimation(false);

        // Dash Cooldown
        yield return new WaitForSeconds(_playerConfig.DashCooldown);
        _canDash = true;
    }

    void SimulateMove(Vector2 moveDirection, float speed)
    {
        float deltaTime = Time.deltaTime;
        Vector2 desiredMove = moveDirection * speed * deltaTime;

        // Step 1: Predict future position
        Vector2 currentPos = new Vector2(_transform.position.x, _transform.position.y);
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
            _player.transform.position = newPos;
            _collider.SetBottomLeft(newPos - _size / 2);
        }
        else
        {
            // Try moving along X axis only
            Vector2 xMove = new Vector2(desiredMove.x, 0);
            Vector2 xPos = currentPos + xMove;
            AABBCollider xCollider = new AABBCollider(xPos - _size / 2, _size);
            xCollider.Layer = _collider.Layer;
            xCollider.Mask = _collider.Mask;
            if (CollisionSystem.RetrieveCollided(xCollider).Count == 0)
            {
                _player.transform.position = xPos;
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
                _player.transform.position = yPos;
                _collider.SetBottomLeft(xPos - _size / 2);
                return;
            }

            // No movement if both X and Y are blocked
        }
    }

    private void TriggerMoveAnimation(bool isMoving = true)
    {
        if (_player.Animatr)
        {
            _player.Animatr.SetFloat("MoveX", _playerDir.x);
            _player.Animatr.SetFloat("MoveY", _playerDir.y);
            _player.Animatr.SetBool("IsMoving", isMoving);
        }
        if (_player.SwordAnimatr)
        {
            _player.SwordAnimatr.SetFloat("MoveX", _playerDir.x);
            _player.SwordAnimatr.SetFloat("MoveY", _playerDir.y);
        }
    }
}
