using UnityEngine;

public class EnemyControllerNetwork : NetworkBehaviour
{
    private EnemyNetworkInterpolator _interpolator = new(NetworkConstants.NET_INTERPOLATION_BUFFER_SIZE);
    private EnemyConfig _config;
    private Animator _animator;
    private SpriteRenderer _spriteRenderer;
    private float _currentHp;
    [SerializeField] private EnemyHealthUI _healthBarPrefab;
    private EnemyHealthUI _healthBar;
    [SerializeField] private EnemyImpactUI _enemyImpactUI;
    [SerializeField] private float heightHealthBar;

    public EnemyConfig Config => _config;

    #region Unity Lifecycle
    void OnEnable()
    {
        NetworkEvents.OnMessageReceived += HandleNetworkMessage;
    }

    void OnDisable()
    {
        NetworkEvents.OnMessageReceived -= HandleNetworkMessage;
    }

    void OnDestroy()
    {
        Destroy(_healthBar.gameObject);
    }

    void FixedUpdate()
    {
        _interpolator.IncrementAndInterpolate((serverState) =>
        {
            bool xChanged = Mathf.Abs(serverState.PositionX - transform.position.x) >= 0.01f;
            bool yChanged = Mathf.Abs(serverState.PositionY - transform.position.y) >= 0.01f;

            _animator.SetBool("IsMoving", xChanged || yChanged);
            if (xChanged) _animator.SetFloat("MoveX", 1);
            else _animator.SetFloat("MoveX", 0);
            if (yChanged) _animator.SetFloat("MoveY", 1);
            else _animator.SetFloat("MoveY", 0);

            transform.position = new(serverState.PositionX, serverState.PositionY);
            
            if (serverState.Health < _currentHp)
            {
                _enemyImpactUI.Flash();
                _healthBar.SetHp(_currentHp);
            }
            _currentHp = serverState.Health;
        });
    }
    #endregion

    #region Initialization
    public override void Initialize(string entityId, ScriptableObject scriptableObject)
    {
        EntityId = entityId;
        if (scriptableObject is EnemyConfig enemyConfig)
        {
            _config = enemyConfig;
            _currentHp = _config.Hp;
            _animator = GetComponent<Animator>();
            _animator.runtimeAnimatorController = _config.Anim;
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _spriteRenderer.sprite = _config.Icon;
        }

        _healthBar = Instantiate(_healthBarPrefab, FindFirstObjectByType<Canvas>().transform);
        Vector2 hpOffset = Vector2.up * heightHealthBar;
        _healthBar.Initialize(transform, _currentHp, hpOffset);
    }
    #endregion

    #region Serve Message
    private void HandleNetworkMessage(ServerMessage message)
    {
        switch (message.MessageType)
        {
            case NetworkMessageTypes.Server.GameState.StateUpdate:
                GameStatesUpdate gameStatesUpdate = (GameStatesUpdate)message;
                _interpolator.Store(gameStatesUpdate.GameStates, (gameStates) =>
                {
                    for (int i = 0; i < gameStates.EnemyStates.Length; i++)
                    {
                        if (gameStates.EnemyStates[i].EnemyId == EntityId)
                        {
                            return i;
                        }
                    }
                    return -1;
                });
                break;

        }
    }
    #endregion
}