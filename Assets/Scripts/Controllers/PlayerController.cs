using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private PlayerInventory _inventory;

    public PlayerInventory Inventory => _inventory;

    void Start()
    {
        _inventory = new PlayerInventory();
    }

}