using UnityEngine;

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
    [SerializeField] private ItemConfig _item;
    public ItemConfig Item => _item;
    [Header("Component")]
    [SerializeField] private EnemyController _prefab;
    public EnemyController Prefab => _prefab;
    public abstract void HandleMove(EnemyController controller);
    public abstract void HandleAttack(EnemyController controller);
    public virtual void OnDeath(EnemyController controller)
    {
        ItemPool.Instance.SpawnItem(_item, controller.transform.position);
    }
    public abstract void Initialize(EnemyController controller);
    private void OnValidate()
    {
        // Ensure AttackRadius < VisionRadius < ChaseRadius
        if (_attackRadius >= _visionRadius)
        {
            Debug.LogWarning($"Attack radius ({_attackRadius}) should be smaller than vision radius ({_visionRadius}). Adjusting...");
            _visionRadius = _attackRadius + 0.1f;
        }

        if (_visionRadius >= _chaseRadius)
        {
            Debug.LogWarning($"Vision radius ({_visionRadius}) should be smaller than chase radius ({_chaseRadius}). Adjusting...");
            _chaseRadius = _visionRadius + 0.1f;
        }
    }
}