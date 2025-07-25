using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private PlayerConfig _config;
    private PlayerInputManager _inputManager;
    [SerializeField] private Rigidbody2D _rb;
    [SerializeField] private Animator _anim;
    [SerializeField] private Animator _swordAnim;
    [SerializeField] private WeaponConfig _weapon;
    [SerializeField] private Transform _attackPoint;
    private InventoryUI _inventoryUI;

    public PlayerConfig Config => _config;
    public PlayerInventory Inventory { get; private set; }
    public PlayerAttack Attack { get; private set; }
    public PlayerInteraction Interaction { get; private set; }
    public PlayerMovement Movement { get; private set; }
    public Rigidbody2D Rb => _rb;
    public Animator Animatr => _anim;
    public Animator SwordAnimatr => _swordAnim;
    public WeaponConfig Weapon => _weapon;
    public Transform AttackPoint => _attackPoint;

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
    }
    
    void Update()
    {
        Movement.Update();
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        Interaction.OnTriggerEnter2D(collider);
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        Interaction.OnTriggerExit2D(collider);
    }

    void OnDestroy()
    {
        _inputManager.OnDestroy();
    }
}