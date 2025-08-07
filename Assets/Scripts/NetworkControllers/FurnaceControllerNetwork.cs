using UnityEngine;

public class FurnaceControllerNetwork : NetworkBehaviour
{
    private FurnaceNetworkInterpolator _interpolator = new(NetworkConstants.NET_INTERPOLATION_BUFFER_SIZE);
    private StationConfig _config;
    private SpriteRenderer _spriteRenderer;
    private AABBCollider _collider;
    private Vector2 _size;

    private bool _isCrafting;
    [SerializeField] private ProgressBarUI progressBarPrefab;
    private ProgressBarUI _progressBar;

    public StationConfig Config => _config;

    #region Unity Lifecycle
    void OnEnable()
    {
        NetworkEvents.OnMessageReceived += HandleNetworkMessage;
    }

    void OnDisable()
    {
        NetworkEvents.OnMessageReceived -= HandleNetworkMessage;
    }

    void FixedUpdate()
    {
        _interpolator.IncrementAndInterpolate((serverState) =>
        {
            if (_isCrafting)
            {
                if (!serverState.IsCrafting) _progressBar.gameObject.SetActive(false);
                else
                {
                    _progressBar.SetProgress(serverState.CraftTime);
                }
            }
            else
            {
                if (serverState.IsCrafting)
                {
                    if (_progressBar == null)
                    {
                        _progressBar = Instantiate(progressBarPrefab, FindFirstObjectByType<Canvas>().transform);
                        Vector2 barOffset = Vector2.down * 0.8f + (_config.Type == StationType.AlchemyStation ? Vector2.right * 0.4f : Vector2.zero);
                        _progressBar.Initialize(transform, barOffset);
                        _progressBar.SetMaxValue(serverState.CraftMaxTime);
                    }
                    else
                    {
                        _progressBar.gameObject.SetActive(true);
                    }
                    _progressBar.SetProgress(serverState.CraftTime);
                }
            }
            _isCrafting = serverState.IsCrafting;
        });
    }
    #endregion

    #region Initialization
    public override void Initialize(string entityId, ScriptableObject scriptableObject)
    {
        EntityId = entityId;
        if (scriptableObject is StationConfig stationConfig)
        {
            _config = stationConfig;
            _spriteRenderer.sprite = _config.Icon;
            SetCollider(ref _collider, _spriteRenderer, transform);
            CollisionSystem.InsertStaticCollider(_collider);
        }
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
                    for (int i = 0; i < gameStates.StationStates.Length; i++)
                    {
                        if (gameStates.StationStates[i].StationId == EntityId)
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

    public void SetCollider(ref AABBCollider collider, SpriteRenderer spriteRenderer, Transform transform)
    {
        if (collider == null)
        {
            collider = new AABBCollider(spriteRenderer, transform)
            {
                Layer = (int)EntityLayer.Obstacle,
                Owner = gameObject
            };
        }
        else
        {
            collider.SetSize(_size);
            Vector2 center = transform.position;
            collider.SetBottomLeft(center - _size / 2f);
        }
    }
}