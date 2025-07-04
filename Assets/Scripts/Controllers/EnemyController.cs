using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public EnemyConfig EnemyConf { get; private set; }
    [Header("Movement")]
    public Vector3 TargetToMove { get; private set; }
    public Vector3 PatrolCenter { get; private set; }
    // private EnemyMovementState _currentMovementState;
    [Header("Combat")]
    private float _currentHp;
    [field: SerializeField] public LayerMask PlayerLayer { get; private set; }
    private Transform _playerTransform;
    public Vector3 LastSeenPlayerPosition { get; private set; }
    public BasicStateMachine<EnemyController, EnemyState> BasicStateMachine { get; private set; }
    public EnemyState CurrentEnemyStateEnum;
    // [Header("Cooldown")]
    // private float _searchTimer;
    // public float _searchInterval;
    // public float _patrolInterval;
    // public float _attackInterval;

    #region UNITY_METHODS
    [ContextMenu("Test")]
    public void Initialize(EnemyConfig config)
    {
        EnemyConf = config;
        _currentHp = config.Hp;
        
        PatrolCenter = transform.position;
        BasicStateMachine = new(this);

        IdleState idleState = new IdleState(this);
        PatrolState patrolState = new PatrolState(this);
        ChaseState chaseState = new ChaseState(this);
        ReturnState returnState = new ReturnState(this);
        SearchState searchState = new SearchState(this);
        AttackState attackState = new AttackState(this);

        BasicStateMachine.AddState(EnemyState.Idle, idleState);
        BasicStateMachine.AddState(EnemyState.Patrol, patrolState);
        BasicStateMachine.AddState(EnemyState.Chase, chaseState);
        BasicStateMachine.AddState(EnemyState.Return, returnState);
        BasicStateMachine.AddState(EnemyState.Search, searchState);
        BasicStateMachine.AddState(EnemyState.Attack, attackState);

        BasicStateMachine.ChangeState(EnemyState.Return);
    }
    private void Update()
    {
        BasicStateMachine?.Execute();
        // HandleState();
        HandleDetection();
        CurrentEnemyStateEnum = BasicStateMachine.CurrentStateEnum;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {

    }
    private void OnDrawGizmosSelected()
    {

    }
    #endregion

    // #region STATE
    // private void HandleState()
    // {
    //     float distanceToPlayer = _playerTransform != null
    //         ? Vector3.Distance(transform.position, _playerTransform.position)
    //         : Mathf.Infinity;

    //     switch (_currentMovementState)
    //     {
    //         case EnemyMovementState.Patrol:
    //             HandlePatrol();
    //             break;

    //         case EnemyMovementState.Chase:
    //             HandleChase(distanceToPlayer);
    //             break;

    //         case EnemyMovementState.Attack:
    //             HandleAttack(distanceToPlayer);
    //             break;

    //         case EnemyMovementState.Search:
    //             HandleSearch();
    //             break;

    //         case EnemyMovementState.Return:
    //             HandleReturn();
    //             break;
    //     }
    // }

    // private void HandlePatrol()
    // {
    //     _patrolInterval -= Time.deltaTime;
    //     if (_patrolInterval <= 0f)
    //     {
    //         EnemyConf.Patrol(this);
    //     }

    //     if (_playerTransform != null)
    //         TransitionTo(EnemyMovementState.Chase);
    // }

    // private void HandleChase(float distanceToPlayer)
    // {
    //     if (_playerTransform == null)
    //     {
    //         TransitionTo(EnemyMovementState.Search);
    //         return;
    //     }

    //     EnemyConf.Chase(this, _playerTransform);

    //     if (distanceToPlayer <= EnemyConf.AttackRadius)
    //         TransitionTo(EnemyMovementState.Attack);
    //     else if (distanceToPlayer > EnemyConf.ChaseRadius)
    //         TransitionTo(EnemyMovementState.Return);
    // }

    // private void HandleAttack(float distanceToPlayer)
    // {
    //     if (_playerTransform == null)
    //     {
    //         TransitionTo(EnemyMovementState.Search);
    //         return;
    //     }

    //     _attackInterval -= Time.deltaTime;
    //     if (_attackInterval <= 0f)
    //     {
    //         EnemyConf.Attack(this, _playerTransform);
    //     }

    //     if (distanceToPlayer > EnemyConf.AttackRadius)
    //         TransitionTo(EnemyMovementState.Chase);
    // }

    // private void HandleSearch()
    // {
    //     _searchTimer -= Time.deltaTime;
    //     _searchInterval -= Time.deltaTime;

    //     if (_searchInterval <= 0f)
    //     {
    //         EnemyConf.Search(this);
    //     }

    //     if (_searchTimer <= 0)
    //         TransitionTo(EnemyMovementState.Return);
    //     else if (_playerTransform != null)
    //         TransitionTo(EnemyMovementState.Chase);
    // }

    // private void HandleReturn()
    // {
    //     EnemyConf.ReturnToSpawn(this);
    //     if (Vector3.Distance(transform.position, PatrolCenter) < 0.1f)
    //         TransitionTo(EnemyMovementState.Patrol);
    // }

    // private void TransitionTo(EnemyMovementState newState)
    // {
    //     _currentMovementState = newState;
    //     if (newState == EnemyMovementState.Patrol)
    //         _patrolInterval = 0f;
    //     else if (newState == EnemyMovementState.Search)
    //     {
    //         _searchTimer = EnemyConf.SearchDuration;
    //         _searchInterval = 0f;
    //     }
    //     else if (newState == EnemyMovementState.Attack)
    //         _attackInterval = 0f;
    // }
    // #endregion

    #region MOVEMENT
    public void SetTargetToMove(Vector3 position)
    {
        TargetToMove = position;
    }
    // public void SetReturnTarget()
    // {
    //     TargetToMove = PatrolCenter;
    // }
    // public void SetPatrolTarget(float radius)
    // {
    //     TargetToMove = PatrolCenter + (Vector3)Random.insideUnitCircle * radius;
    // }
    // public void SetSearchTarget(float radius)
    // {
    //     TargetToMove = LastSeenPlayerPosition + (Vector3)Random.insideUnitCircle * radius;
    // }
    public bool IsTooFarFromPatrolCenter()
    {
        return Vector3.Distance(transform.position, PatrolCenter) > EnemyConf.ChaseRadius;
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
    public bool IsPlayerInRange()
    {
        return _playerTransform != null;
    }
    public float DistanceToPlayer()
    {
        return _playerTransform != null
            ? Vector3.Distance(transform.position, _playerTransform.position)
            : Mathf.Infinity;
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
    void DrawSearchRadius()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(LastSeenPlayerPosition, EnemyConf.SearchRadius);
    }
    #endregion
}