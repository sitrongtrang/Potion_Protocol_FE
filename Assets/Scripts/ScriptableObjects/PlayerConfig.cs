using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "PlayerConfig", menuName = "Scriptable Objects/PlayerConfig")]
public class PlayerConfig : ScriptableObject
{
    [SerializeField] private float _moveSpeed;
    public float MoveSpeed => _moveSpeed;

    [SerializeField] private float _dashCooldown;
    public float DashCooldown => _dashCooldown;

    [SerializeField] private float _dashSpeed;
    public float DashSpeed => _dashSpeed;

    [SerializeField] private float _dashTime;
    public float DashTime => _dashTime;

    [SerializeField] private float _attackCooldown;
    public float AttackCooldown => _attackCooldown;

    [SerializeField] private float _attackDelay;
    public float AttackDelay => _attackDelay;

    [SerializeField] private float[] _skillsCooldown;
    public float[] SkillsCoolDown => _skillsCooldown;

    [SerializeField] private PlayerController _prefab;
    public PlayerController Prefab => _prefab;

    public void Spawn(Vector2 position, InputActionAsset loadedInputAsset = null)
    {
        PlayerController player = Instantiate(_prefab, position, Quaternion.identity);
        player.Initialize(this, loadedInputAsset);
    } 
}
