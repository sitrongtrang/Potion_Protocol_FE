using System.Collections;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;

public class PlayerMovement
{
    [SerializeField] private PlayerConfig _playerConfig;
    private PlayerController _player;
    private PlayerInputManager _inputManager;
    private Vector2 _moveDir;
    //private bool _isMoving = false;
    //public bool IsMoving => _isMoving;
    private Vector2 _playerDir;
    private bool _isDashing = false;
    private bool _canDash = true;

    public Vector2 MoveDir => _moveDir;
    public Vector2 PlayerDir => _playerDir;

    private float _speedMultiplier = 1;
    public void SetSpeedMultiplier(float multiplier)
    {
        _speedMultiplier *= multiplier;
        Debug.Log("Set speed multiplier");
    }

    public void Initialize(PlayerController player, PlayerInputManager inputManager)
    {
        _player = player;
        _playerConfig = player.Config;
        _inputManager = inputManager;

        _inputManager.controls.Player.Move.performed += ctx => Move(ctx);
        _inputManager.controls.Player.Move.canceled += ctx => CancelMove();
        _inputManager.controls.Player.Dash.performed += ctx => Dash();
    }

    public void Update()
    {
        if (_moveDir != Vector2.zero)
        {
            Vector2 targetPos = _player.Rb.position + _moveDir * _playerConfig.MoveSpeed * Time.fixedDeltaTime * _speedMultiplier;
            Debug.Log(_speedMultiplier);
            _player.Rb.MovePosition(targetPos);
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
            Vector2 newPos = _playerConfig.MoveSpeed * Time.fixedDeltaTime * _moveDir + _player.Rb.position;
            
            _player.Rb.MovePosition(newPos);

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
