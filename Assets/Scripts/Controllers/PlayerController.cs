using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private PlayerConfig _config;
    [SerializeField] private PlayerInventory _inventory;
    private PlayerInteraction _interactionComponent;
    private PlayerAttack _attackComponent;
    private PlayerMovement _movementComponent;

    public PlayerConfig Config => _config;
    public PlayerInventory Inventory => _inventory;

    void Awake()
    {
        _inventory = new PlayerInventory();
        _interactionComponent = GetComponent<PlayerInteraction>();
        _attackComponent = GetComponent<PlayerAttack>();
        _movementComponent = GetComponent<PlayerMovement>();

        _interactionComponent.Initialize(_inventory);
        _attackComponent.Initialize(this);
    }
    
    void Update()
    {
        _interactionComponent.MyUpdate();
        _attackComponent.MyUpdate();
        _movementComponent.MyUpdate();
    }
}