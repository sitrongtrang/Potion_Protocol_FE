using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private PlayerInventory _inventory;

    public PlayerInventory Inventory => _inventory;

    void Start()
    {
        _inventory = new PlayerInventory();
        GetComponent<PlayerInteraction>().Initialize(_inventory);
    }
    void Update()
    {
        GetComponent<PlayerAttack>().MyUpdate();
        GetComponent<PlayerInteraction>().MyUpdate();
        GetComponent<PlayerMovement>().MyUpdate();
    }
}