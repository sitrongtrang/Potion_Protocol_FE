using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(NetworkIdentity))]
public class TstController : MonoBehaviour
{
    [Header("Components")]
    private float _sendTimer = 0f;
    public NetworkIdentity Identity { get; private set; }

    [Header("Syncing")]
    private PlayerNetworkSimulator _simulator = new(NetworkConstants.NET_PRED_BUFFER_SIZE);
    private PlayerNetworkInterpolator _interpolator = new(NetworkConstants.NET_INTERPOLATION_BUFFER_SIZE);
    // private int _serverSequence = int.MaxValue;
    // private bool _isReconciling = false;
    private PlayerInputSnapshot _inputListener = new();
    // private NetworkPredictionBuffer<PlayerInputMessage, PlayerSnapshot> _networkPredictionBuffer = new(NetworkConstants.NET_PRED_BUFFER_SIZE);
    // private NetworkInterpolationBuffer<PlayerStateInterpolateData> _networkInterpolationBuffer = new(NetworkConstants.NET_INTERPOLATION_BUFFER_SIZE);

    [Header("Input")]
    private PlayerInputManager _inputManager;

    [Header("Game Components")]
    public PlayerInventory Inventory { get; private set; }
    public PlayerInteraction Interaction { get; private set; }
    private PlayerConfig _config;
    private Animator _animator;
    private SpriteRenderer _spriteRenderer;

    #region Unity Lifecycle
    void OnEnable()
    {
        NetworkEvents.OnMessageReceived += HandleNetworkMessage;
    }
    void OnDisable()
    {
        NetworkEvents.OnMessageReceived -= HandleNetworkMessage;
    }
    void Awake()
    {
        Application.runInBackground = true;
        Identity = GetComponent<NetworkIdentity>();
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }
    void Update()
    {
        if (!Identity.IsLocalPlayer) return;

        if (_inputManager != null)
        {
            _inputListener.MoveDir = _inputManager.controls.Player.Move.ReadValue<Vector2>().normalized;

            _inputListener.AttackPressed = _inputManager.controls.Player.Attack.WasPressedThisFrame();
            _inputListener.DashPressed = _inputManager.controls.Player.Dash.WasPressedThisFrame();
            _inputListener.PickupPressed = _inputManager.controls.Player.Pickup.WasPressedThisFrame();
            _inputListener.DropPressed = _inputManager.controls.Player.Drop.WasPressedThisFrame();
            _inputListener.CombinePressed = _inputManager.controls.Player.Combine.WasPressedThisFrame();
            _inputListener.TransferPressed = _inputManager.controls.Player.Transfer.WasPressedThisFrame();
            _inputListener.SubmitPressed = _inputManager.controls.Player.Submit.WasPressedThisFrame();
        }

        _sendTimer += Time.deltaTime;
        while (_sendTimer >= NetworkConstants.NET_TICK_INTERVAL)
        {
            _sendTimer -= NetworkConstants.NET_TICK_INTERVAL;

            NetworkManager.Instance.SendMessage(new BatchPlayerInputMessage
            {
                PlayerId = Identity.PlayerId,
                PlayerInputMessages = _simulator.InputBufferAsArray
            });
        }
    }

    void FixedUpdate()
    {
        if (Identity.IsLocalPlayer)
        {
            Simulate(_inputListener);
        }
        else
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
            });
        }
    }
    #endregion

    #region Initialization
    public void Initialize(PlayerConfig config, InputActionAsset inputManager, string id, bool isLocal)
    {
        _inputManager = new PlayerInputManager(inputManager);
        Identity.Initialize(id, isLocal);

        // Inventory = new PlayerInventory();
        // Interaction = new PlayerInteraction();

        // Inventory.Initialize(this, inputManager);
        // Interaction.Initialize(this, inputManager);
        _config = config;
        _animator.runtimeAnimatorController = _config.Anim;
        _spriteRenderer.sprite = _config.Icon;
    }
    #endregion

    #region Simulation
    private void Simulate(PlayerInputSnapshot inputSnapshot)
    {
        PlayerInputSnapshot cpy = new(inputSnapshot);

        if (cpy.MoveDir != Vector2.zero && cpy.DashPressed)
        {

        }

        if (cpy.PickupPressed)
        {

        }

        if (cpy.DropPressed)
        {

        }

        if (cpy.CombinePressed)
        {

        }

        if (cpy.TransferPressed)
        {

        }

        if (cpy.SubmitPressed)
        {

        }

        if (cpy.MoveDir != Vector2.zero)
            TryMove(cpy);
    }

    private bool TryMove(PlayerInputSnapshot inputSnapshot)
    {
        _simulator.Simulate(inputSnapshot,
            (inputSnapshot) =>
            {
                _animator.SetBool("IsMoving", inputSnapshot.MoveDir != Vector2.zero);
                _animator.SetFloat("MoveX", inputSnapshot.MoveDir.x);
                _animator.SetFloat("MoveY", inputSnapshot.MoveDir.y);
                transform.position = transform.position + (Vector3)(5 * Time.fixedDeltaTime * inputSnapshot.MoveDir);
                return new()
                {
                    Position = transform.position,
                };
            }
        );
        return true;
    }
    #endregion

    #region Server Message
    private void HandleNetworkMessage(ServerMessage message)
    {
        switch (message.MessageType)
        {
            case NetworkMessageTypes.Server.GameState.StateUpdate:
                GameStatesUpdate gameStatesUpdate = (GameStatesUpdate)message;
                if (Identity.IsLocalPlayer)
                {
                    PlayerSnapshot playerSnapshot = new();
                    for (int i = 0; i < gameStatesUpdate.GameStates.Length; i++)
                    {
                        GameStateUpdate gameState = gameStatesUpdate.GameStates[i];
                        for (int j = 0; j < gameState.PlayerStates.Length; j++)
                        {
                            PlayerState playerState = gameState.PlayerStates[j];
                            if (playerState.PlayerId == Identity.PlayerId &&
                                playerSnapshot.ProcessedInputSequence < playerState.ProcessedInputSequence)
                            {
                                playerSnapshot.ProcessedInputSequence = playerState.ProcessedInputSequence;
                                playerSnapshot.Position = new(playerState.PositionX, playerState.PositionY);
                                break;
                            }
                        }
                    }
                    Debug.Log(playerSnapshot.ProcessedInputSequence);
                    Debug.Log(playerSnapshot.Position);
                    TryReconcileServer(playerSnapshot);
                }
                else
                {
                    _interpolator.Store(gameStatesUpdate.GameStates, (gameStates) =>
                    {
                        for (int i = 0; i < gameStates.PlayerStates.Length; i++)
                        {
                            if (gameStates.PlayerStates[i].PlayerId == Identity.PlayerId)
                            {
                                return i;
                            }
                        }
                        return -1;
                    });
                }
                break;

        }
    }

    private void TryReconcileServer(PlayerSnapshot state)
    {
        _simulator.Reconcile(state,
            (serverSnapshot) =>
            {

            },
            (serverSnapshot, historySnapshot) =>
            {
                return Vector2.Distance(serverSnapshot.Position, historySnapshot.Position) > 0.1f;
            },
            (snapshot) =>
            {
                transform.position = snapshot.Position;
            },
            (inputMessage) =>
            {
                Vector2 moveDir = new(inputMessage.MoveDirX, inputMessage.MoveDirY);
                return new PlayerSnapshot()
                {
                    Position = transform.position + (Vector3)(5f * Time.fixedDeltaTime * moveDir)
                };
            }
        );
    }
    #endregion

    #region Utilities

    #endregion
}