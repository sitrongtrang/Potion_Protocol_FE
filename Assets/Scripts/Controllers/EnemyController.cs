using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("Component")]
    public EnemyConfig EnemyConf { get; private set; }
    public EnemySpawner Spawner { get; private set; }
    public int IndexPosition { get; private set; }
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
    private bool _isPlayerInRange = false;
    public bool IsPlayerInRange => _isPlayerInRange;
    [Header("Enemy State")]
    public BasicStateMachine<EnemyController, EnemyState> BasicStateMachine { get; private set; }
    public EnemyState CurrentEnemyStateEnum => BasicStateMachine.CurrentStateEnum;
    [Header("Pathfinding")]
    public int CurrentPathIndex { get; private set; }
    public List<Vector2> PathVectorList { get; private set; }
    [SerializeField] private bool _movementIgnoreObstacles;

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
    public void Initialize(EnemyConfig config, EnemySpawner spawner, Vector2 patrolCenter, int indexPosition)
    {
        EnemyConf = config;
        Spawner = spawner;
        IndexPosition = indexPosition;

        _currentHp = config.Hp;

        PatrolCenter = patrolCenter;
        BasicStateMachine = new(this);

        config.Initialize(this);
    }
    #endregion

    #region MOVEMENT
    public void CurrentPathIndexIncrement()
    {
        CurrentPathIndex += 1;
    }
    public void StopMoving()
    {
        PathVectorList = null;
    }
    public void SetTargetToMove(Vector2 position)
    {
        TargetToMove = position;
        CurrentPathIndex = 0;
        if (!_movementIgnoreObstacles)
        {
            PathVectorList = Pathfinding.Instance?.FindPath(transform.position, position);
            if (PathVectorList != null && PathVectorList.Count > 0)
            {
                PathVectorList.RemoveAt(0);
            }
        }
        else
        {
            PathVectorList = new() { position };
        }
    }
    public bool IsTooFarFromPatrolCenter()
    {
        return Vector2.Distance(transform.position, PatrolCenter) > EnemyConf.ChaseRadius;
    }
    #endregion

    #region COMBAT
    private void HandleDetection()
    {
        Collider2D playerHit = Physics2D.OverlapCircle(transform.position, EnemyConf.VisionRadius, PlayerLayer);
        if (playerHit != null && playerHit.CompareTag("Player"))
        {
            _playerTransform = playerHit.transform;
            Vector2 start = transform.position;
            Vector2 end = _playerTransform.position;
            RaycastHit2D obstacleHit = Physics2D.Linecast(start, end, ObstacleLayer);
            if (obstacleHit.transform == null)
            {
                _isPlayerInRange = true;
                LastSeenPlayerPosition = _playerTransform.position;
            }
            else
            {
                _isPlayerInRange = false;
            }
        }
        else
        {
            _playerTransform = null;
        }
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
        Spawner.UnoccupiedSpace(IndexPosition);
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