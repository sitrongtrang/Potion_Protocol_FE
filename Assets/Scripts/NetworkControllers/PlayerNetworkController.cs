using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(NetworkIdentity))]
public class PlayerNetworkController : MonoBehaviour
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
    private Vector2 _playerDir;
    private bool _canAttack;
    // private NetworkPredictionBuffer<PlayerInputMessage, PlayerSnapshot> _networkPredictionBuffer = new(NetworkConstants.NET_PRED_BUFFER_SIZE);
    // private NetworkInterpolationBuffer<PlayerStateInterpolateData> _networkInterpolationBuffer = new(NetworkConstants.NET_INTERPOLATION_BUFFER_SIZE);

    [Header("Input")]
    private PlayerInputManager _inputManager;

    [Header("Game Components")]
    private PlayerConfig _config;
    private Animator _animator;
    private SpriteRenderer _spriteRenderer;
    private AABBCollider _collider;
    private Vector2 _size;
    private List<WeaponConfig> _weapons = new();
    private Animator _swordAnimator;

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
            _inputListener.CraftPressed = _inputManager.controls.Player.Combine.WasPressedThisFrame();
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
                float xDir = Mathf.Abs(serverState.PositionX - transform.position.x);
                float yDir = Mathf.Abs(serverState.PositionY - transform.position.y);
                Vector2 dir = new Vector2(xDir, yDir).normalized;
                _playerDir = dir;

                _animator.SetBool("IsMoving", dir != Vector2.zero);
                _animator.SetFloat("MoveX", dir.x);
                _animator.SetFloat("MoveY", dir.y);

                transform.position = new(serverState.PositionX, serverState.PositionY);
                Vector2 center = transform.position;
                _collider.SetBottomLeft(center - _size / 2f);
            });
        }
    }
    #endregion

    #region Initialization
    public void Initialize(PlayerConfig config, InputActionAsset inputManager, string id, bool isLocal)
    {
        _inputManager = new PlayerInputManager(inputManager);
        Identity.Initialize(id, isLocal);

        _config = config;
        _canAttack = true;
        _animator.runtimeAnimatorController = _config.Anim;
        _spriteRenderer.sprite = _config.Icon;
        Transform WeaponContainer = transform.Find("Weapons");
        for (int i = 0; i < WeaponContainer.childCount; i++)
        {
            if (i >= _weapons.Count) _weapons.Add(_config.Weapons[i]);
            else _weapons[i] = _config.Weapons[i];
            Transform weapon = WeaponContainer.GetChild(i);
            weapon.GetComponent<SpriteRenderer>().sprite = _weapons[i].Icon;
            weapon.GetComponent<Animator>().runtimeAnimatorController = _weapons[i].Anim;
            if (weapon.name == "Sword") _swordAnimator = weapon.GetComponent<Animator>();
        }

        _collider = AABBCollider.GetColliderBaseOnSprite(_spriteRenderer, transform);
        _size = _collider.Size;
    }
    #endregion

    #region Simulation
    private void Simulate(PlayerInputSnapshot inputSnapshot)
    {
        PlayerInputSnapshot cpy = new(inputSnapshot);

        if (cpy.MoveDir != Vector2.zero)
        {
            TryMove(cpy, cpy.DashPressed);
        }

        if (cpy.PickupPressed)
        {

        }

        if (cpy.DropPressed)
        {

        }

        if (cpy.CraftPressed)
        {

        }

        if (cpy.TransferPressed)
        {

        }

        if (cpy.SubmitPressed)
        {

        }

        if (cpy.AttackPressed)
        {
            TryAttack(cpy);
        }
    }

    private bool TryMove(PlayerInputSnapshot inputSnapshot, bool DashPressed)
    {
        _simulator.Simulate(inputSnapshot,
            (inputSnapshot) =>
            {
                _animator.SetBool("IsMoving", inputSnapshot.MoveDir != Vector2.zero);
                _animator.SetFloat("MoveX", inputSnapshot.MoveDir.x);
                _animator.SetFloat("MoveY", inputSnapshot.MoveDir.y);
                float moveSpeed = DashPressed ? _config.DashSpeed : _config.MoveSpeed;
                Vector2 targetPos = transform.position + (Vector3)(moveSpeed * Time.fixedDeltaTime * inputSnapshot.MoveDir);
                Vector2 resolvedPos = ContextSolver.ResolveStatic(transform.position, targetPos, _collider, CollisionSystem.Tree);
                _playerDir = inputSnapshot.MoveDir.normalized;
                transform.position = resolvedPos;
                Vector2 center = transform.position;
                _collider.SetBottomLeft(center - _size / 2f);
                return new()
                {
                    Position = transform.position,
                };
            }
        );
        return true;
    }

    private bool TryAttack(PlayerInputSnapshot inputSnapshot)
    {
        if (!_canAttack) return false;

        // If an alchemy nearby, cannot attack
        AlchemyControllerNetwork alchemy = FindFirstObjectByType<AlchemyControllerNetwork>();
        if (Vector2.Distance(alchemy.transform.position, transform.position) <= _config.InteractDistance)
        {
            return false;
        }

        // Check wall hit
        Vector2 dir = _playerDir.normalized;
        float skinWidth = 0.2f;
        Vector2 origin = (Vector2)transform.position + dir * skinWidth;
        bool hitObstacle = CheckWall(origin, dir);

        if (hitObstacle)
        {
            Debug.Log("Vướng tường nè má.");
            return false;
        }

        // Play animation
        _swordAnimator.SetTrigger("Attack");
        if (dir.x != 0 || dir.y != 0)
        {
            _swordAnimator.SetFloat("MoveX", dir.x);
            _swordAnimator.SetFloat("MoveY", dir.y);
            _canAttack = false;
            StartCoroutine(AttackCooldown());
            return true;
        }
        return false;
    }

    private IEnumerator AttackCooldown()
    {
        yield return new WaitForSeconds(_config.AttackCooldown);
        _canAttack = true;
    }

    private bool CheckWall(Vector2 origin, Vector2 dir)
    {
        float minDistanceToWall = 0.15f;
        List<AABBCollider> walls = CollisionSystem.RayCast(origin, dir, minDistanceToWall, EntityLayer.Obstacle);

        Debug.DrawRay(origin, dir.normalized * minDistanceToWall, Color.cyan, 2f);

        if (walls.Count > 0) return true;
        else return false;
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
                    Position = transform.position + (Vector3)(_config.MoveSpeed * Time.fixedDeltaTime * moveDir)
                };
            }
        );
    }
    #endregion

    #region Utilities

    #endregion
}