using UnityEngine;

[CreateAssetMenu(fileName = "PlayerAttribute", menuName = "Scriptable Objects/PlayerAttribute")]
public class PlayerAttribute : ScriptableObject
{
    [SerializeField] private float _moveSpeed;
    public float MoveSpeed => _moveSpeed;
    [SerializeField] float _dashCooldown;
    public float DashCoolDown => _dashCooldown;
    [SerializeField] float _dashSpeed;
    public float DashSpeed => _dashSpeed;
    [SerializeField] private float _dashTime;
    public float DashTime => _dashTime;
    [SerializeField] float _attackCooldown;
    public float AttackCooldown => _attackCooldown;

    [SerializeField] float[] _skillsCooldown;
    public float[] SkillsCoolDown => _skillsCooldown;
    
}
