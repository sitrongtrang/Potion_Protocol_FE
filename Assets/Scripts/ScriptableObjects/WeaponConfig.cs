using UnityEngine;

[CreateAssetMenu(fileName = "WeaponConfig", menuName = "Scriptable Objects/WeaponConfig")]
public class WeaponConfig : EntityConfig
{
    [SerializeField] private float _attackRange;
    [SerializeField] private int _attackDamage;

    public float AttackRange => _attackRange;
    public int AttackDamage => _attackDamage;
}
