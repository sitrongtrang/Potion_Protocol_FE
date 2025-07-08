using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("Components")]
    [field: SerializeField] public EnemyConfig EnemyConf { get; private set; }
    [Header("Movement")]
    public Vector3 SpawnPosition { get; private set; }
    public Vector3 PatrolTarget { get; private set; }
    public Vector3 PatrolCenter;
    private EnemyMovementState _currentMovementState;
    [Header("Combat")]
    private float _currentHp;
    private float _searchTimer;
    [field: SerializeField] public LayerMask PlayerLayer { get; private set; }
    private Transform _playerTransform;
    public Vector3 LastSeenPlayerPosition { get; private set; }

    #region UNITY_METHODS
    private void Start()
    {
        SpawnPosition = transform.position;
        _currentHp = EnemyConf.Hp;
        _currentMovementState = EnemyMovementState.Return;
    }
    private void Update()
    {

    }
    private void OnCollisionEnter2D(Collision2D collision)
    {

    }
    private void OnDrawGizmosSelected()
    {

    }
    #endregion

    #region STATE
    private void HandleState()
    {
        float distanceToPlayer = _playerTransform != null ? Vector3.Distance(transform.position, _playerTransform.position) : Mathf.Infinity;

        switch (_currentMovementState)
        {
            case EnemyMovementState.Patrol:
                EnemyConf.Patrol(this);
                if (_playerTransform != null)
                    _currentMovementState = EnemyMovementState.Chase;
                break;

            case EnemyMovementState.Chase:
                if (_playerTransform != null)
                {
                    EnemyConf.Chase(this, _playerTransform);

                    if (distanceToPlayer <= EnemyConf.AttackRadius)
                        _currentMovementState = EnemyMovementState.Attack;
                    else if (distanceToPlayer > EnemyConf.ChaseRadius)
                        _currentMovementState = EnemyMovementState.Return;
                }
                else
                {
                    _currentMovementState = EnemyMovementState.Search;
                    _searchTimer = EnemyConf.SearchDuration;
                }
                break;

            case EnemyMovementState.Attack:
                if (_playerTransform != null)
                {
                    EnemyConf.Attack(this, _playerTransform);

                    if (distanceToPlayer > EnemyConf.AttackRadius)
                        _currentMovementState = EnemyMovementState.Chase;
                }
                else
                {
                    _currentMovementState = EnemyMovementState.Search;
                    _searchTimer = EnemyConf.SearchDuration;
                }
                break;

            case EnemyMovementState.Search:
                EnemyConf.Search(this);
                _searchTimer -= Time.deltaTime;
                if (_searchTimer <= 0)
                    _currentMovementState = EnemyMovementState.Return;
                else if (_playerTransform != null)
                    _currentMovementState = EnemyMovementState.Chase;
                break;

            case EnemyMovementState.Return:
                EnemyConf.ReturnToSpawn(this);
                if (Vector3.Distance(transform.position, SpawnPosition) < 0.1f)
                    _currentMovementState = EnemyMovementState.Patrol;
                break;
        }
    }
    #endregion

    #region MOVEMENT
    public void MoveTowards(Vector3 target)
    {
        transform.position = Vector3.MoveTowards(transform.position, target, EnemyConf.Speed * Time.deltaTime);
    }
    public bool HasPatrolTarget()
    {
        return PatrolTarget != default && Vector3.Distance(transform.position, PatrolTarget) > 0.1f;
    }
    public void SetPatrolTargetAroundSpawn(float radius)
    {
        PatrolTarget = SpawnPosition + (Vector3)Random.insideUnitCircle * radius;
    }
    #endregion

    #region COMBAT
    private void HandleDetection()
    {
        Collider2D hit = Physics2D.OverlapCircle(transform.position, EnemyConf.VisionRadius, PlayerLayer);
        if (hit != null && hit.CompareTag("Player"))
        {
            _playerTransform = hit.transform;
            LastSeenPlayerPosition = _playerTransform.position;
        }
        else
        {
            _playerTransform = null;
        }
    }
    public void TakeDamage(float amount)
    {
        _currentHp -= amount;
        if (_currentHp <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        EnemyConf.OnDeath(this);
        Destroy(gameObject);
    }
    #endregion

    #region DEBUG
    private void TestDamage()
    {
        TakeDamage(1);
    }
    void DrawChaseRadius()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, EnemyConf.ChaseRadius);
    }
    void DrawPatrolRadius()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(PatrolCenter, EnemyConf.PatrolRadius);
    }
    #endregion
}