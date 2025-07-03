using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("Components")]
    [field: SerializeField] public EnemyConfig EnemyConf { get; private set; }
    [Header("Movement")]
    public Vector3 PatrolTarget { get; private set; }
    public Vector3 PatrolCenter { get; private set; }
    private EnemyMovementState _currentMovementState;
    [Header("Combat")]
    private float _currentHp;
    private float _searchTimer;
    [field: SerializeField] public LayerMask PlayerLayer { get; private set; }
    private Transform _playerTransform;
    public Vector3 SearchTarget { get; private set; }
    public Vector3 LastSeenPlayerPosition { get; private set; }

    #region UNITY_METHODS
    private void Start()
    {
        // PatrolCenter = transform.position;
        _currentHp = EnemyConf.Hp;
        _currentMovementState = EnemyMovementState.Return;
    }
    public void Initialize(Vector3 patrolCenter)
    {
        PatrolCenter = patrolCenter;
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
        float distanceToPlayer = _playerTransform != null
            ? Vector3.Distance(transform.position, _playerTransform.position)
            : Mathf.Infinity;

        switch (_currentMovementState)
        {
            case EnemyMovementState.Patrol:
                HandlePatrol();
                break;

            case EnemyMovementState.Chase:
                HandleChase(distanceToPlayer);
                break;

            case EnemyMovementState.Attack:
                HandleAttack(distanceToPlayer);
                break;

            case EnemyMovementState.Search:
                HandleSearch();
                break;

            case EnemyMovementState.Return:
                HandleReturn();
                break;
        }
    }

    private void HandlePatrol()
    {
        EnemyConf.Patrol(this);
        if (_playerTransform != null)
            TransitionTo(EnemyMovementState.Chase);
    }

    private void HandleChase(float distanceToPlayer)
    {
        if (_playerTransform == null)
        {
            TransitionToSearch();
            return;
        }

        EnemyConf.Chase(this, _playerTransform);

        if (distanceToPlayer <= EnemyConf.AttackRadius)
            TransitionTo(EnemyMovementState.Attack);
        else if (distanceToPlayer > EnemyConf.ChaseRadius)
            TransitionTo(EnemyMovementState.Return);
    }

    private void HandleAttack(float distanceToPlayer)
    {
        if (_playerTransform == null)
        {
            TransitionToSearch();
            return;
        }

        EnemyConf.Attack(this, _playerTransform);

        if (distanceToPlayer > EnemyConf.AttackRadius)
            TransitionTo(EnemyMovementState.Chase);
    }

    private void HandleSearch()
    {
        EnemyConf.Search(this);
        _searchTimer -= Time.deltaTime;

        if (_searchTimer <= 0)
            TransitionTo(EnemyMovementState.Return);
        else if (_playerTransform != null)
            TransitionTo(EnemyMovementState.Chase);
    }

    private void HandleReturn()
    {
        EnemyConf.ReturnToSpawn(this);
        if (Vector3.Distance(transform.position, PatrolCenter) < 0.1f)
            TransitionTo(EnemyMovementState.Patrol);
    }

    private void TransitionTo(EnemyMovementState newState)
    {
        _currentMovementState = newState;
    }

    private void TransitionToSearch()
    {
        _searchTimer = EnemyConf.SearchDuration;
        TransitionTo(EnemyMovementState.Search);
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
    public void SetPatrolTarget(float radius)
    {
        PatrolTarget = PatrolCenter + (Vector3)Random.insideUnitCircle * radius;
    }
    public void SetSearchTarget(float radius)
    {
        SearchTarget = LastSeenPlayerPosition + (Vector3)Random.insideUnitCircle * radius;
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