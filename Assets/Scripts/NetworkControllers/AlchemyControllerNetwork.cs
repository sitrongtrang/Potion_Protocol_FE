using UnityEngine;

public class AlchemyControllerNetwork : NetworkBehaviour
{
    private AlchemyNetworkInterpolator _interpolator = new(NetworkConstants.NET_INTERPOLATION_BUFFER_SIZE);
    private StationConfig _config;
    private bool _isCrafting;
    private string[] _itemIds;
    [SerializeField] private GameObject _table;
    [SerializeField] private GameObject[] _itemsOnTable;
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
            _itemIds = serverState.ItemIds;
            DisplayItems(_itemIds);
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

    private void DisplayItems(string[] itemIds)
    {
        for (int i = 0; i < _itemsOnTable.Length; i++)
        {
            if (i >= itemIds[i].Length)
            {
                _itemsOnTable[i].GetComponent<SpriteRenderer>().sprite = null;
                _itemsOnTable[i].SetActive(false);
            }
            else
            {
                ItemConfig itemConfig = IdConfigMapper.MapItemIdToConfig(itemIds[i]);
                _itemsOnTable[i].SetActive(true);
                _itemsOnTable[i].GetComponent<SpriteRenderer>().sprite = itemConfig.Icon;
            }
        }
    }
}