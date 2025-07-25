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
    private float _speedMultiplier = 1;
    public void SetSpeedMultiplier(float multiplier)
    {
        _speedMultiplier *= multiplier;
    }

    public Vector2 MoveDir => _moveDir;
    public Vector2 PlayerDir => _playerDir;

    public void Initialize(PlayerController player, PlayerInputManager inputManager)
    {
        _player = player;
        _playerConfig = player.Config;
        _inputManager = inputManager;

        _inputManager.controls.Player.Move.performed += ctx =>
        {
            _moveDir = ctx.ReadValue<Vector2>().normalized;
            _playerDir = _moveDir;
            if (!_player) return;
            if (_player.Animatr)
            {
                _player.Animatr.SetFloat("MoveX", _playerDir.x);
                _player.Animatr.SetFloat("MoveY", _playerDir.y);
                _player.Animatr.SetBool("IsMoving", true);
            }
            if (_player.SwordAnimatr)
            {
                _player.SwordAnimatr.SetFloat("MoveX", _playerDir.x);
                _player.SwordAnimatr.SetFloat("MoveY", _playerDir.y);
            }

        };
        _inputManager.controls.Player.Move.canceled += ctx =>
        {
            _moveDir = Vector2.zero;
            if (!_player) return;
            if (_player.Animatr)
            {
                _player.Animatr.SetFloat("MoveX", _playerDir.x);
                _player.Animatr.SetFloat("MoveY", _playerDir.y);
                _player.Animatr.SetBool("IsMoving", false);
            }
            if (_player.SwordAnimatr)
            {
                _player.SwordAnimatr.SetFloat("MoveX", _playerDir.x);
                _player.SwordAnimatr.SetFloat("MoveY", _playerDir.y);
            }
        };
        _inputManager.controls.Player.Dash.performed += ctx =>
        {
            if (_canDash && _player) _player.StartCoroutine(Dash());
        };
    }

    public void Update()
    {
        if (_moveDir != Vector2.zero)
        {
            Vector2 targetPos = _playerConfig.MoveSpeed * Time.fixedDeltaTime * _speedMultiplier * _moveDir + _player.Rb.position;
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
            Vector2 newPos = _player.Rb.position + _playerDir * _playerConfig.DashSpeed * Time.fixedDeltaTime;
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
