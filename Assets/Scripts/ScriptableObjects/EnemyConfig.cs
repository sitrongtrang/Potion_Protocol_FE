using UnityEngine;

// [CreateAssetMenu(fileName = "EnemyConfig", menuName = "Scriptable Objects/Enemy/Configuration")]
public abstract class EnemyConfig : ScriptableObject
{
    [Header("Basic Stats")]
    [SerializeField] private string _name;
    public string Name => _name;

    [SerializeField] private float _hp;
    public float Hp => _hp;

    [SerializeField] private float _speed;
    public float Speed => _speed;

    [SerializeField] private float _damage;
    public float Damage => _damage;

    [Header("Patrol")]
    [SerializeField] private float _patrolRadius;
    public float PatrolRadius => _patrolRadius;

    [SerializeField] private float _patrolInterval;
    public float PatrolInterval => _patrolInterval;

    [Header("Chase")]
    [SerializeField] private float _chaseRadius;
    public float ChaseRadius => _chaseRadius;

    [Header("Attack")]
    [SerializeField] private float _visionRadius;
    public float VisionRadius => _visionRadius;

    [SerializeField] private float _attackRadius;
    public float AttackRadius => _attackRadius;

    [SerializeField] private float _attackInterval;
    public float AttackInterval => _attackInterval;

    [Header("Search")]
    [SerializeField] private float _searchRadius;
    public float SearchRadius => _searchRadius;

    [SerializeField] private float _searchDuration;
    public float SearchDuration => _searchDuration;

    [SerializeField] private float _searchInterval;
    public float SearchInterval => _searchInterval;

    [Header("Drop")]
    [SerializeField] private string[] ingredientsConfig;
    public string[] IngredientsConfig => ingredientsConfig;    
    public abstract void Move(EnemyController controller);
    public abstract void Patrol(EnemyController controller);
    public abstract void Chase(EnemyController controller, Transform target);
    public abstract void Attack(EnemyController controller, Transform target);
    public abstract void Search(EnemyController controller);
    public abstract void ReturnToSpawn(EnemyController controller);
    public abstract void OnDeath(EnemyController controller);
}