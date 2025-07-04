using NUnit.Framework;
using UnityEngine;

public class PlayerMovement : IPlayerAction
{
    private PlayerController _player;
    private Vector2 _moveDir;
    public Vector2 MoveDir => _moveDir;
    private bool _isMoving = false;
    public bool IsMoving => _isMoving;
    [SerializeField] private PlayerConfig _playerConfig; // player attribute from data asset
    private Vector2 _playerDir; // player direction
    public Vector2 PlayerDir => _playerDir;
    [SerializeField] private float _dashCD; // player

    private float _dashTime = 0;
    private bool _isDashing = false;

    public void Initialize(PlayerController player)
    {
        _player = player;
        _playerConfig = player.Config;
    }
    public void MyUpdate()
    {
        // move logic
        _moveDir = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")).normalized;
        _isMoving = _moveDir != Vector2.zero;
        if (_isMoving) _playerDir = _moveDir;
        _player.gameObject.transform.Translate(_moveDir * _playerConfig.MoveSpeed * Time.deltaTime);

        // dash logic
        if (_dashCD <= 0 && Input.GetKey(KeyCode.L))
        {
            // Dash();
            _isDashing = true;
        }
        if (_dashTime <= _playerConfig.DashTime && _isDashing)
        {
            _player.gameObject.transform.Translate(_playerDir * _playerConfig.DashSpeed * Time.deltaTime);
            _dashTime += Time.deltaTime;
        }
        else if (_dashTime >= _playerConfig.DashTime)
        {
            _isDashing = false;
            _dashCD = _playerConfig.DashCoolDown;
            _dashTime = 0;
        }
        _dashCD -= Time.deltaTime;

    }
}
