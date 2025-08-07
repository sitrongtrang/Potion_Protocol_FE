using UnityEngine;

public class AlchemyControllerNetwork : NetworkBehaviour
{
    private AlchemyNetworkInterpolator _interpolator = new(NetworkConstants.NET_INTERPOLATION_BUFFER_SIZE);
    private GameStateHandler _gameStateHandler;
    private StationConfig _config;
    private SpriteRenderer _spriteRenderer;
    private AABBCollider _collider;
    private Vector2 _size;
    private AABBCollider _tableCollider;
    [SerializeField] private GameObject _table;
    [SerializeField] private GameObject[] _itemsOnTable;
    [SerializeField] private ProgressBarUI progressBarPrefab;
    private ProgressBarUI _progressBar;
    private bool _isCrafting;
    private string[] _itemTypeIds;

    public StationConfig Config => _config;

    #region Unity Lifecycle
    void Start()
    {
        _gameStateHandler = FindFirstObjectByType<GameStateHandler>();
    }
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
            _itemTypeIds = serverState.ItemTypeIds;
            DisplayItems(_itemTypeIds);
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
            for (int i = 0; i < _itemsOnTable.Length; i++) _itemsOnTable[i].SetActive(false);

            SetCollider(ref _collider, _spriteRenderer, transform);
            SetCollider(ref _tableCollider, _table.GetComponent<SpriteRenderer>(), _table.transform);
            CollisionSystem.InsertStaticCollider(_collider);
            CollisionSystem.InsertStaticCollider(_tableCollider);
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
                _interpolator.Store(gameStatesUpdate.GameStates, (gameStates) => { return 0; });
                break;
        }
    }
    #endregion

    private void DisplayItems(string[] itemTypeIds)
    {
        for (int i = 0; i < _itemsOnTable.Length; i++)
        {
            if (i >= itemTypeIds[i].Length)
            {
                _itemsOnTable[i].GetComponent<SpriteRenderer>().sprite = null;
                _itemsOnTable[i].SetActive(false);
            }
            else
            {
                ScriptableObject scriptableObject = _gameStateHandler.PrefabsMap.GetSO(itemTypeIds[i]);
                if (scriptableObject is ItemConfig itemConfig)
                {
                    _itemsOnTable[i].SetActive(true);
                    _itemsOnTable[i].GetComponent<SpriteRenderer>().sprite = itemConfig.Icon;
                }
            }
        }
    }
    
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