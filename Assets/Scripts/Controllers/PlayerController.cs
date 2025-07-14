using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private PlayerConfig _config;
    // input manager (new input system)
    private PlayerInputManager _inputManager;
    private List<IUpdatableComponent> _updatableComponents = new();
    [SerializeField] private Rigidbody2D _rb;
    [SerializeField] private Animator _anim;
    [SerializeField] private Animator _swordAnim;
    [SerializeField] private WeaponConfig _weapon;
    [SerializeField] private Transform attackPoint;

    public PlayerConfig Config => _config;
    public PlayerInventory Inventory { get; private set; }
    public PlayerAttack Attack { get; private set; }
    public PlayerInteraction Interaction { get; private set; }
    public PlayerMovement Movement { get; private set; }
    public Rigidbody2D Rb => _rb;
    public Animator Animatr => _anim;
    public Animator SwordAnimatr => _swordAnim;
    public WeaponConfig Weapon => _weapon;
    public Transform AttackPoint => attackPoint;

    public void Initialize(PlayerConfig config, InputActionAsset loadedAsset = null)
    {
        _config = config;
        _inputManager = loadedAsset != null
            ? new PlayerInputManager(loadedAsset)
            : new PlayerInputManager();

        Inventory = RegisterComponent(new PlayerInventory());
        Attack = RegisterComponent(new PlayerAttack());
        Interaction = RegisterComponent(new PlayerInteraction());
        Movement = RegisterComponent(new PlayerMovement());
    }
    
    void Update()
    {
        for (int i = 0; i < _updatableComponents.Count; i++)
        {
            _updatableComponents[i].MyUpdate();
        }
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        Interaction.OnTriggerEnter2D(collider);
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        Interaction.OnTriggerExit2D(collider);
    }

    private T RegisterComponent<T>(T component) where T : class
    {
        if (component is IComponent playerComponent)
        {
            playerComponent.Initialize(this, _inputManager);
        }

        if (component is IUpdatableComponent updatable)
        {
            _updatableComponents.Add(updatable);
        }

        return component;
    }
    void OnDestroy()
    {
        _inputManager.OnDestroy();
    }
}