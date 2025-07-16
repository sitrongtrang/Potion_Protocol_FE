using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyController : MonoBehaviour
{
    [Header("Component")]
    [SerializeField] private Animator _animator;
    public Animator Animatr => _animator;
    public EnemyConfig EnemyConf { get; private set; }
    public EnemySpawner Spawner { get; private set; }
    public int PositionIndex { get; private set; }
    public int TypeIndex { get; private set; }
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

    [Header("Health Bar")]
    [SerializeField] private EnemyHealthUI healthBarPrefab;
    private EnemyHealthUI _healthBar;

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
    public void Initialize(EnemyConfig config, EnemySpawner spawner, Vector2 patrolCenter, int positionIndex, int typeIndex)
    {
        EnemyConf = config;
        Spawner = spawner;
        PositionIndex = positionIndex;
        TypeIndex = typeIndex;

        _currentHp = config.Hp;

        PatrolCenter = patrolCenter;

        _healthBar = Instantiate(
            healthBarPrefab,
            FindFirstObjectByType<Canvas>().transform
        );
        Vector3 hpOffset = Vector3.up * 1.2f;
        _healthBar.Initialize(
            transform,
            _currentHp,
            hpOffset
        );

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
            Pathfinding.Instance?.FindPath(transform.position, position, (path) => {
            if (path != null) {
                PathVectorList = path;
            }
        });
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
        Debug.Log("Hp " +  _currentHp);
        _healthBar.SetHp(_currentHp);
        if (_currentHp <= 0)
        {
            Die();
        }
    }
    private void Die()
    {
        Destroy(_healthBar.gameObject);
        EnemyConf.OnDeath(this);
        ItemPool.Instance.SpawnItem(EnemyConf.Item, transform.position);
        Spawner.UnoccupiedSpace(PositionIndex);
        Spawner.UnspawnedEnemy(TypeIndex);
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