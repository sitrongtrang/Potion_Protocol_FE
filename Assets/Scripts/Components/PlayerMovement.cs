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
    private float _directionChangeInterval;
    private bool _moveCancelled;
    private bool _isDashing = false;
    private bool _canDash = true;
    private SpriteRenderer _spriteRenderer;
    private AABBCollider _collider;
    private float _speedMultiplier = 1;

    public AABBCollider Collider => _collider;
    public Vector2 MoveDir => _moveDir;
    public Vector2 PlayerDir => _playerDir;
    public float SpeedMultiplier
    {
        get => _speedMultiplier;
        set => _speedMultiplier = value;
    }

    #region Initialization
    public void Initialize(PlayerController player, PlayerInputManager inputManager)
    {
        _player = player;
        _transform = player.transform;
        _playerConfig = player.Config;
        _inputManager = inputManager;
        _playerDir = Vector2.down;
        _directionChangeInterval = 1f * Time.deltaTime;

        _inputManager.controls.Player.Move.performed += ctx => TryMove(ctx);
        _inputManager.controls.Player.Move.canceled += ctx => CancelMove();
        _inputManager.controls.Player.Dash.performed += ctx => TryDash();

        _spriteRenderer = _player.GetComponent<SpriteRenderer>();

        if (_spriteRenderer) SetCollider();
    }
    #endregion

    public void Update()
    {
        if (_moveDir != Vector2.zero && !_isDashing)
        {
            SimulateMove(_moveDir, _playerConfig.MoveSpeed * _speedMultiplier);
        }
    }

    #region Movement
    private void TryMove(CallbackContext ctx)
    {
        if (_isDashing) return;
        _moveCancelled = false;
        Vector2 oldDir = _playerDir;
        _playerDir = ctx.ReadValue<Vector2>().normalized;
        if (oldDir == _playerDir || _moveDir != Vector2.zero)
        {
            _moveDir = _playerDir;
        }
        else
        {
            _moveDir = Vector2.zero;
            _player.StartCoroutine(StartMoving());
        }
        if (!_player) return;
        TriggerMoveAnimation(true);
    }

    private IEnumerator StartMoving()
    {
        // If user hold the move button for shorter than this amount of time, the player will just turn without moving
        yield return new WaitForSeconds(_directionChangeInterval);
        if (!_moveCancelled) _moveDir = _playerDir;
    }

    private void CancelMove()
    {
        _moveCancelled = true;
        _moveDir = Vector2.zero;
        if (!_player) return;
        TriggerMoveAnimation(false);
    }

    private void TryDash()
    {
        if (_canDash && _player) _player.StartCoroutine(SimulateDash());
    }

    private IEnumerator SimulateDash()
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

        Vector2 currentPos = new Vector2(_transform.position.x, _transform.position.y);
        Vector2 newPos = currentPos + desiredMove;

        Vector2 resolvedPos = ContextSolver.ResolveStatic(_transform.position, newPos, _collider, CollisionSystem.Tree);
        _player.transform.position = resolvedPos;
        SetCollider();
        return;
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

    private void SetCollider()
    {
        AABBCollider temp = AABBCollider.GetColliderBaseOnSprite(_spriteRenderer, _transform);
        if (_collider == null)
        {
            _collider = new AABBCollider(temp)
            {
                Layer = (int)EntityLayer.Player,
                Owner = _player.gameObject
            };
            _collider.Mask.SetLayer((int)EntityLayer.Obstacle);
        }
        else
        {
            _collider.SetSize(temp.Size);
            _collider.SetBottomLeft(temp.BottomLeft / 2f);
        }

    }
    #endregion
}
