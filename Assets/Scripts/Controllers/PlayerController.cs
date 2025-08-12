using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private PlayerConfig _config;
    private PlayerInputManager _inputManager;
    private Animator _animator;
    private SpriteRenderer _spriteRenderer;
    private List<WeaponConfig> _weapons = new();
    [SerializeField] private Animator _swordAnim;
    [SerializeField] private Transform _attackPoint;

    public PlayerConfig Config => _config;
    public PlayerInventory Inventory { get; private set; }
    public PlayerAttack Attack { get; private set; }
    public PlayerInteraction Interaction { get; private set; }
    public PlayerMovement Movement { get; private set; }
    public Animator Animatr => _animator;
    public Animator SwordAnimatr => _swordAnim;
    public List<WeaponConfig> Weapons => _weapons;
    public Transform AttackPoint => _attackPoint;

    public void Initialize(EntityConfig config, InputActionAsset loadedAsset = null)
    {
        if (config is PlayerConfig playerConfig)
        {
            _config = playerConfig;
            _animator = GetComponent<Animator>();
            _animator.runtimeAnimatorController = _config.Anim;
            _spriteRenderer = GetComponent<SpriteRenderer>();
            if (_spriteRenderer) _spriteRenderer.sprite = _config.Icon;

            Transform WeaponContainer = transform.Find("Weapons");
            for (int i = 0; i < WeaponContainer.childCount; i++)
            {
                if (i >= _weapons.Count) _weapons.Add(playerConfig.Weapons[i]);
                else _weapons[i] = playerConfig.Weapons[i];
                Transform weapon = WeaponContainer.GetChild(i);
                weapon.GetComponent<SpriteRenderer>().sprite = _weapons[i].Icon;
                weapon.GetComponent<Animator>().runtimeAnimatorController = _weapons[i].Anim;
            }
        }

        _inputManager = loadedAsset != null
            ? new PlayerInputManager(loadedAsset)
            : new PlayerInputManager();

        Inventory = new PlayerInventory();
        Attack = new PlayerAttack();
        Interaction = new PlayerInteraction();
        Movement = new PlayerMovement();

        Inventory.Initialize(_inputManager);
        Attack.Initialize(this, _inputManager);
        Interaction.Initialize(this, _inputManager);
        Movement.Initialize(this, _inputManager);

        InventoryUI inventoryUI = FindFirstObjectByType<InventoryUI>(FindObjectsInactive.Include);
        inventoryUI.Initialize(Inventory);
        inventoryUI.gameObject.SetActive(true);

        SkillContainerUI skillContainerUI = FindFirstObjectByType<SkillContainerUI>(FindObjectsInactive.Include);
        skillContainerUI.Initialize(Attack);
        skillContainerUI.gameObject.SetActive(true);
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