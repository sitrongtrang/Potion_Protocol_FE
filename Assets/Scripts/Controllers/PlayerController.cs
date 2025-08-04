using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private PlayerConfig _config;
    private PlayerInputManager _inputManager;
    [SerializeField] private Animator _anim;
    [SerializeField] private Animator _swordAnim;
    [SerializeField] private WeaponConfig _weapon;
    [SerializeField] private Transform _attackPoint;
    private InventoryUI _inventoryUI;
    private SkillContainerUI _skillContainerUI;

    public PlayerConfig Config => _config;
    public PlayerInventory Inventory { get; private set; }
    public PlayerAttack Attack { get; private set; }
    public PlayerInteraction Interaction { get; private set; }
    public PlayerMovement Movement { get; private set; }
    public Animator Animatr => _anim;
    public Animator SwordAnimatr => _swordAnim;
    public WeaponConfig Weapon => _weapon;
    public Transform AttackPoint => _attackPoint;
    public float InteractionDistance = 0.6f;

    public void Initialize(InputActionAsset loadedAsset = null)
    {
        _inputManager = loadedAsset != null
            ? new PlayerInputManager(loadedAsset)
            : new PlayerInputManager();

        Inventory = new PlayerInventory();
        Attack = new PlayerAttack();
        Interaction = new PlayerInteraction();
        Movement = new PlayerMovement();

        Inventory.Initialize(this, _inputManager);
        Attack.Initialize(this, _inputManager);
        Interaction.Initialize(this, _inputManager);
        Movement.Initialize(this, _inputManager);

        _inventoryUI = FindFirstObjectByType<InventoryUI>();
        _inventoryUI.Initialize(this);
        _inventoryUI.gameObject.SetActive(true);

        _skillContainerUI = FindFirstObjectByType<SkillContainerUI>();
        _skillContainerUI.Initialize(this);
    }

    void Update()
    {
        Movement.Update();
    }

    void OnDestroy()
    {
        _inputManager.OnDestroy();
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(Movement.Collider.Bounds.center, Movement.Collider.Bounds.size);
    }
}