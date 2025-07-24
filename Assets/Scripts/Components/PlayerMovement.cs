using System.Collections;
using UnityEngine;

public class PlayerMovement
{
    private PlayerController _player;
    private PlayerInputManager _inputManager;
    
    private Vector2 _moveDir;
    public Vector2 MoveDir => _moveDir;
    //private bool _isMoving = false;
    //public bool IsMoving => _isMoving;
    [SerializeField] private PlayerConfig _playerConfig; // player attribute from data asset
    private Vector2 _playerDir; // player direction
    public Vector2 PlayerDir => _playerDir;
    private bool _isDashing = false;
    private bool _canDash = true;


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

    public void MyUpdate()
    {
        if (_moveDir != Vector2.zero)
        {
            Vector2 targetPos = _player.Rb.position + _moveDir * _playerConfig.MoveSpeed * Time.fixedDeltaTime;
            _player.Rb.MovePosition(targetPos);
        }
        //_player.gameObject.transform.Translate(_moveDir * _playerConfig.MoveSpeed * Time.deltaTime);
    }

    private IEnumerator Dash()
    {
        _isDashing = true;
        _canDash = false;

        float dashTime = 0f;
        while (dashTime < _playerConfig.DashTime)
        {

            Vector2 newPos = _player.Rb.position + _playerDir * _playerConfig.DashSpeed * Time.fixedDeltaTime;
            _player.Rb.MovePosition(newPos);

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

            dashTime += Time.deltaTime;
            yield return null;
        }
        _isDashing = false;

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

        // Dash Cooldown
        yield return new WaitForSeconds(_playerConfig.DashCooldown);
        _canDash = true;
    }
}
