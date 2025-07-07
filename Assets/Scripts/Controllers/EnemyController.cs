using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public EnemyConfig EnemyConf { get; private set; }
    [Header("Movement")]
    public Vector2 TargetToMove { get; private set; }
    public Vector2 PatrolCenter { get; private set; }
    [Header("Combat")]
    private float _currentHp;
    [SerializeField] private LayerMask _playerLayer;
    public LayerMask PlayerLayer => _playerLayer;
    [SerializeField] private LayerMask _obstacleLayer;
    public LayerMask ObstacleLayer => _obstacleLayer;
    private Transform _playerTransform;
    public Vector2 LastSeenPlayerPosition { get; private set; }
    public BasicStateMachine<EnemyController, EnemyState> BasicStateMachine { get; private set; }
    [Header("Enemy State")]
    public EnemyState CurrentEnemyStateEnum => BasicStateMachine.CurrentStateEnum;

    #region UNITY_METHODS
    private void Update()
    {
        BasicStateMachine?.Execute();
    }
    private void FixedUpdate()
    {
        HandleDetection();
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {

    }
    private void OnDrawGizmosSelected()
    {

    }
    #endregion

    #region STATE
    public void Initialize(EnemyConfig config, Vector2 patrolCenter)
    {
        EnemyConf = config;
        _currentHp = config.Hp;

        PatrolCenter = patrolCenter;
        BasicStateMachine = new(this);

        config.Initialize(this);
        
        // This is for example only, add states in enemy config

        // IdleState idleState = new IdleState(this);
        // PatrolState patrolState = new PatrolState(this);
        // ChaseState chaseState = new ChaseState(this);
        // ReturnState returnState = new ReturnState(this);
        // SearchState searchState = new SearchState(this);
        // AttackState attackState = new AttackState(this);

        // BasicStateMachine.AddState(EnemyState.Idle, idleState);
        // BasicStateMachine.AddState(EnemyState.Patrol, patrolState);
        // BasicStateMachine.AddState(EnemyState.Chase, chaseState);
        // BasicStateMachine.AddState(EnemyState.Return, returnState);
        // BasicStateMachine.AddState(EnemyState.Search, searchState);
        // BasicStateMachine.AddState(EnemyState.Attack, attackState);

        // BasicStateMachine.ChangeState(EnemyState.Return);
    }
    #endregion

    #region MOVEMENT
    public void SetTargetToMove(Vector2 position)
    {
        TargetToMove = position;
    }
    public bool IsTooFarFromPatrolCenter()
    {
        return Vector2.Distance(transform.position, PatrolCenter) > EnemyConf.ChaseRadius;
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
        if (_playerTransform == null) return false;
        Vector2 start = transform.position;
        Vector2 end = _playerTransform.position;
        RaycastHit2D hit = Physics2D.Linecast(start, end, ObstacleLayer);
        return _playerTransform != null;
    }
    public float DistanceToPlayer()
    {
        return _playerTransform != null
            ? Vector2.Distance(transform.position, _playerTransform.position)
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
    void DrawVisionRadius()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(PatrolCenter, EnemyConf.VisionRadius);
    }
    void DrawSearchRadius()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(LastSeenPlayerPosition, EnemyConf.SearchRadius);
    }
    #endregion
}